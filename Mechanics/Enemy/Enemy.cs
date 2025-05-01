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
    protected bool isFlipped;
    public bool isGrounded = false; 
    public Rectangle hitbox;
    public Vector2 velocity;
    protected Dictionary<string, AnimatedTexture> animations;
    protected string currentAnimation;
    private float _lastDamageTime;
    private const float DamageCooldown = 1.0f;
    protected bool isDying = false;
    protected bool isRemoved = false;
    public bool IsRemoved => isRemoved;

    public Enemy(Vector2 startPosition, int health, int damage, GraphicsDevice graphicsDevice, Player player)
    {
        this.position = startPosition;
        this.health = health;
        this.damage = damage;
        this.animations = new Dictionary<string, AnimatedTexture>();
        this.currentAnimation = "Idle"; // По умолчанию
        this._player = player;
    }

    public virtual void Update(GameTime gameTime)
    {
        float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
        float total = (float)gameTime.TotalGameTime.TotalSeconds;

        // 1) Если в процессе “умирания” — обновляем только Die-анимацию
        if (isDying)
        {
            var dieAnim = animations[currentAnimation];
            dieAnim.UpdateFrame(elapsed);

            // Предполагаем, что AnimatedTexture имеет свойство IsFinished
            if (dieAnim.IsAnimationComplete)
                isRemoved = true;
                //hitbox = new Rectangle(0, 0, 0, 0);

            // не выполняем никакой другой логики
            return;
        }

        // 2) Обычное обновление анимации
        if (animations.ContainsKey(currentAnimation))
            animations[currentAnimation].UpdateFrame(elapsed);

        // 3) Логика столкновений и урона
        if (_player._hitboxRect.Intersects(hitbox))
            Console.WriteLine("Enemy hit player");

        if (_player.hitboxAttack.Intersects(hitbox) 
            && total - _lastDamageTime >= DamageCooldown)
        {
            _lastDamageTime = total;
            TakeDamage(_player.damage);
        }
    }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
        float distanceToPlayer = Vector2.Distance(new Vector2(hitbox.X, Hitbox.Y), new Vector2(_player._position.X, _player._position.Y));
        //Console.WriteLine(distanceToPlayer - hitbox.Width);
        
        if (animations.ContainsKey(currentAnimation))
        {
            animations[currentAnimation].DrawFrame(spriteBatch, position, isFlipped);
        }
        
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