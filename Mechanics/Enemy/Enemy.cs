using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

public abstract class Enemy
{
    protected Vector2 position;
    protected int health;
    protected int damage;
    protected bool isFlipped;
    protected bool isGrounded = false; 

    protected Dictionary<string, AnimatedTexture> animations;
    protected string currentAnimation;

    public Enemy(Vector2 startPosition, int health, int damage,GraphicsDevice graphicsDevice)
    {
        this.position = startPosition;
        this.health = health;
        this.damage = damage;
        this.animations = new Dictionary<string, AnimatedTexture>();
        this.currentAnimation = "Idle"; // По умолчанию
    }

    public virtual void Update(GameTime gameTime)
    {
        
        // Обновить текущую анимацию
        if (animations.ContainsKey(currentAnimation))
        {
            animations[currentAnimation].UpdateFrame((float)gameTime.ElapsedGameTime.TotalSeconds);
        }
    }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
        if (animations.ContainsKey(currentAnimation))
        {
            animations[currentAnimation].DrawFrame(spriteBatch, position, isFlipped);
        }
        
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        // Тут можно включить анимацию смерти или просто убрать врага
    }
    
    public Vector2 Position
    {
        get => position;
        set => position = value;
    }

    public int Health => health;
    public int Damage => damage;
}