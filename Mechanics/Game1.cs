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
    
    private LoadMap _map;
    private LoadMap _mapCollisions;
    
    private List<Rectangle> intersections;
    private EnemyManager enemyManager; //
    private Skeleton _skeleton; //

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
        
        _playerPosition = new Vector2(450, 100);
        _player = new Player(_playerPosition, true, Content, GraphicsDevice);
        
        debugTexture = new Texture2D(GraphicsDevice, 1, 1);
        debugTexture.SetData(new[] { Color.White });
        
        _map = new LoadMap("../../../Maps/level2_face.csv", "TextureAtlas/ALL_content", Content, GraphicsDevice, 16);
        _map.LoadMapp("../../../Maps/level2_face.csv");
        _mapCollisions = new LoadMap("../../../Maps/level2_collision.csv", "TextureAtlas/ALL_content", Content, GraphicsDevice, 16);
        _mapCollisions.LoadMapp("../../../Maps/level2_collision.csv");
        
        //enemyManager = new EnemyManager(); //
        //_skeleton = new Skeleton(Content, GraphicsDevice, new Vector2(125, 370), _player); //
        //_skeleton1 = new Skeleton(Content, GraphicsDevice, new Vector2(350, 170), _player);
        //enemyManager.AddEnemy(_skeleton); // 
        //enemyManager.AddEnemy(_skeleton1);
        
        //sceneManager.AddScene(new Level2(Content, sceneManager, GraphicsDevice, _player));
    }

    protected override void Update(GameTime gameTime) 
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        
        _player.ProcessMovement(gameTime);
        _player.Update(gameTime);
        if (!isNextScene)
        {
            _mapCollisions.Update(_player);
        }
        
        //enemyManager.Update(gameTime); // 
        if (_player._hitboxRect.X + _player._hitboxRect.Width > 975)
        {
            if (!isNextScene)
            {
                isNextScene = true;
                sceneManager.AddScene(new Level1(Content, sceneManager, GraphicsDevice, _player));
                _map.ClearMap(); // Очищаем карты
                _mapCollisions.ClearMap();
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
    }

    protected override void Draw(GameTime gameTime)
    {
        // TODO: Add your drawing code here
        GraphicsDevice.Clear(Color.CornflowerBlue);
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        if (isNextScene) sceneManager.GetCurrentScene().Draw(_spriteBatch);
        _player.Draw(_spriteBatch);
        _spriteBatch.DrawString(_font, "Health:" + _player.health, new Vector2(435, 50), Color.Red);
        //_spriteBatch.Draw(debugTexture, _whiteSquare, Color.White);
        _map?.Draw(_spriteBatch);
        _spriteBatch.End();
        
        base.Draw(gameTime);
    }
}