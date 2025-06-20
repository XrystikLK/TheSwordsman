using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SomeTest.Maps;

public class Level3 : IScene
{
    private ContentManager contentManager;
    private SceneManager sceneManager;
    private GraphicsDevice graphicsDevice;
    private Player player;
    
    private EnemyManager enemyManager;
    private Skeleton _skeleton;
    private GoldSkeleton _goldSkeleton;
    private Chest _chest;
    
    private LoadMap mapFg;
    private LoadMap mapMg;
    private LoadMap mapCollision;
    private Texture2D texture;
    public Level3(ContentManager contentManager, SceneManager sceneManager, GraphicsDevice graphicsDevice, Player player)
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
        mapFg.LoadMapp("Level3/level3_fg.csv");
        mapMg = new LoadMap( "TextureAtlas/Dungeon", contentManager, graphicsDevice, 16);
        mapMg.LoadMapp("Level3/level3_mg.csv");
        mapCollision = new LoadMap( "TextureAtlas/ALL_content", contentManager, graphicsDevice, 16);
        mapCollision.LoadMapp("Level3/level3_collision.csv");
        
        enemyManager = new EnemyManager();
        
        _skeleton = new Skeleton(contentManager, graphicsDevice, new Vector2(250, 450), player);
        _goldSkeleton = new GoldSkeleton(contentManager, graphicsDevice, new Vector2(750, 310), player);
        _chest = new Chest(contentManager, graphicsDevice, new Vector2(835, 350), player);
        
        enemyManager.AddEnemy(_skeleton);
        enemyManager.AddEnemy(_goldSkeleton);
        enemyManager.AddEnemy(_chest);
        player._position = new Vector2(40, 450);
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
        if (player._hitboxRect.X <= 0) player._position.X = - 25; 
        
        
        if (player._hitboxRect.X  > 960)
        {
            sceneManager.AddScene(new Level4(contentManager, sceneManager, graphicsDevice, player));
        }
        mapCollision.Update(player);
        mapCollision.Update(_skeleton);
        mapCollision.Update(_goldSkeleton);
        mapCollision.Update(_chest);
        enemyManager.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        mapFg.Draw(spriteBatch);
        mapMg.Draw(spriteBatch);
        enemyManager.Draw(spriteBatch);
        //spriteBatch.Draw(texture, Vector2.Zero, Color.White);
    }
    public int LevelNumber { get; } = 3;
}
