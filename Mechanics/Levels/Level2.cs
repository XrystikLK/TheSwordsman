using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SomeTest.Maps;

public class Level2 : IScene
{
    private ContentManager contentManager;
    private SceneManager sceneManager;
    private Player player;
    private Texture2D texture;
    public Level2(ContentManager contentManager, SceneManager sceneManager, Player player)
    {
        this.player = player;
        this.sceneManager = sceneManager;
        this.contentManager = contentManager;
    }

    public void Load()
    {
        texture = contentManager.Load<Texture2D>("TextureAtlas/Dungeon");
    }

    public void Update(GameTime gameTime)
    {
        
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(texture, Vector2.Zero, Color.White);
    }
}