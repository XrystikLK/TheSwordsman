using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SomeTest;

public class Chest : Enemy
{
    private float gravity = 800f;
    private Texture2D debugTexture;
    private bool _isActive = false;
    public Chest(ContentManager content, GraphicsDevice graphicsDevice, Vector2 startPosition, Player player)
        : base(startPosition, health: 25, damage: 10, graphicsDevice, player)
    {
        
        var attackAnimation = new AnimatedTexture(Vector2.Zero, 0f, 1.1f, 0f);
        attackAnimation.Load(content, "Enemy/Chest/Attack", frameCount: 8, framesPerSec: 10);
        
        var dieAnimation = new AnimatedTexture(Vector2.Zero, 0f, 1.1f, 0f);
        dieAnimation.Load(content, "Enemy/Chest/Die", frameCount: 8, framesPerSec: 10);
        
        var walkingAnimation = new AnimatedTexture(Vector2.Zero, 0f, 1.1f, 0f);
        walkingAnimation.Load(content, "Enemy/Chest/Walk", frameCount: 6, framesPerSec: 8);
        
        var hurtAnimation = new AnimatedTexture(Vector2.Zero, 0f, 1.1f, 0f);
        hurtAnimation.Load(content, "Enemy/Chest/Hurt", frameCount: 3, framesPerSec: 5);
        
        var idleAnimation = new AnimatedTexture(Vector2.Zero, 0, 1.1f, 0f);
        idleAnimation.Load(content, "Enemy/Chest/Idle", frameCount: 7, framesPerSec: 7);
        
        animations.Add("Idle", idleAnimation);
        animations.Add("Attack", attackAnimation);
        animations.Add("Die", dieAnimation);
        animations.Add("Walk", walkingAnimation);
        animations.Add("Hurt", hurtAnimation);
        
        debugTexture = new Texture2D(graphicsDevice, 1, 1);
        debugTexture.SetData(new[] { Color.White });
        hitbox = new Rectangle((int)position.X, (int)position.Y, 41, 65);
        velocity = new Vector2(0, 0);
        originalVelocity = velocity;
        _previousAnimation = "Idle";
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        
        Console.WriteLine(hitbox);
        
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        float total = (float)gameTime.TotalGameTime.TotalSeconds;
        Chase();
        Jumping();
        // Применяем гравитацию, если не на земле
        if (!isGrounded)
        {
            velocity.Y += gravity * deltaTime;
        }
        if ((_player._hitboxRect.Intersects(hitbox) || _player.hitboxAttack.Intersects(hitbox) || isHurting))
        {
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
        
        _previousAnimation = currentAnimation;
        position += velocity * deltaTime;
        hitbox.X = (int)(position.X) + 15;
        hitbox.Y = (int)(position.Y) + 40;
        if (_player.hitboxAttack.Intersects(hitbox) && !isDying && _player.isAttacking)
        {
            isHurting = true;
        }
        if (isHurting)
        {
            currentAnimation = "Hurt";
            if (animations["Hurt"].IsAnimationComplete) isHurting = false;
        }
        else if (_player._hitboxRect.Intersects(hitbox) && !isDying)
        {
            currentAnimation = "Attack";
            _isActive = true;
        }
        else if (_isActive) currentAnimation = "Walk";
        
        if (health <= 0){
            currentAnimation = "Die";
            //gravity = 0;
        };
    }
    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        //spriteBatch.Draw(debugTexture, hitbox, Color.Red * 0.5f);
    }
}