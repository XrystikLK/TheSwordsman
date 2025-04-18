using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace SomeTest;

public class Player
{
    private AnimatedTexture _heroIdle;
    private AnimatedTexture _heroWalkLeft;
    private AnimatedTexture _heroWalkRight;
    
    public Vector2 _position;
    public Rectangle _hitboxRect => new Rectangle((int)_position.X + 30, (int)_position.Y + 10, 37, 62);
    public bool isDebug;
    
    private string _currentDirection;
    private Texture2D _debugTexture;
    private Viewport _viewport;
    
    

    public Player(Vector2 position, bool isDebug, ContentManager content, GraphicsDevice graphicsDevice)
    {
        this._position = position;
        this.isDebug = isDebug;
        this._currentDirection = "Idle";
        _viewport = graphicsDevice.Viewport;
            
        _heroIdle = new AnimatedTexture(Vector2.Zero, 0, 2f, 0.5f);;
        _heroIdle.Load(content, "Hero/HeroIdle_SpriteList", 4, 8);
        
        _heroWalkLeft = new AnimatedTexture(Vector2.Zero, 0, 2f, 0.5f);;
        _heroWalkLeft.Load(content, "Hero/HeroRunningLeft_SpriteList", 6, 8);
        
        _heroWalkRight = new AnimatedTexture(Vector2.Zero, 0, 2f, 0.5f);;
        _heroWalkRight.Load(content, "Hero/HeroRunningRight_SpriteList", 6, 8);
        
        _debugTexture = new Texture2D(graphicsDevice, 1, 1);
        _debugTexture.SetData(new[] { Color.White });
        
    }

    public void ProcessMovement(GameTime gameTime)
    {
        KeyboardState newState = Keyboard.GetState();
        if (newState.IsKeyDown(Keys.W)) {
            _position.Y -= 3;
            _currentDirection = "Idle";
        }
        else if (newState.IsKeyDown(Keys.A)){
            _position.X -= 3;
            _currentDirection = "Left";
        }
        else if (newState.IsKeyDown(Keys.D)){
            _position.X += 3;
            _currentDirection = "Right";
        }
        else if (newState.IsKeyDown(Keys.S)){
            _position.Y += 3;
            _currentDirection = "Down";
        }
        else
        {
            _currentDirection = "Idle";
        }

        // Console.WriteLine($"HITBOX POSITION: {Hitbox}");
        // Console.WriteLine($"PLAYER POSITION: {_position}");
        if (_hitboxRect.X < 0) _position.X = -30;
        if (_hitboxRect.X + _hitboxRect.Width > _viewport.Width + 5) _position.X = _viewport.Width - 62;

    }

    public void Update(GameTime gameTime)
    {
        float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        switch (_currentDirection)
        {
            case "Left":
                _heroWalkLeft.UpdateFrame(elapsed);
                break;
            case "Right":
                _heroWalkRight.UpdateFrame(elapsed);
                break;
            default:
                _heroIdle.UpdateFrame(elapsed);
                break;
        }
    }
    
    public void Draw(SpriteBatch spriteBatch)
    {
        switch (_currentDirection)
        {
            case "Left":
                _heroWalkLeft.DrawFrame(spriteBatch, _position);
                break;
            case "Right":
                _heroWalkRight.DrawFrame(spriteBatch, _position);
                break;
            default:
                _heroIdle.DrawFrame(spriteBatch, _position);
                break;
        }
        if (isDebug) spriteBatch.Draw(_debugTexture, _hitboxRect, Color.Red * 0.5f);
        
        // Для отладки можно отрисовать хитбокс
        // spriteBatch.DrawRectangle(_hitboxRect, Color.Red);
    }
    
}
