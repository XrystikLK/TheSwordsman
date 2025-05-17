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
        
        mapFg = new LoadMap("../../../Maps/Level5/level5_7_fg.csv", "TextureAtlas/Dungeon", contentManager, graphicsDevice, 16);
        mapFg.LoadMapp("../../../Maps/Level5/level5_7_fg.csv");
        mapMg = new LoadMap("../../../Maps/Level5/level5_7_mg.csv", "TextureAtlas/Dungeon", contentManager, graphicsDevice, 16);
        mapMg.LoadMapp("../../../Maps/Level5/level5_7_mg.csv");
        mapCollision = new LoadMap("../../../Maps/Level5/level5_7_collision.csv", "TextureAtlas/ALL_content", contentManager, graphicsDevice, 16);
        mapCollision.LoadMapp("../../../Maps/Level5/level5_7_collision.csv");
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
            _arcaneArcher = new ArcaneArcher(contentManager, graphicsDevice, new Vector2(750, 378), player);
        
            enemyManager.AddEnemy(_fireSpirit);
            enemyManager.AddEnemy(_arcaneArcher);
            player._position = new Vector2(-25, 368);
        }
           
    }

    public void Update(GameTime gameTime)
    {
        
        var a = player._hitboxRect.X;
        var b = player._hitboxRect.Width;
        if (player._hitboxRect.X + player._hitboxRect.Width > 940 && sceneManager.scenesStack.Count == 5)
        {
            sceneManager.AddScene(new Level6(contentManager, sceneManager, graphicsDevice, player));
        }

        if (player._hitboxRect.X + player._hitboxRect.Width < 20 && player._hitboxRect.Y <= 150)
        {
            sceneManager.AddScene(new Level8(contentManager, sceneManager, graphicsDevice, player));
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
}
