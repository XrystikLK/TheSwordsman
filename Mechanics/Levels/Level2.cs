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
    private GoldSkeleton _goldSkeleton;
    private FireSpirit _fireSpirit;
    private ArcaneArcher _arcaneArcher;
    private Death _death;
    
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
        
        mapFg = new LoadMap("../../../Maps/Level2/level2_fg.csv", "TextureAtlas/Dungeon", contentManager, graphicsDevice, 16);
        mapFg.LoadMapp("../../../Maps/Level2/level2_fg.csv");
        mapMg = new LoadMap("../../../Maps/Level2/level2_mg.csv", "TextureAtlas/Dungeon", contentManager, graphicsDevice, 16);
        mapMg.LoadMapp("../../../Maps/Level2/level2_mg.csv");
        mapCollision = new LoadMap("../../../Maps/Level2/level2_collision.csv", "TextureAtlas/ALL_content", contentManager, graphicsDevice, 16);
        mapCollision.LoadMapp("../../../Maps/Level2/level2_collision.csv");
        
        enemyManager = new EnemyManager();
        //_skeleton = new Skeleton(contentManager, graphicsDevice, new Vector2(50, 450), player);
        //_goldSkeleton = new GoldSkeleton(contentManager, graphicsDevice, new Vector2(150, 450), player);
        _fireSpirit = new FireSpirit(contentManager, graphicsDevice, new Vector2(200, 50), player);
        _arcaneArcher = new ArcaneArcher(contentManager, graphicsDevice, new Vector2(250, 450), player);
        _death = new Death(contentManager, graphicsDevice, new Vector2(50, 450), player);
        //enemyManager.AddEnemy(_skeleton);
        //enemyManager.AddEnemy(_goldSkeleton);
        enemyManager.AddEnemy(_fireSpirit);
        enemyManager.AddEnemy(_arcaneArcher);
        enemyManager.AddEnemy(_death);
        player._position = new Vector2(450, 0);
    }

    public void Update(GameTime gameTime)
    {
        var a = player._hitboxRect.X;
        var b = player._hitboxRect.Width;
        if (player._hitboxRect.X + player._hitboxRect.Width > 900)
        {
            sceneManager.AddScene(new Level3(contentManager, sceneManager, graphicsDevice, player));
        }
        mapCollision.Update(player);
        //mapCollision.Update(_skeleton);
        //mapCollision.Update(_goldSkeleton);
        //mapCollision.Update(_fireSpirit);
        mapCollision.Update(_death);
        mapCollision.Update(_arcaneArcher);
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
