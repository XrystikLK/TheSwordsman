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
    
    private Player _player;
    private Vector2 _playerPosition;
    
    private Texture2D testTexture;
    private Rectangle _whiteSquare;
    
    private LoadMap _map;
    private LoadMap _mapCollisions;
    
    private List<Rectangle> intersections;
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
        
        testTexture = new Texture2D(GraphicsDevice, 1, 1);
        testTexture.SetData(new[] { Color.White });
        
        _map = new LoadMap("../../../Maps/level2_face.csv", "TextureAtlas/ALL_content", Content, GraphicsDevice, 16);
        _map.LoadMapp("../../../Maps/level2_face.csv");
        _mapCollisions = new LoadMap("../../../Maps/level2_collision.csv", "TextureAtlas/ALL_content", Content, GraphicsDevice, 16);
        _mapCollisions.LoadMapp("../../../Maps/level2_collision.csv");
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        
        _player.ProcessMovement(gameTime);
        _player.Update(gameTime);
        
        _mapCollisions.Update(_player);
        // TODO: Add your update logic here
        // Console.WriteLine(_player._hitboxRect.Y - _player._hitboxRect.Height + 16);
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
        _spriteBatch.Draw(testTexture, _whiteSquare, Color.White);
        _map.Draw(_spriteBatch);
        _spriteBatch.End();
        
        base.Draw(gameTime);
    }
}