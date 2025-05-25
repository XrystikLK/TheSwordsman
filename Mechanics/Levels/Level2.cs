using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SomeTest.Maps;

public class Level2 : IScene
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
    public Level2(ContentManager contentManager, SceneManager sceneManager, GraphicsDevice graphicsDevice, Player player)
    {
        this.player = player;
        this.sceneManager = sceneManager;
        this.contentManager = contentManager;
        this.graphicsDevice = graphicsDevice;
        
    }

    public void Load()
    {
        texture = contentManager.Load<Texture2D>("TextureAtlas/All_content");
        
        mapFg = new LoadMap( "TextureAtlas/Dungeon", contentManager, graphicsDevice, 16);
        mapFg.LoadMapp("Level2/level2_fg.csv");
        mapMg = new LoadMap( "TextureAtlas/Dungeon", contentManager, graphicsDevice, 16);
        mapMg.LoadMapp("Level2/level2_mg.csv");
        mapCollision = new LoadMap("TextureAtlas/ALL_content", contentManager, graphicsDevice, 16);
        mapCollision.LoadMapp("Level2/level2_collision.csv");
        
        enemyManager = new EnemyManager();
        _skeleton = new Skeleton(contentManager, graphicsDevice, new Vector2(50, 495), player);
        
        enemyManager.AddEnemy(_skeleton);
        player._position = new Vector2(450, 0);
        player._velocity.Y = 1;
    }

    public void Update(GameTime gameTime)
    {
        var a = player._hitboxRect.X;
        var b = player._hitboxRect.Width;
        // Ограничение, чтобы игрок не смог убежать пока есть враги
        if (enemyManager.GetEnemies().Count != 0)
        {
            if (player._hitboxRect.X + player._hitboxRect.Width > 960)
            {
                player._position.X = 960 - 50;
            }
        }
        
        if (player._hitboxRect.X > 960)
        {
            sceneManager.AddScene(new Level3(contentManager, sceneManager, graphicsDevice, player));
        }
        mapCollision.Update(player);
        mapCollision.Update(_skeleton);
        enemyManager.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        mapMg.Draw(spriteBatch);
        mapFg.Draw(spriteBatch);
        enemyManager.Draw(spriteBatch);
        //spriteBatch.Draw(texture, Vector2.Zero, Color.White);
    }
    public int LevelNumber { get; } = 2;
}
