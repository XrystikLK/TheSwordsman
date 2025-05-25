using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SomeTest.Maps;

/// <summary>
/// Класс, реализующий первый уровень
/// </summary>
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
    
    
    /// <summary>
    /// Конструктор уровня
    /// </summary>
    /// <param name="contentManager">Менеджер загрузки контента</param>
    /// <param name="sceneManager">Менеджер управления сценами</param>
    /// <param name="graphicsDevice">Графическое устройство</param>
    /// <param name="player">Игровой персонаж</param>
    public Level1(ContentManager contentManager, SceneManager sceneManager, GraphicsDevice graphicsDevice, Player player)
    {
        this.player = player;
        this.sceneManager = sceneManager;
        this.contentManager = contentManager;
        this.graphicsDevice = graphicsDevice;
        
    }

    /// <summary>
    /// Загружает ресурсы уровня (текстуры, карты, врагов)
    /// </summary>
    public void Load()
    {
        texture = contentManager.Load<Texture2D>("TextureAtlas/All_content");
        
        mapFg = new LoadMap( "TextureAtlas/Dungeon", contentManager, graphicsDevice, 16);
        mapFg.LoadMapp("Level1/level1_fg.csv");
        mapMg = new LoadMap( "TextureAtlas/Dungeon", contentManager, graphicsDevice, 16);
        mapMg.LoadMapp("Level1/level1_mg.csv");
        mapCollision = new LoadMap( "TextureAtlas/ALL_content", contentManager, graphicsDevice, 16);
        mapCollision.LoadMapp("Level1/level1_collision.csv");
        
        enemyManager = new EnemyManager(); 
        player._position = new Vector2(450, 0);
        
    }
    /// <summary>
    /// Обновляет состояние уровня каждый кадр
    /// </summary>
    /// <param name="gameTime">Время игры для синхронизации</param>
    
    public void Update(GameTime gameTime)
    {
        var a = player._hitboxRect.X;
        var b = player._hitboxRect.Width;
        if (player._hitboxRect.Y + player._hitboxRect.Height > 700)
        {
            sceneManager.AddScene(new Level2(contentManager, sceneManager, graphicsDevice, player));
        }
        mapCollision.Update(player);
    }
    
    /// <summary>
    /// Отрисовывает уровень
    /// </summary>
    /// <param name="spriteBatch">Контекст отрисовки</param>
    public void Draw(SpriteBatch spriteBatch)
    {
        mapMg.Draw(spriteBatch);
        mapFg.Draw(spriteBatch);
        //enemyManager.Draw(spriteBatch);
        //spriteBatch.Draw(texture, Vector2.Zero, Color.White);
    }

    public int LevelNumber { get; } = 1;
}
