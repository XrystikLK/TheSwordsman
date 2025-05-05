using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SomeTest.Maps;

public class Level1 : IScene
{
    private ContentManager contentManager;
    private SceneManager sceneManager;
    private Player player;
    private Texture2D texture;
    public Level1(ContentManager contentManager, SceneManager sceneManager, Player player)
    {
        this.player = player;
        this.sceneManager = sceneManager;
        this.contentManager = contentManager;
    }

    public void Load()
    {
        texture = contentManager.Load<Texture2D>("TextureAtlas/All_content");
    }

    public void Update(GameTime gameTime)
    {
        var a = player._hitboxRect.X;
        var b = player._hitboxRect.Width;
        if (player._hitboxRect.X + player._hitboxRect.Width > 900)
        {
            sceneManager.AddScene(new Level2(contentManager, sceneManager, player));
        }

        Console.WriteLine($"{a}+{b}");
        // if (Keyboard.GetState().IsKeyDown(Keys.Back))
        // {
        //     sceneManager.AddScene(new Level2(contentManager, sceneManager));
        // }
        //
        // if (Keyboard.GetState().IsKeyDown(Keys.G))
        // {
        //     sceneManager.AddScene(new Level1(contentManager, sceneManager));
        // }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(texture, Vector2.Zero, Color.White);
    }
}
