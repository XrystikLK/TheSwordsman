using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SomeTest.Maps;

public class Level5 : IScene
{
    private ContentManager contentManager;
    private SceneManager sceneManager;
    private GraphicsDevice graphicsDevice;
    private Player player;
    
    private EnemyManager enemyManager;
    private Skeleton _skeleton;
    private GoldSkeleton _goldSkeleton;
    private FireSpirit _fireSpirit;
    private ArcaneArcher _arcaneArcher;
    private Death _death;
    private Chest _chest;
    
    private LoadMap mapFg;
    private LoadMap mapMg;
    private LoadMap mapCollision;
    private Texture2D texture;
    public Level5(ContentManager contentManager, SceneManager sceneManager, GraphicsDevice graphicsDevice, Player player)
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
        mapFg.LoadMapp("Level5/level5_7_fg.csv");
        mapMg = new LoadMap( "TextureAtlas/Dungeon", contentManager, graphicsDevice, 16);
        mapMg.LoadMapp("Level5/level5_7_mg.csv");
        mapCollision = new LoadMap( "TextureAtlas/ALL_content", contentManager, graphicsDevice, 16);
        mapCollision.LoadMapp("Level5/level5_7_collision.csv");
        enemyManager = new EnemyManager();
        if (sceneManager.scenesStack.Count >= 6)
        {
             player._position = new Vector2(870, 45);
            // _chest = new Chest(contentManager, graphicsDevice, new Vector2(450, 50), player);
            // enemyManager.AddEnemy(_chest);
        }
        else
        {
            _fireSpirit = new FireSpirit(contentManager, graphicsDevice, new Vector2(450, 50), player);
            _arcaneArcher = new ArcaneArcher(contentManager, graphicsDevice, new Vector2(780, 378), player);
        
            enemyManager.AddEnemy(_fireSpirit);
            enemyManager.AddEnemy(_arcaneArcher);
            player._position = new Vector2(-25, 368);
        }
           
    }

    public void Update(GameTime gameTime)
    {
        
        var a = player._hitboxRect.X;
        var b = player._hitboxRect.Width;
        
        Console.WriteLine(sceneManager.scenesStack.Count);

        if (sceneManager.scenesStack.Count == 5 )
        {
            // Ограничение, чтобы игрок не смог убежать пока есть враги
            if (enemyManager.GetEnemies().Count != 0)
            {
                if (player._hitboxRect.X <= 0) player._position.X = - 25;
                if (player._hitboxRect.X > 940) sceneManager.AddScene(new Level6(contentManager, sceneManager, graphicsDevice, player));
            }
            
            else if (player._hitboxRect.X > 940)
            {
                sceneManager.AddScene(new Level6(contentManager, sceneManager, graphicsDevice, player));
            }
        }
        else
        {
            if (player._hitboxRect.X + player._hitboxRect.Width > 960) 
            {
                player._position.X = 960 - 50; 
            } 
           if (player._hitboxRect.X + player._hitboxRect.Width < 20 && player._hitboxRect.Y <= 150)
           { 
               sceneManager.AddScene(new Level8(contentManager, sceneManager, graphicsDevice, player));
           }  
        }
        
        
        mapCollision.Update(player);
        
        if (sceneManager.scenesStack.Count >= 6)
        {
            //mapCollision.Update(_chest);
        }
        else
        {
            mapCollision.Update(_arcaneArcher);
        }
        enemyManager.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        mapMg.Draw(spriteBatch);
        mapFg.Draw(spriteBatch);
        enemyManager.Draw(spriteBatch);
        //spriteBatch.Draw(texture, Vector2.Zero, Color.White);
    }
    public int LevelNumber { get; } = 5;
}
