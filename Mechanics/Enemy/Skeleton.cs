using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

public class Skeleton : Enemy
{
    private const float gravity = 800f; // Сила гравитации (пикселей в секунду в квадрате)
    private Vector2 velocity = Vector2.Zero; // Скорость перемещения
    public Rectangle hitbox;
    private Texture2D debugTexture;
    public Skeleton(ContentManager content, GraphicsDevice graphicsDevice, Vector2 startPosition)
        : base(startPosition, health: 50, damage: 10, graphicsDevice)
    {
        var idleAnimation = new AnimatedTexture(Vector2.Zero, 0f, 1f, 0f);
        idleAnimation.Load(content, "Enemy/Skeleton_Idle", frameCount: 8, framesPerSec: 9);

        //var runAnimation = new AnimatedTexture(Vector2.Zero, 0f, 1f, 0f);
        //runAnimation.Load(content, "Textures/SkeletonRun", frameCount: 8, framesPerSec: 12);

        animations.Add("Idle", idleAnimation);
        //animations.Add("Run", runAnimation);

        currentAnimation = "Idle";
        debugTexture = new Texture2D(graphicsDevice, 1, 1);
        debugTexture.SetData(new[] { Color.White });
        hitbox = new Rectangle((int)position.X, (int)position.Y, 75, 75);
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
        Console.WriteLine(hitbox);
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