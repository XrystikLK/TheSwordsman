using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SomeTest;

public class Skeleton : Enemy
{
    private float gravity = 800f;
    private Texture2D debugTexture;
    public Skeleton(ContentManager content, GraphicsDevice graphicsDevice, Vector2 startPosition, Player player)
        : base(startPosition, health: 25, damage: 10, graphicsDevice, player)
    {
        var idleAnimation = new AnimatedTexture(Vector2.Zero, 0f, 0.9f, 0f);
        idleAnimation.Load(content, "Enemy/WhiteSkeleton/Idle", frameCount: 8, framesPerSec: 9);
        
        var attackAnimation = new AnimatedTexture(Vector2.Zero, 0f, 0.9f, 0f);
        attackAnimation.Load(content, "Enemy/WhiteSkeleton/Attack", frameCount: 10, framesPerSec: 14);
        
        var dieAnimation = new AnimatedTexture(Vector2.Zero, 0f, 0.9f, 0f);
        dieAnimation.Load(content, "Enemy/WhiteSkeleton/Die", frameCount: 13, framesPerSec: 13);
        
        var walkingAnimation = new AnimatedTexture(Vector2.Zero, 0f, 0.9f, 0f);
        walkingAnimation.Load(content, "Enemy/WhiteSkeleton/Walk", frameCount: 10, framesPerSec: 12);
        
        var hurtAnimation = new AnimatedTexture(Vector2.Zero, 0f, 0.9f, 0f);
        hurtAnimation.Load(content, "Enemy/WhiteSkeleton/Hurt", frameCount: 5, framesPerSec: 10);
        
        animations.Add("Idle", idleAnimation);
        animations.Add("Attack", attackAnimation);
        animations.Add("Die", dieAnimation);
        animations.Add("Walk", walkingAnimation);
        animations.Add("Hurt", hurtAnimation);
        
        debugTexture = new Texture2D(graphicsDevice, 1, 1);
        debugTexture.SetData(new[] { Color.White });
        hitbox = new Rectangle((int)position.X, (int)position.Y, 35, 58);
        velocity = new Vector2(100f, 0);
        originalVelocity = velocity;
        _previousAnimation = "Idle";
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        Chase();
        Jumping();
        MeleeInteractionLogic(gameTime);
        // Применяем гравитацию, если не на земле
        if (!isGrounded)
        {
            velocity.Y += gravity * deltaTime;
        }

        _previousAnimation = currentAnimation;
        position += velocity * deltaTime;
        hitbox.X = (int)(position.X) + 30;
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
        spriteBatch.Draw(debugTexture, hitbox, Color.Red * 0.5f);
    }
}