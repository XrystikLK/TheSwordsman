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
    private Skeleton _skeleton;
    private GoldSkeleton _goldSkeleton;
    private FireSpirit _fireSpirit;
    private ArcaneArcher _arcaneArcher;
    private Death _death;
    
    private LoadMap mapFg;
    private LoadMap mapMg;
    private LoadMap mapCollision;
    private Texture2D texture;
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
        
        mapFg = new LoadMap("../../../Maps/Level8/level8_fg.csv", "TextureAtlas/Dungeon", contentManager, graphicsDevice, 16);
        mapFg.LoadMapp("../../../Maps/Level8/level8_fg.csv");
        mapMg = new LoadMap("../../../Maps/Level8/level8_mg.csv", "TextureAtlas/Dungeon", contentManager, graphicsDevice, 16);
        mapMg.LoadMapp("../../../Maps/Level8/level8_mg.csv");
        mapCollision = new LoadMap("../../../Maps/Level8/level8_collision.csv", "TextureAtlas/ALL_content", contentManager, graphicsDevice, 16);
        mapCollision.LoadMapp("../../../Maps/Level8/level8_collision.csv");
        
        player._position = new Vector2(925, 45);
    }

    public void Update(GameTime gameTime)
    {
        var a = player._hitboxRect.X;
        var b = player._hitboxRect.Width;
        if (player._hitboxRect.Y > 970)
        {
            sceneManager.AddScene(new Level5(contentManager, sceneManager, graphicsDevice, player));
        }
        mapCollision.Update(player);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        mapMg.Draw(spriteBatch);
        mapFg.Draw(spriteBatch);
        //spriteBatch.Draw(texture, Vector2.Zero, Color.White);
    }
}
