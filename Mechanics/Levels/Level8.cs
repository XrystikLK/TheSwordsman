using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SomeTest.Maps;

public class Level8 : IScene
{
    private ContentManager contentManager;
    private SceneManager sceneManager;
    private GraphicsDevice graphicsDevice;
    private Player player;
    
    private EnemyManager enemyManager;
    private Chest _chest;
    private FireSpirit _fireSpirit;

    private SpriteFont _font;
    private LoadMap mapFg;
    private LoadMap mapMg;
    private LoadMap mapCollision;
    private Texture2D texture;
    private Texture2D debugTexture;
    public Level8(ContentManager contentManager, SceneManager sceneManager, GraphicsDevice graphicsDevice, Player player)
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
        mapFg.LoadMapp("Level8/level8_fg.csv");
        mapMg = new LoadMap( "TextureAtlas/Dungeon", contentManager, graphicsDevice, 16);
        mapMg.LoadMapp("Level8/level8_mg.csv");
        mapCollision = new LoadMap( "TextureAtlas/ALL_content", contentManager, graphicsDevice, 16);
        mapCollision.LoadMapp("Level8/level8_collision.csv");
        
        debugTexture = new Texture2D(graphicsDevice, 1, 1);
        debugTexture.SetData(new[] { Color.White });
        _font = contentManager.Load<SpriteFont>("Fonts/Main");
        
        enemyManager = new EnemyManager();
        
        _fireSpirit = new FireSpirit(contentManager, graphicsDevice, new Vector2(435, 550), player);
        _chest = new Chest(contentManager, graphicsDevice, new Vector2(535, 50), player);
        
        enemyManager.AddEnemy(_fireSpirit);
        enemyManager.AddEnemy(_chest);
        
        player._position = new Vector2(925, 45);
    }

    public void Update(GameTime gameTime)
    {
        var a = player._hitboxRect.X;
        var b = player._hitboxRect.Width;
        if (player._hitboxRect.X + player._hitboxRect.Width > 960)
        {
            player._position.X = 960 - 50;
        }
        if (player._hitboxRect.Y > 970)
        {
            sceneManager.AddScene(new Level9(contentManager, sceneManager, graphicsDevice, player));
        }
        mapCollision.Update(player);
        mapCollision.Update(_chest);
        enemyManager.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        mapMg.Draw(spriteBatch);
        mapFg.Draw(spriteBatch);
        enemyManager.Draw(spriteBatch);
    }
    public int LevelNumber { get; } = 8;
}
