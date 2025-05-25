using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SomeTest;

public class Death : Enemy
{
    private float gravity = 800f;
    private Texture2D debugTexture;
    public Death(ContentManager content, GraphicsDevice graphicsDevice, Vector2 startPosition, Player player)
        : base(startPosition, health: 100, damage: 20, graphicsDevice, player)
    {
        var idle_walkAnimation = new AnimatedTexture(Vector2.Zero, 0f, 1.1f, 0f);
        idle_walkAnimation.Load(content, "Enemy/Death/Walk_Idle", frameCount: 8, framesPerSec: 9);
        
        var attackAnimation = new AnimatedTexture(Vector2.Zero, 0f, 1.1f, 0f);
        attackAnimation.Load(content, "Enemy/Death/Attack", frameCount: 8, framesPerSec: 10);
        
        var dieAnimation = new AnimatedTexture(Vector2.Zero, 0f, 1.1f, 0f);
        dieAnimation.Load(content, "Enemy/Death/Die", frameCount: 12, framesPerSec: 14);
        
        var hurtAnimation = new AnimatedTexture(Vector2.Zero, 0f, 1.1f, 0f);
        hurtAnimation.Load(content, "Enemy/Death/Hurt", frameCount: 2, framesPerSec: 4);
        
        animations.Add("Idle", idle_walkAnimation);
        animations.Add("Attack", attackAnimation);
        animations.Add("Die", dieAnimation);
        animations.Add("Walk", idle_walkAnimation);
        animations.Add("Hurt", hurtAnimation);
        
        debugTexture = new Texture2D(graphicsDevice, 1, 1);
        debugTexture.SetData(new[] { Color.White });
        hitbox = new Rectangle((int)position.X, (int)position.Y, 40, 70);
        velocity = new Vector2(100f, 0);
        originalVelocity = velocity;
        _previousAnimation = "Idle";
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        Chase(true);
        MeleeInteractionLogic(gameTime, 3);
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        // Применяем гравитацию, если не на земле
        if (!isGrounded)
        {
            velocity.Y += gravity * deltaTime;
        }

        _previousAnimation = currentAnimation;
        position += velocity * deltaTime;
        hitbox.X = (int)(position.X) + 25;
        hitbox.Y = (int)(position.Y);
        if (_player.hitboxAttack.Intersects(hitbox) && !isDying && _player.isAttacking)
        {
            isHurting = true;
        }
        if (isHurting)
        {
            currentAnimation = "Hurt";
            if (animations["Hurt"].IsAnimationComplete) isHurting = false;
        }
        else if (_player._hitboxRect.Intersects(hitbox) && !isDying) currentAnimation = "Attack";
        
        else if (velocity.X !=  0) currentAnimation = "Walk";
        
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