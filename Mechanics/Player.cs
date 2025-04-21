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
    private AnimatedTexture _heroWalkLeft;
    private AnimatedTexture _heroWalkRight;
    private AnimatedTexture _heroJump;
    
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
        this._currentDirection = "Idle";
        _viewport = graphicsDevice.Viewport;
            
        _heroIdle = new AnimatedTexture(Vector2.Zero, 0, 2f, 0.5f);;
        _heroIdle.Load(content, "Hero/HeroIdle_SpriteList", 4, 8);
        
        _heroWalkLeft = new AnimatedTexture(Vector2.Zero, 0, 2f, 0.5f);;
        _heroWalkLeft.Load(content, "Hero/HeroRunningLeft_SpriteList", 6, 8);
        
        _heroWalkRight = new AnimatedTexture(Vector2.Zero, 0, 2f, 0.5f);;
        _heroWalkRight.Load(content, "Hero/HeroRunningRight_SpriteList", 6, 8);
        
        _heroJump = new AnimatedTexture(Vector2.Zero, 0, 2f, 0.5f);;
        _heroJump.Load(content, "Hero/HeroJump_SpriteList", 4, 4);
        
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
            _currentDirection = "Jump";
        }

        // Применяем гравитацию, если не на земле
        if (!IsGrounded)
        {
            _velocity.Y += Gravity * deltaTime;
        }
        else
        {
            _velocity.Y = 0; // Сбрасываем вертикальную скорость на земле
        }
        
        _position += _velocity * deltaTime;
        //Console.WriteLine(_velocity);
        if (Math.Abs(_velocity.X) < 0.1f && _velocity.Y == 0)
        {
            _currentDirection = "Idle";
        }
        
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
        float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
        switch (_currentDirection)
        {
            case "Left":
                _heroWalkLeft.UpdateFrame(elapsed);
                break;
            case "Right":
                _heroWalkRight.UpdateFrame(elapsed);
                break;
            case "Jump":
                _heroJump.UpdateFrame(elapsed);
                break;
            default:
                _heroIdle.UpdateFrame(elapsed);
                break;
        }
    }
    
    /// <summary>
    /// Отрисовывает персонажа и его хитбокс(при включенной deBug режиме)
    /// </summary>
    /// <param name="spriteBatch">Объект для отрисовки спрайтов</param>
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
            case "Jump":
                _heroJump.DrawFrame(spriteBatch, _position);
                break;
            default:
                _heroIdle.DrawFrame(spriteBatch, _position);
                break;
        }
        if (isDebug) spriteBatch.Draw(_debugTexture, _hitboxRect, Color.Red * 0.5f);
    }
    
}
