using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SomeTest.Maps;

public class Level6 : IScene
{
    private ContentManager contentManager;
    private SceneManager sceneManager;
    private GraphicsDevice graphicsDevice;
    private Player player;
    
    private EnemyManager enemyManager;

    private GoldSkeleton _goldSkeleton;
    private FireSpirit _fireSpirit;
    private ArcaneArcher _arcaneArcher;
    private Death _death;
    
    private LoadMap mapFg;
    private LoadMap mapMg;
    private LoadMap mapCollision;
    private Texture2D texture;
    public Level6(ContentManager contentManager, SceneManager sceneManager, GraphicsDevice graphicsDevice, Player player)
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
        mapFg.LoadMapp("Level6/level6_fg.csv");
        mapMg = new LoadMap( "TextureAtlas/Dungeon", contentManager, graphicsDevice, 16);
        mapMg.LoadMapp("Level6/level6_mg.csv");
        mapCollision = new LoadMap( "TextureAtlas/ALL_content", contentManager, graphicsDevice, 16);
        mapCollision.LoadMapp("Level6/level6_collision.csv");
        
        enemyManager = new EnemyManager();

        _goldSkeleton = new GoldSkeleton(contentManager, graphicsDevice, new Vector2(400, 580), player);
        _death = new Death(contentManager, graphicsDevice, new Vector2(250, 40), player);
        _fireSpirit = new FireSpirit(contentManager, graphicsDevice, new Vector2(800, 580), player);
        _arcaneArcher = new ArcaneArcher(contentManager, graphicsDevice, new Vector2(800, 580), player);
        
        enemyManager.AddEnemy(_fireSpirit);
        enemyManager.AddEnemy(_goldSkeleton);
        enemyManager.AddEnemy(_death);
        
        player._position = new Vector2(-10, 368);
    }

    public void Update(GameTime gameTime)
    {
        var a = player._hitboxRect.X;
        var b = player._hitboxRect.Width;
        Console.WriteLine(enemyManager.GetEnemies().Count);
        if (player._hitboxRect.X <= 0 && enemyManager.GetEnemies().Count != 0)
        {
            player._position.X = - 25;
        }
        
        if (player._hitboxRect.X + player._hitboxRect.Width < 0 && player._hitboxRect.Y < 145 && enemyManager.GetEnemies().Count == 0)
        {
            sceneManager.AddScene(new Level5(contentManager, sceneManager, graphicsDevice, player));
        }
        
        mapCollision.Update(player);
        mapCollision.Update(_goldSkeleton);
        mapCollision.Update(_death);
        enemyManager.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        mapMg.Draw(spriteBatch);
        mapFg.Draw(spriteBatch);
        enemyManager.Draw(spriteBatch);
    }
    public int LevelNumber { get; } = 6;
}
