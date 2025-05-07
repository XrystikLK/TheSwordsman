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
    private GraphicsDevice graphicsDevice;
    private Player player;
    
    private EnemyManager enemyManager;
    private Skeleton _skeleton;

    private LoadMap mapFg;
    private LoadMap mapMg;
    private LoadMap mapCollision;
    private Texture2D texture;
    public Level1(ContentManager contentManager, SceneManager sceneManager, GraphicsDevice graphicsDevice, Player player)
    {
        this.player = player;
        this.sceneManager = sceneManager;
        this.contentManager = contentManager;
        this.graphicsDevice = graphicsDevice;
        
    }

    public void Load()
    {
        texture = contentManager.Load<Texture2D>("TextureAtlas/All_content");
        
        mapFg = new LoadMap("../../../Maps/Level1/level1_fg.csv", "TextureAtlas/Dungeon", contentManager, graphicsDevice, 16);
        mapFg.LoadMapp("../../../Maps/Level1/level1_fg.csv");
        mapMg = new LoadMap("../../../Maps/Level1/level1_mg.csv", "TextureAtlas/Dungeon", contentManager, graphicsDevice, 16);
        mapMg.LoadMapp("../../../Maps/Level1/level1_mg.csv");
        mapCollision = new LoadMap("../../../Maps/Level1/level1_collision.csv", "TextureAtlas/ALL_content", contentManager, graphicsDevice, 16);
        mapCollision.LoadMapp("../../../Maps/Level1/level1_collision.csv");
        
        enemyManager = new EnemyManager(); 
        //_skeleton = new Skeleton(contentManager, graphicsDevice, new Vector2(50, 450), player);
        //enemyManager.AddEnemy(_skeleton);
        player._position = new Vector2(450, 0);
    }

    public void Update(GameTime gameTime)
    {
        var a = player._hitboxRect.X;
        var b = player._hitboxRect.Width;
        if (player._hitboxRect.Y + player._hitboxRect.Height > 700)
        {
            sceneManager.AddScene(new Level2(contentManager, sceneManager, graphicsDevice, player));
        }
        mapCollision.Update(player);
        //mapCollision.Update(_skeleton);
        //enemyManager.Update(gameTime);
        //Console.WriteLine(player._position);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        
        mapMg.Draw(spriteBatch);
        mapFg.Draw(spriteBatch);
        //enemyManager.Draw(spriteBatch);
        //spriteBatch.Draw(texture, Vector2.Zero, Color.White);
    }
}
