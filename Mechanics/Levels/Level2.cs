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
        _skeleton = new Skeleton(contentManager, graphicsDevice, new Vector2(50, 450), player);
        //_goldSkeleton = new GoldSkeleton(contentManager, graphicsDevice, new Vector2(150, 450), player);
        enemyManager.AddEnemy(_skeleton);
        //enemyManager.AddEnemy(_goldSkeleton);
        player._position = new Vector2(450, 0);
    }

    public void Update(GameTime gameTime)
    {
        var a = player._hitboxRect.X;
        var b = player._hitboxRect.Width;
        if (player._hitboxRect.X + player._hitboxRect.Width > 900)
        {
            //sceneManager.AddScene(new Level2(contentManager, sceneManager, player));
        }
        mapCollision.Update(player);
        mapCollision.Update(_skeleton);
        //mapCollision.Update(_goldSkeleton);
        enemyManager.Update(gameTime);
        Console.WriteLine(player._position);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        mapMg.Draw(spriteBatch);
        mapFg.Draw(spriteBatch);
        enemyManager.Draw(spriteBatch);
        //spriteBatch.Draw(texture, Vector2.Zero, Color.White);
    }
}
