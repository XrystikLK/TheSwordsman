using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using SomeTest;

/// <summary>
/// Базовый абстрактный класс врага, содержащий общую логику поведения и взаимодействия с игроком.
/// </summary>
public abstract class Enemy
{
    protected Player _player;
    protected Vector2 position;
    public int health;
    protected int damage;
    public bool isGrounded = false; 
    public Rectangle hitbox;
    public Vector2 velocity;
    public bool isStack = false;
    protected Dictionary<string, AnimatedTexture> animations;
    protected string currentAnimation;
    public float _lastDamageTimeEnemy;
    public float _lastDamageTimeHero;
    public const float DamageCooldown = 1.0f;
    protected bool isDying = false;
    protected bool isRemoved = false;
    public bool playerIsRight;
    public bool playerIsBottom;
    public string _previousAnimation;
    private Vector2 knockbackVelocity;
    public bool isHurting = false;
    public Vector2 originalVelocity;
    
    public bool IsRemoved => isRemoved;

    /// <summary>
    /// Конструктор врага
    /// </summary>
    /// <param name="startPosition">Начальная позиция врага</param>
    /// <param name="health">Здоровье врага</param>
    /// <param name="damage">Урон врага</param>
    /// <param name="graphicsDevice">Графическое устройство</param>
    /// <param name="player">Ссылка на объект игрока</param>
    public Enemy(Vector2 startPosition, int health, int damage, GraphicsDevice graphicsDevice, Player player)
    {
        this.position = startPosition;
        this.health = health;
        this.damage = damage;
        this.animations = new Dictionary<string, AnimatedTexture>();
        this.currentAnimation = "Idle";
        this._player = player;
        knockbackVelocity = new Vector2(300f, 200f);
    }

    /// <summary>
    /// Обновляет состояние врага.
    /// </summary>
    public virtual void Update(GameTime gameTime)
    {
        float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
        float total = (float)gameTime.TotalGameTime.TotalSeconds; 
        playerIsRight = _player._position.X > position.X;
        playerIsBottom = _player._hitboxRect.Y < position.Y;
        // 1) Если в процессе “умирания” — обновляем только Die-анимацию
        if (isDying)
        {
            var dieAnim = animations["Die"];
            dieAnim.UpdateFrame(elapsed);
        
            if (dieAnim.IsAnimationComplete)
                isRemoved = true;
            
            return;
        }
        
        // 2) Обычное обновление анимации
        if (animations.ContainsKey(currentAnimation))
        {
            if (currentAnimation == "Attack" && _previousAnimation != "Attack") animations["Attack"].Reset();
            if (currentAnimation == "Hurt" && _previousAnimation != "Hurt") animations["Hurt"].Reset();
            animations[currentAnimation].UpdateFrame(elapsed);
        }
    }

    /// <summary>
    /// Отрисовывает врага на экране
    /// </summary>
    public virtual void Draw(SpriteBatch spriteBatch)
    {
        //Console.WriteLine(distanceToPlayer - hitbox.Width);
        if (animations.ContainsKey(currentAnimation))
        {
            if (playerIsRight) animations[currentAnimation].DrawFrame(spriteBatch, position);
            else animations[currentAnimation].DrawFrame(spriteBatch, position, true);
        }
    }

    
    /// <summary>
    /// Логика ближнего боя между игроком и врагом
    /// </summary>
    /// <param name="gameTime">Игровое время</param>
    /// <param name="frameGetDamage">Кадр в анимации врага, на которых игрок должен получить урон</param>
    /// <param name="stopWhenHurting">Останавливать ли врага при получении урона</param>
    /// <param name="slowWhenHurting">Замедлять врага при нанесении урона</param>
    /// <param name="stopWhenIntersection">Останавливать врага при пересечении хотбоксов с игроком</param>
    public void MeleeInteractionLogic(GameTime gameTime, int? frameGetDamage = null, bool stopWhenHurting = false, bool slowWhenHurting= false, bool stopWhenIntersection = false ) // frameGetDamage - кадр из анимации на которой должен проходить урон
    {
        float total = (float)gameTime.TotalGameTime.TotalSeconds; 
        if ((_player._hitboxRect.Intersects(hitbox) || _player.hitboxAttack.Intersects(hitbox) || isHurting) && !isDying)
        {
            if (stopWhenHurting && isHurting) velocity.X = 0;
            if (slowWhenHurting && isHurting) velocity.X = playerIsRight ? Math.Abs(35f) : -Math.Abs(35f);
            if (stopWhenIntersection && _player._hitboxRect.Intersects(hitbox)) velocity.X = 0;
            // 4) Логика нанесения урона монстру
            if (_player.hitboxAttack.Intersects(hitbox)) 
            {
                // Возможно сделать систему отталкивания, но пока так
                if (total - _lastDamageTimeEnemy >= DamageCooldown)
                {
                    //Console.WriteLine("Enemy get damage ");
                    _lastDamageTimeEnemy = total;
                    TakeDamage(_player.damage);
                }
            }
            // 3) Логика столкновений и урона
            bool shouldApplyDamage = 
                frameGetDamage == null || 
                animations["Attack"].frame == frameGetDamage.Value;
            
            if (_player._hitboxRect.Intersects(hitbox) && shouldApplyDamage)
            {
                if (total - _lastDamageTimeHero >= DamageCooldown)
                {
                    _lastDamageTimeHero = total;
                    _player.TakeDamage(damage);
                }
            }
        }
        // else
        // {
        //     if (stopWhenHurting) velocity.X = originalVelocity.X;
        // }
    }
    /// <summary>
    /// Логика ближнего боя между игроком и врагом
    /// </summary>
    /// <param name="gameTime">Игровое время</param>
    /// <param name="frameGetDamage">Кадр в анимации врага, на которых игрок должен получить урон</param>
    /// <param name="stopWhenHurting">Останавливать ли врага при получении урона</param>
    /// <param name="slowWhenHurting">Замедлять врага при нанесении урона</param>
    /// <param name="stopWhenIntersection">Останавливать врага при пересечении хотбоксов с игроком</param>
    public void MeleeInteractionLogic(GameTime gameTime, ref int bossHealthBar, int? frameGetDamage = null, bool stopWhenHurting = false, bool slowWhenHurting= false, bool stopWhenIntersection = false ) // frameGetDamage - кадр из анимации на которой должен проходить урон
    {
        float total = (float)gameTime.TotalGameTime.TotalSeconds; 
        if ((_player._hitboxRect.Intersects(hitbox) || _player.hitboxAttack.Intersects(hitbox) || isHurting) && !isDying)
        {
            if (stopWhenHurting && isHurting) velocity.X = 0;
            if (slowWhenHurting && isHurting) velocity.X = playerIsRight ? Math.Abs(35f) : -Math.Abs(35f);
            if (stopWhenIntersection && _player._hitboxRect.Intersects(hitbox)) velocity.X = 0;
            // 4) Логика нанесения урона монстру
            if (_player.hitboxAttack.Intersects(hitbox)) 
            {
                // Возможно сделать систему отталкивания, но пока так
                if (total - _lastDamageTimeEnemy >= DamageCooldown)
                {
                    bossHealthBar -= 20;
                    //Console.WriteLine("Enemy get damage ");
                    _lastDamageTimeEnemy = total;
                    TakeDamage(_player.damage);
                }
            }
            // 3) Логика столкновений и урона
            bool shouldApplyDamage = 
                frameGetDamage == null || 
                animations["Attack"].frame == frameGetDamage.Value;
            
            if (_player._hitboxRect.Intersects(hitbox) && shouldApplyDamage)
            {
                if (total - _lastDamageTimeHero >= DamageCooldown)
                {
                    _lastDamageTimeHero = total;
                    _player.TakeDamage(damage);
                }
            }
        }
        // else
        // {
        //     if (stopWhenHurting) velocity.X = originalVelocity.X;
        // }
    }

