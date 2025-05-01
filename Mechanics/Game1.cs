using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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
    private EnemyManager enemyManager;
    private Skeleton _skeleton;
    private Skeleton _skeleton1;
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        _graphics.PreferredBackBufferWidth = 960;
        _graphics.PreferredBackBufferHeight = 640;
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
        
        _playerPosition = new Vector2(100, 100);
        _player = new Player(_playerPosition, true, Content, GraphicsDevice);
        
        debugTexture = new Texture2D(GraphicsDevice, 1, 1);
        debugTexture.SetData(new[] { Color.White });
        
        _map = new LoadMap("../../../Maps/level2_face.csv", "TextureAtlas/ALL_content", Content, GraphicsDevice, 16);
        _map.LoadMapp("../../../Maps/level2_face.csv");
        _mapCollisions = new LoadMap("../../../Maps/level2_collision.csv", "TextureAtlas/ALL_content", Content, GraphicsDevice, 16);
        _mapCollisions.LoadMapp("../../../Maps/level2_collision.csv");
        
        enemyManager = new EnemyManager();
        _skeleton = new Skeleton(Content, GraphicsDevice, new Vector2(250, 170), _player);
        //_skeleton1 = new Skeleton(Content, GraphicsDevice, new Vector2(350, 370), _player);
        enemyManager.AddEnemy(_skeleton);
        //enemyManager.AddEnemy(_skeleton1);
    }

    protected override void Update(GameTime gameTime) 
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        
        _player.ProcessMovement(gameTime);
        _player.Update(gameTime);
        _mapCollisions.Update(_player);
        _mapCollisions.Update(_skeleton);
        //_mapCollisions.Update(_skeleton1);
        enemyManager.Update(gameTime);
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
        _player.Draw(_spriteBatch);
        enemyManager.Draw(_spriteBatch);
        _spriteBatch.Draw(debugTexture, _whiteSquare, Color.White);
        _map.Draw(_spriteBatch);
        _spriteBatch.End();
        
        base.Draw(gameTime);
    }
}