using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SomeTest;

public class FireSpirit : Enemy
{
    private float gravity = 800f;
    private Texture2D debugTexture;
    public FireSpirit(ContentManager content, GraphicsDevice graphicsDevice, Vector2 startPosition, Player player)
        : base(startPosition, health: 1, damage: 15, graphicsDevice, player)
    {
        
        var walkingAnimation = new AnimatedTexture(Vector2.Zero, 0f, 0.2f, 0f);
        walkingAnimation.Load(content, "Enemy/FireSpirit/Walk", frameCount: 10, framesPerSec: 12);
        
        var dieAnimation = new AnimatedTexture(Vector2.Zero, 0f, 1.7f, 0f);
        dieAnimation.Load(content, "Enemy/FireSpirit/Die", frameCount: 5, framesPerSec: 6);
        
        animations.Add("Walk", walkingAnimation);
        animations.Add("Die", dieAnimation);
        
        debugTexture = new Texture2D(graphicsDevice, 1, 1);
        debugTexture.SetData(new[] { Color.White });
        hitbox = new Rectangle((int)position.X, (int)position.Y, 40, 58);
        velocity = new Vector2(250f, 300f);
        originalVelocity = velocity;
        _previousAnimation = "Walk";
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        float totalTime = (float)gameTime.TotalGameTime.TotalSeconds;
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        if (_player.hitboxAttack.Intersects(hitbox)) 
        {
            // Возможно сделать систему отталкивания, но пока так
            if (totalTime - _lastDamageTimeEnemy >= DamageCooldown)
            {
                //Console.WriteLine("Enemy get damage ");
                _lastDamageTimeEnemy = totalTime;
                TakeDamage(_player.damage);
            }
        }
            
        if (!isDying &&_player._hitboxRect.Intersects(hitbox))
        {
            if (totalTime - _lastDamageTimeHero >= DamageCooldown)
            {
                _lastDamageTimeHero = totalTime;
                _player.TakeDamage(damage);
                TakeDamage(_player.damage);
            }
        }
        
        
         
        FlyChase();
        // Применяем гравитацию, если не на земле
        
        _previousAnimation = currentAnimation;
        position += velocity * deltaTime;
        hitbox.X = (int)(position.X) + 5;
        hitbox.Y = (int)(position.Y);
        
        if (health >= 1) currentAnimation = "Walk";
        else currentAnimation = "Die";
        // if (_player._hitboxRect.Intersects(hitbox))
        // {
        //     
        //     if (totalTime - _lastDamageTimeHero >= DamageCooldown)
        //     {
        //         _lastDamageTimeHero = totalTime;
        //         _player.TakeDamage(damage);
        //         TakeDamage(health);
        //     }
        // }
        // if (_player.hitboxAttack.Intersects(hitbox))
        // {
        //     currentAnimation = "Die";
        //     TakeDamage(_player.damage);
        // }
        // else if (_player._hitboxRect.Intersects(hitbox))
        // {
        //     _player.TakeDamage(damage);
        //     TakeDamage(_player.damage);
        // }
        // else if (health <= 0){
        //     currentAnimation = "Die";
        //     //gravity = 0;
        // };
    }
    
    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        //spriteBatch.Draw(debugTexture, hitbox, Color.Red * 0.5f);
    }
}