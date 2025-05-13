using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using SomeTest;

public abstract class Enemy
{
    protected Player _player;
    protected Vector2 position;
    protected int health;
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

    public virtual void Update(GameTime gameTime)
    {
        float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
        float total = (float)gameTime.TotalGameTime.TotalSeconds; 
        playerIsRight = _player._position.X > position.X;
        playerIsBottom = _player._hitboxRect.Center.Y < position.Y;
        // 1) Если в процессе “умирания” — обновляем только Die-анимацию
        if (isDying)
        {
            var dieAnim = animations["Die"];
            dieAnim.UpdateFrame(elapsed);
        
            if (dieAnim.IsAnimationComplete)
                isRemoved = true;
            //hitbox = new Rectangle(0, 0, 0, 0);
            
            return;
        }
        
        // 2) Обычное обновление анимации
        if (animations.ContainsKey(currentAnimation))
        {
            if (currentAnimation == "Attack" && _previousAnimation != "Attack") animations["Attack"].Reset();
            if (currentAnimation == "Hurt" && _previousAnimation != "Hurt") animations["Hurt"].Reset();
            animations[currentAnimation].UpdateFrame(elapsed);
        }
        
        // Наверное нужно будет это прописывать во всех классах
        
    }

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
    /// Логика взаимодействия игрока и моба
    /// </summary>
    /// <param name="gameTime">Игрвое время</param>
    public void MeleeInteractionLogic(GameTime gameTime)
    {
        float total = (float)gameTime.TotalGameTime.TotalSeconds; 
        if ((_player._hitboxRect.Intersects(hitbox) || _player.hitboxAttack.Intersects(hitbox) || isHurting))
        {
            velocity.X = 0;
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
            if (_player._hitboxRect.Intersects(hitbox))
            {
                if (total - _lastDamageTimeHero >= DamageCooldown)
                {
                    _lastDamageTimeHero = total;
                    _player.TakeDamage(damage);
                    //Нужно сделать чтобы игрока отталкивало назад при столкновении
                    //Console.WriteLine("Enemy hit player");
                }
            
            }
        }
        else velocity.X = originalVelocity.X;
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        Console.WriteLine(health);
        if (health <= 0)
        {
            Die();
            Console.WriteLine("Enemy is dead");
        }
    }

    protected virtual void Die()
    {
        isDying = true;
        velocity = Vector2.Zero;
    }
    
    
    public void SetPosition(Vector2 newPosition)
    {
        position = newPosition;
    }

    public void SetPositionY(float newY)
    {
        position.Y = newY;
    }

    public void Chase()
    {
        if (isDying) return;
        // float distanceToPlayer = Vector2.Distance(new Vector2(hitbox.X, Hitbox.Y), new Vector2(_player._position.X, _player._position.Y));
        // Console.WriteLine(distanceToPlayer);
        
        velocity.X = playerIsRight ? Math.Abs(velocity.X) : -Math.Abs(velocity.X);
    }

    public void Archer()
    {
        if (isDying) return;
        var distanceToPlayer = Vector2.Distance(_player._position, position);
        velocity.X = playerIsRight ? Math.Abs(velocity.X) : -Math.Abs(velocity.X);
        Console.WriteLine(distanceToPlayer);
    }
        

    public void FlyChase()
    {
        if (isDying) return;
        velocity.X = playerIsRight ? Math.Abs(velocity.X) : -Math.Abs(velocity.X);
        velocity.Y = playerIsBottom ? -Math.Abs(velocity.Y) : Math.Abs(velocity.Y);
    }
    public void Jumping()
    {
        if (isDying) return;
        if (isGrounded && isStack)
        {
            const float jumpForce = -200f;
            velocity.Y = jumpForce;
            isGrounded = false;
        }
    }
    public void SetPositionX(float newX)
    {
        position.X = newX;
    }
    
    public Vector2 Position
    {
        get => position;
        set => position = value;
    }

    public int Health => health;
    public int Damage => damage;
    public Rectangle Hitbox => hitbox;
}