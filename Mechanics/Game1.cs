using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
    private double testTime;
    
    private LoadMap mapFg;
    private LoadMap mapMg;
    private LoadMap mapCollision;
    
    private List<Rectangle> intersections;

    private SpriteFont _font;
    private SceneManager sceneManager;

    public bool isNextScene = false;
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        _graphics.PreferredBackBufferWidth = 960;
        _graphics.PreferredBackBufferHeight = 640;
        sceneManager = new SceneManager();
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
        
        _font = Content.Load<SpriteFont>("Fonts/Main");
        
        _playerPosition = new Vector2(604, 409);
        _player = new Player(_playerPosition, true, Content, GraphicsDevice);
        
        debugTexture = new Texture2D(GraphicsDevice, 1, 1);
        debugTexture.SetData(new[] { Color.White });
        
        mapFg = new LoadMap("../../../Maps/Level0/level0_fg.csv", "TextureAtlas/Dungeon", Content, GraphicsDevice, 16);
        mapFg.LoadMapp("../../../Maps/Level0/level0_fg.csv");
        mapMg = new LoadMap("../../../Maps/Level0/level0_mg.csv", "TextureAtlas/Dungeon", Content, GraphicsDevice, 16);
        mapMg.LoadMapp("../../../Maps/Level0/level0_mg.csv");
        mapCollision = new LoadMap("../../../Maps/Level0/level0_collision.csv", "TextureAtlas/ALL_content", Content, GraphicsDevice, 16);
        mapCollision.LoadMapp("../../../Maps/Level0/level0_collision.csv");
        
    }

    protected override void Update(GameTime gameTime) 
    {
        var _pausedGameTime = new GameTime(gameTime.TotalGameTime, TimeSpan.Zero);
        KeyboardState keyboardState  = Keyboard.GetState();
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        
        if (_player._dieAnimationFinished)
        {
            if (keyboardState.IsKeyDown(Keys.R))
            {
                RestartGame();
            }
            base.Update(_pausedGameTime);
            return;
        }
        _player.ProcessMovement(gameTime);
        _player.Update(gameTime);
        if (!isNextScene)
        {
            mapCollision.Update(_player);
        }
        
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
        if (isNextScene) sceneManager.GetCurrentScene().Update(gameTime);
        
        // TODO: Add your update logic here
        //Console.WriteLine(_player._velocity);
        
        
        // testTime += gameTime.ElapsedGameTime.TotalSeconds;
        // if (testTime >= 1.0)
        // {
        //     Console.WriteLine("Game Over");
        //     Random random = new Random();
        //     Window.Position = new Point(random.Next(100, 450), random.Next(100, 450));
        //     _graphics.ApplyChanges();
        //     testTime = 0;
        // }
        
        if (_player._hitboxRect.Intersects(_whiteSquare))
        {
            Console.WriteLine("You hit the wall");
        }
        base.Update(gameTime);
        Console.WriteLine(sceneManager.scenesStack.Count);
    }
    
    private void RestartGame()
    {
        sceneManager.Clear();
        _player._dieAnimationFinished = false;
        _player.health = 100;
        sceneManager.AddScene(new Level1(Content, sceneManager, GraphicsDevice, _player));
    }

    protected override void Draw(GameTime gameTime)
    {
        // TODO: Add your drawing code here
        Console.WriteLine(_player._position);
        GraphicsDevice.Clear(Color.CornflowerBlue);
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        if (isNextScene) sceneManager.GetCurrentScene().Draw(_spriteBatch);
        mapMg.Draw(_spriteBatch);
        mapFg.Draw(_spriteBatch);
        _player.Draw(_spriteBatch);
        _spriteBatch.DrawString(_font, "Health:" + _player.health, new Vector2(435, 50), Color.Red);
        //_spriteBatch.Draw(debugTexture, _whiteSquare, Color.White);
        
        if (_player._dieAnimationFinished)
        {
            var fadeColor = Color.Black * 0.7f;
            
            _spriteBatch.Draw(debugTexture, new Rectangle(0, 0, 960, 640), fadeColor);
            _spriteBatch.DrawString(_font, "Нажмите R что начать заново", new Vector2(300, 320), Color.Red);
        }
        _spriteBatch.End();
        base.Draw(gameTime);
    }
}