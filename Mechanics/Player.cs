using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace SomeTest;

/// <summary>
/// Класс игрового персонажа с анимациями и обработкой управления
/// </summary>
public class Player
{
    private AnimatedTexture _heroIdle;
    private AnimatedTexture _heroWalking;
    private AnimatedTexture _heroJump;
    private AnimatedTexture _heroFalling;
    
    public Vector2 _position;
    public Rectangle _hitboxRect => new Rectangle((int)_position.X + 30, (int)_position.Y + 10, 37, 62);
    public bool isDebug;
    
    private string _currentDirection;
    private Texture2D _debugTexture;
    private Viewport _viewport;
    
    // Физика и движение
    public Vector2 _velocity = Vector2.Zero;
    public bool IsGrounded = false;
        
    // Константы движения
    public const float MoveSpeed = 200f;
    public const float Gravity = 800f;
    public const float JumpForce = -400f;
    public const float GroundDrag = 0.8f;
    public const float AirDrag = 0.95f;

    /// <summary>
    /// Конструктор игрового персонажа
    /// </summary>
    /// <param name="position">Начальная позиция персонажа</param>
    /// <param name="isDebug">Включить ли режим отладки</param>
    /// <param name="content">Менеджер контента для загрузки текстур</param>
    /// <param name="graphicsDevice">Графическое устройство</param>
    public Player(Vector2 position, bool isDebug, ContentManager content, GraphicsDevice graphicsDevice)
    {
        this._position = position;
        this.isDebug = isDebug;
        this._currentDirection = "Right";
        _viewport = graphicsDevice.Viewport;
            
        _heroIdle = new AnimatedTexture(Vector2.Zero, 0, 2f, 0.5f);;
        _heroIdle.Load(content, "Hero/HeroIdle_SpriteList", 4, 6);
        
        _heroWalking = new AnimatedTexture(Vector2.Zero, 0, 2f, 0.5f);;
        _heroWalking.Load(content, "Hero/HeroRunning_SpriteList", 6, 8);
        
        _heroJump = new AnimatedTexture(Vector2.Zero, 0, 2f, 0.5f);;
        _heroJump.Load(content, "Hero/HeroJumpV2_SpriteList", 7, 8);
        
        _heroFalling = new AnimatedTexture(Vector2.Zero, 0, 2f, 0.5f);
        _heroFalling.Load(content, "Hero/HeroFalling_SpriteList", 2, 4);
        
        _debugTexture = new Texture2D(graphicsDevice, 1, 1);
        _debugTexture.SetData(new[] { Color.White });
    }

    /// <summary>
    /// Обрабатывает ввод с клавиатуры и перемещает персонажа
    /// </summary>
    /// <param name="gameTime">Время игрового цикла</param>
    public void ProcessMovement(GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        KeyboardState keyboardState  = Keyboard.GetState();
        
        _velocity.X = 0;
        
        if (keyboardState.IsKeyDown(Keys.A))
        {
            _velocity.X = -MoveSpeed;
            _currentDirection = "Left";
        }
        else if (keyboardState.IsKeyDown(Keys.D))
        {
            _velocity.X = MoveSpeed;
            _currentDirection = "Right";
        }
        else if (keyboardState.IsKeyDown(Keys.S))
        {
            _position.Y += 3;
        }
        else if (keyboardState.IsKeyDown(Keys.W))
        {
            _position.Y -= 3;
        }

        // Обработка прыжка
        if (keyboardState.IsKeyDown(Keys.Space) && IsGrounded)
        {
            _velocity.Y = JumpForce;
            IsGrounded = false;
        }

        // Применяем гравитацию, если не на земле
        if (!IsGrounded)
        {
            _velocity.Y += Gravity * deltaTime;
        }
        // Сбрасываем вертикальную скорость на земле
        else if (_velocity.Y > 0)
        {
            _velocity.Y = 0; 
        }
        
        _position += _velocity * deltaTime;
        //Console.WriteLine(_velocity);
        
        // Console.WriteLine($"HITBOX POSITION: {Hitbox}");
        // Console.WriteLine($"PLAYER POSITION: {_position}");
        // Ограничение движения по горизонтали
        if (_hitboxRect.X < 0) _position.X = -30;
        if (_hitboxRect.X + _hitboxRect.Width > _viewport.Width + 5) _position.X = _viewport.Width - 62;

    }

    /// <summary>
    /// Обновляет анимацию персонажа
    /// </summary>
    /// <param name="gameTime">Время игрового цикла</param>
    public void Update(GameTime gameTime)
    {
        //Console.WriteLine(IsGrounded);
        float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (_velocity.X != 0 && IsGrounded)
            _heroWalking.UpdateFrame(elapsed);
        if (_velocity.X == 0 && IsGrounded)
            _heroIdle.UpdateFrame(elapsed);
        if (!IsGrounded)
            _heroJump.UpdateFrame(elapsed);
        if (_velocity.Y > 1)
            _heroFalling.UpdateFrame(elapsed);
        
        // switch (_currentDirection)
        // {
        //     case "Left":
        //         _heroWalkLeft.UpdateFrame(elapsed);
        //         break;
        //     case "Right":
        //         _heroWalkRight.UpdateFrame(elapsed);
        //         break;
        //     case "Jump":
        //         _heroJump.UpdateFrame(elapsed);
        //         break;
        //     default:
        //         _heroIdle.UpdateFrame(elapsed);
        //         break;
        // }
        // if (_velocity.Y > 1)
        //     _heroFalling.UpdateFrame(elapsed);
    }
    
    /// <summary>
    /// Отрисовывает персонажа и его хитбокс(при включенной debug режиме)
    /// </summary>
    /// <param name="spriteBatch">Объект для отрисовки спрайтов</param>
    public void Draw(SpriteBatch spriteBatch)
    {
        if (IsGrounded)
        {
            if (_velocity.X < 0)
                _heroWalking.DrawFrame(spriteBatch, _position, true);
            else if (_velocity.X > 0)
                _heroWalking.DrawFrame(spriteBatch, _position);
            
            if (_velocity.X == 0 && _currentDirection == "Left")
                _heroIdle.DrawFrame(spriteBatch, _position, true);
            else if(_velocity.X == 0 && _currentDirection == "Right")
                _heroIdle.DrawFrame(spriteBatch, _position);
        }

        if (_currentDirection == "Right")
        {
           if (!IsGrounded && _velocity.Y < 0)
               _heroJump.DrawFrame(spriteBatch, _position);
           else if (_velocity.Y > 1)
               _heroFalling.DrawFrame(spriteBatch, _position); 
        }
        else
        {
            if (!IsGrounded && _velocity.Y < 0)
                _heroJump.DrawFrame(spriteBatch, _position, true);
            else if (_velocity.Y > 1)
                _heroFalling.DrawFrame(spriteBatch, _position, true); 
        }
        
        // switch (_currentDirection)
        // {
        //     case "Left":
        //         _heroWalkLeft.DrawFrame(spriteBatch, _position);
        //         break;
        //     case "Right":
        //         _heroWalkRight.DrawFrame(spriteBatch, _position);
        //         break;
        //     case "Jump":
        //         _heroJump.DrawFrame(spriteBatch, _position);
        //         break;
        //     default:
        //         _heroIdle.DrawFrame(spriteBatch, _position);
        //         break;
        // }
            
        if (isDebug) spriteBatch.Draw(_debugTexture, _hitboxRect, Color.Red * 0.5f);
    }
    
}
