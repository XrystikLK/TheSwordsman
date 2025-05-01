using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SomeTest;

public class Skeleton : Enemy
{
    private const float gravity = 800f; // Сила гравитации (пикселей в секунду в квадрате)
    private Texture2D debugTexture;
    public Skeleton(ContentManager content, GraphicsDevice graphicsDevice, Vector2 startPosition, Player player)
        : base(startPosition, health: 50, damage: 10, graphicsDevice, player)
    {
        var idleAnimation = new AnimatedTexture(Vector2.Zero, 0f, 1f, 0f);
        idleAnimation.Load(content, "Enemy/Skeleton_Idle", frameCount: 8, framesPerSec: 9);
        var attackAnimation = new AnimatedTexture(Vector2.Zero, 0f, 1f, 0f);
        attackAnimation.Load(content, "Enemy/Skeleton_attack", frameCount: 10, framesPerSec: 15);
        var dieAnimation = new AnimatedTexture(Vector2.Zero, 0f, 1f, 0f);
        dieAnimation.Load(content, "Enemy/Skeleton_die", frameCount: 13, framesPerSec: 13);
        
        animations.Add("Idle", idleAnimation);
        animations.Add("Attack", attackAnimation);
        animations.Add("Die", dieAnimation);
        
        debugTexture = new Texture2D(graphicsDevice, 1, 1);
        debugTexture.SetData(new[] { Color.White });
        hitbox = new Rectangle((int)position.X, (int)position.Y, 75, 75);
        velocity = new Vector2(0, 0);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        // Применяем гравитацию, если не на земле
        if (!isGrounded)
        {
            velocity.Y += gravity * deltaTime;
        }
        
        position += velocity * deltaTime;
        hitbox.X = (int)(position.X);
        hitbox.Y = (int)(position.Y);
        if (_player._hitboxRect.Intersects(hitbox) && !isDying) currentAnimation = "Attack";
        else currentAnimation = "Idle";
        if (health <= 0) currentAnimation = "Die";

        // Меняем анимацию в зависимости от скорости
        // if (speed > 0)
        //     currentAnimation = "Run";
        // else
        //     currentAnimation = "Idle";
        //
        // position.X += speed;
    }
    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        spriteBatch.Draw(debugTexture, hitbox, Color.Red * 0.5f);
    }
}