    /// <summary>
    /// Нанесение урона врагу
    /// </summary>
    /// <param name="amount">Количество урона</param>
    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Die();
            Console.WriteLine("Enemy is dead");
        }
    }

    /// <summary>
    /// Вызывает гибель врага
    /// </summary>
    protected virtual void Die()
    {
        isDying = true;
        velocity = Vector2.Zero;
    }
    

    /// <summary>
    /// Устанавлиет новую позицию врага(ось OY)
    /// </summary>
    /// <param name="newY">Значение координаты по оси OY</param>
    public void SetPositionY(float newY)
    {
        position.Y = newY;
    }

    /// <summary>
    /// Логика преследования игрока
    /// </summary>
    /// <param name="haveDetectionRange">Имеет ли враг радиус обнаружения</param>
    
    public void Chase(bool haveDetectionRange = false)
    {
        if (isDying)
        {
            velocity.X = 0;
            return;
        }
        //Если враг находится под игроком
        if (_player._position.Y - position.Y < -20f && Math.Abs(_player._position.X - position.X) <= 2f)
        {
            velocity.X = 0;
            return;
        }
        
        float distanceToPlayer = Vector2.Distance(new Vector2(hitbox.X, Hitbox.Y), new Vector2(_player._position.X, _player._position.Y));
        // Console.WriteLine(distanceToPlayer);
        if (haveDetectionRange)
        {
            if (distanceToPlayer <= 350)
            {
                if (!isHurting)
                {
                    velocity.X = playerIsRight ? Math.Abs(originalVelocity.X) : -Math.Abs(originalVelocity.X);
                    
                }
                // else if (slowWhenHurting)
                // {
                //     velocity.X = playerIsRight ? Math.Abs(35f) : -Math.Abs(35f);
                // }
                else velocity.X = playerIsRight ? Math.Abs(originalVelocity.X) : -Math.Abs(originalVelocity.X);
            }
            else
            {
                if (!isHurting)
                {
                    velocity.X = 0;
                }
            }
        }
        else velocity.X = playerIsRight ? Math.Abs(originalVelocity.X) : -Math.Abs(originalVelocity.X);
       
    }

    public void Archer()
    {
        if (isDying) return;
        var distanceToPlayer = Vector2.Distance(_player._position, position);
        velocity.X = playerIsRight ? Math.Abs(velocity.X) : -Math.Abs(velocity.X);
    }
        

    /// <summary>
    /// Логика преследования игрока по воздуху
    /// </summary>
    public void FlyChase()
    {
        if (isDying) return;
        velocity.X = playerIsRight ? Math.Abs(velocity.X) : -Math.Abs(velocity.X);
        velocity.Y = playerIsBottom ? -Math.Abs(velocity.Y) : Math.Abs(velocity.Y);
    }
    /// <summary>
    /// Логика прыжка
    /// </summary>
    public void Jumping()
    {
        if (isDying) return;
        if (isGrounded && isStack)
        {
            const float jumpForce = -220f;
            velocity.Y = jumpForce;
            isGrounded = false;
        }
    }
    public void SetPositionX(float newX)
    {
        position.X = newX;
    }
    
    // public Vector2 Position
    // {
    //     get => position;
    //     set => position = value;
    // }
    //
    // public int Health => health;
    // public int Damage => damage;
    public Rectangle Hitbox => hitbox;
}