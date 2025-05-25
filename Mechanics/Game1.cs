using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using SomeTest.Maps;

namespace SomeTest;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    
    public Player _player;
    private Vector2 _playerPosition;
    
    public Texture2D debugTexture;
    private Rectangle _whiteSquare;
    public Texture2D _healtBar;
    private Texture2D _control;
    private double testTime;
    
    private LoadMap mapFg;
    private LoadMap mapMg;
    private LoadMap mapCollision;
    private EnemyManager enemyManager;
    private Boss _boss;
    
    private List<Rectangle> intersections;

    private SpriteFont _font;
    private SceneManager sceneManager;

    public Song caveAmbient;
    public Song bossFight;

    public bool isNextScene = false;
    public bool StartMusic = false;
    private float _fadeAlpha = 0f;
    private KeyboardState _prevousKeyboardState;
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        _graphics.PreferredBackBufferWidth = 960;
        _graphics.PreferredBackBufferHeight = 640;
        sceneManager = new SceneManager();
        Window.Title = "TheSwordsman";
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        _whiteSquare = new Rectangle(200, 200, 100, 100);
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _control = Content.Load<Texture2D>("control");
        
        _font = Content.Load<SpriteFont>("Fonts/Main");
        caveAmbient = Content.Load<Song>("Audio/MapAmbiend");
        
        
        _playerPosition = new Vector2(604, 409);
        _player = new Player(_playerPosition, false, Content, GraphicsDevice);
        
        _healtBar = Content.Load<Texture2D>("healtBar");
        debugTexture = new Texture2D(GraphicsDevice, 1, 1);
        debugTexture.SetData(new[] { Color.White });
        
        mapFg = new LoadMap( "TextureAtlas/Dungeon", Content, GraphicsDevice, 18);
        mapFg.LoadMapp("Level0/level0_fg.csv");
        mapMg = new LoadMap( "TextureAtlas/Dungeon", Content, GraphicsDevice, 18);
        mapMg.LoadMapp("Level0/level0_mg.csv");
        mapCollision = new LoadMap( "TextureAtlas/ALL_content", Content, GraphicsDevice, 18);
        mapCollision.LoadMapp("Level0/level0_collision.csv");
        // enemyManager = new EnemyManager();
        // _boss = new Boss(Content, GraphicsDevice, new Vector2(50, 50), _player);
        //
        // enemyManager.AddEnemy(_boss);
        
    }

    protected override void Update(GameTime gameTime) 
    {
        if (_player._hitboxRect.X <= 0 && !isNextScene) _player._position.X = - 25;
        var _pausedGameTime = new GameTime(gameTime.TotalGameTime, TimeSpan.Zero);
        KeyboardState keyboardState  = Keyboard.GetState();
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        
        
        if (keyboardState.IsKeyDown(Keys.R) && !_prevousKeyboardState.IsKeyDown(Keys.R))
        {
            _player._velocity.Y = 1;
            MediaPlayer.Stop();
            RestartGame();
        }
        
        
        // Обработка логики, когда игрок погиб
        if (_player._dieAnimationFinished)
        {
            
            if (keyboardState.IsKeyDown(Keys.R) && !_prevousKeyboardState.IsKeyDown(Keys.R))
            {
                MediaPlayer.Stop();
                RestartGame();
            }
            base.Update(_pausedGameTime);
            return;
        }
        
        _player.ProcessMovement(gameTime);
        _player.Update(gameTime);
        //mapCollision.Update(_boss);
        //enemyManager.Update(gameTime);
        
        if (!isNextScene)
        {
            mapCollision.Update(_player);
        }
        // Переход на новый уровень
        if (_player._hitboxRect.X + _player._hitboxRect.Width > 975)
        {
            if (!isNextScene)
            {
                isNextScene = true;
                sceneManager.AddScene(new Level1(Content, sceneManager, GraphicsDevice, _player));
                mapMg.ClearMap();
                mapFg.ClearMap();
                mapCollision.ClearMap();
            }
        }

        if (keyboardState.IsKeyDown(Keys.Q))
        {
            isNextScene = true;
            sceneManager.AddScene(new Level6(Content, sceneManager, GraphicsDevice, _player));
            mapMg.ClearMap();
            mapFg.ClearMap();
            mapCollision.ClearMap();
        }

        if (isNextScene)
        {
            sceneManager.GetCurrentScene().Update(gameTime);
            Console.WriteLine(sceneManager.GetCurrentScene());
        }
        
        if (sceneManager.scenesStack.Count >= 1 && sceneManager.GetCurrentScene().LevelNumber is >= 1 and <= 8 && MediaPlayer.State != MediaState.Playing && !StartMusic)
        {
            MediaPlayer.Play(caveAmbient);
            MediaPlayer.Volume = 0.6f;
            MediaPlayer.IsRepeating = true;
            StartMusic = true;
        }
        
        base.Update(gameTime);
        _prevousKeyboardState = keyboardState;
    }
    
    /// <summary>
    /// Перезапускает игру
    /// </summary>
    public void RestartGame()
    {
        bool isLevel8 = sceneManager.GetCurrentScene()?.ToString()?.Contains("8") ?? false;
        bool isLevel9 = sceneManager.GetCurrentScene()?.ToString()?.Contains("9") ?? false;
        sceneManager.Clear();
        _player._dieAnimationFinished = false;
        _player.health = 100;
        StartMusic = false;
        Console.WriteLine(isLevel8);
        if (isLevel8 || isLevel9)
        {
            sceneManager.AddScene(new Level8(Content, sceneManager, GraphicsDevice, _player));
        }
        else sceneManager.AddScene(new Level1(Content, sceneManager, GraphicsDevice, _player));
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue * 0.7f);
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        if (isNextScene) sceneManager.GetCurrentScene().Draw(_spriteBatch);
        mapMg.Draw(_spriteBatch);
        mapFg.Draw(_spriteBatch);
        _player.Draw(_spriteBatch);
        _spriteBatch.Draw(_healtBar, new Rectangle(51, 50, 125, 20), Color.White);
        _spriteBatch.Draw(debugTexture, new Rectangle(74, 54, 100, 11), Color.Gray);
        _spriteBatch.Draw(debugTexture, new Rectangle(74, 54, _player.health, 11), Color.Red);
        if (!isNextScene) _spriteBatch.Draw(_control, new Rectangle(375, 250, 300, 100), Color.White);
        //enemyManager.Draw(_spriteBatch);
        if (_player._dieAnimationFinished)
        {
            var fadeColor = Color.Black * 0.7f;
            _spriteBatch.Draw(debugTexture, new Rectangle(0, 0, 960, 640), fadeColor);
            if (isNextScene)
            {
                if (sceneManager.GetCurrentScene().LevelNumber == 9 && ((Level9)sceneManager.GetCurrentScene())._boss.health <= 0)
                {
                    _spriteBatch.DrawString(_font, "The end", new Vector2(450, 320), Color.White);;
                }
                else 
                    {_spriteBatch.DrawString(_font, "Нажмите R что начать заново", new Vector2(300, 320), Color.Red);}
            }
        }
        _spriteBatch.End();
        
        base.Draw(gameTime);
    }
}