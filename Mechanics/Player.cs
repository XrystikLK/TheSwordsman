using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace SomeTest;

/// <summary>
/// Класс, предоставляющий игрового персонажа
/// </summary>
public class Player
{
   
    public int health = 100;
    public int damage = 25;
    
    public Vector2 _position;
    public Vector2 _velocity = Vector2.Zero;
    
    //private SoundEffect RunningAudio;
    private SoundEffect SwordSwing;
    private SoundEffect Jump;
    private SoundEffect Hurt;
    private SoundEffect Hurt2;
    private bool _playHurt1 = true;
    private SoundEffect Attack;
    private SoundEffect Attack2;
    private bool _playAttack1 = true;
    private SoundEffect Running;
    private SoundEffect Running2;
    private float _stepSoundCooldown;
    private const float StepSoundInterval = 0.3f; // Интервал между звуками шагов
    private bool _nextStepIsRunning1 = true;
    
    // Состояние персонажа
    public bool IsGrounded = false;
    public bool isAttacking = false;
    public bool isHurt = false;
    public bool isDebug;
    public bool _dieAnimationFinished = false;
    public Rectangle hitboxAttack;
    public bool _isMoving => IsGrounded && _velocity != Vector2.Zero;
    
    
    private float _attackCooldown = 0.8f;
    private float _timeSinceLastAttack = 0f;
    private bool _canAttack = true; 

    // Константы движения
    public const float MoveSpeed = 200f;
    public const float Gravity = 700f;
    
    public float JumpForce = -315f;

    // Анимации
    private AnimatedTexture _heroIdle;
    private AnimatedTexture _heroWalking;
    private AnimatedTexture _heroJump;
    private AnimatedTexture _heroFalling;
    private AnimatedTexture _heroAttack;
    private AnimatedTexture _heroHurt;
    private AnimatedTexture _heroDie;

    private string _currentDirection = "Right";
    private Texture2D _debugTexture;
    private Viewport _viewport;
    private KeyboardState _previousKeyboardState;

    public Player(Vector2 position, bool isDebug, ContentManager content, GraphicsDevice graphicsDevice)
    {
        this._position = position;
        this.isDebug = isDebug;
        this._viewport = graphicsDevice.Viewport;

        // Инициализация анимаций
        _heroIdle = new AnimatedTexture(Vector2.Zero, 0, 1.5f, 0.5f);
        _heroIdle.Load(content, "Hero/HeroIdle_SpriteList", 4, 6);

        _heroWalking = new AnimatedTexture(Vector2.Zero, 0, 1.5f, 0.5f);
        _heroWalking.Load(content, "Hero/HeroRunning_SpriteList", 6, 8);

        _heroJump = new AnimatedTexture(Vector2.Zero, 0, 1.5f, 0.5f);
        _heroJump.Load(content, "Hero/HeroJumpV2_SpriteList", 7, 8);

        _heroFalling = new AnimatedTexture(Vector2.Zero, 0, 1.5f, 0.5f);
        _heroFalling.Load(content, "Hero/HeroFalling_SpriteList", 2, 4);

        _heroAttack = new AnimatedTexture(Vector2.Zero, 0, 1.5f, 0.5f);
        _heroAttack.Load(content, "Hero/HeroAttack_SpriteList", 5, 12);

        _heroHurt = new AnimatedTexture(Vector2.Zero, 0, 1.5f, 0.5f);
        _heroHurt.Load(content, "Hero/HeroHurt_SpriteList", 3, 9);

        _heroDie = new AnimatedTexture(Vector2.Zero, 0, 1.5f, 0.5f);
        _heroDie.Load(content, "Hero/HeroDie_SpriteListpng", 7, 9);

        _debugTexture = new Texture2D(graphicsDevice, 1, 1);
        _debugTexture.SetData(new[] { Color.White });
        
        //RunningAudio = content.Load<SoundEffect>("Audio/Running");
        SwordSwing = content.Load<SoundEffect>("Audio/SwordSwing");
        Jump = content.Load<SoundEffect>("Audio/Jump");
        Hurt = content.Load<SoundEffect>("Audio/Hurt");
        Hurt2 = content.Load<SoundEffect>("Audio/Hurt2");
        Attack = content.Load<SoundEffect>("Audio/Attack");
        Attack2 = content.Load<SoundEffect>("Audio/Attack2");
        Running = content.Load<SoundEffect>("Audio/Running");
        Running2 = content.Load<SoundEffect>("Audio/Running2");
    }

    public Rectangle _hitboxRect => new Rectangle((int)_position.X + 25, (int)_position.Y + 10, 25, 45);
    public bool IsDying => health <= 0;

    
    /// <summary>
    /// Обрабатывает ввод пользователя и перемещает персонажа
    /// </summary>
    /// <param name="gameTime">Время игры для расчета движения</param>
    public void ProcessMovement(GameTime gameTime)
    {
        if (_dieAnimationFinished) return;
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        var keyboardState = Keyboard.GetState();
        
        // Обработка движения по горизонтали
        _velocity.X = 0;
        // Обработка направления движения
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

        // Обработка прыжка
        if (keyboardState.IsKeyDown(Keys.Space) && IsGrounded)
        {
            Jump.Play(0.3f, 0f, 0f);
            _velocity.Y = JumpForce;
            IsGrounded = false;
        }

        // Применение гравитации
        if (!IsGrounded)
        {
            _velocity.Y += Gravity * deltaTime;
        }
        else if (_velocity.Y > 0)
        {
            _velocity.Y = 0;
        }

        // Обновление позиции
        _position += _velocity * deltaTime;

        if (!_canAttack)
        {
            _timeSinceLastAttack += deltaTime;
            if (_timeSinceLastAttack >= _attackCooldown)
            {
                _canAttack = true;
                _timeSinceLastAttack = 0f;
            }
        }
        
        // Обработка атаки
        if (keyboardState.IsKeyDown(Keys.L) && !_previousKeyboardState.IsKeyDown(Keys.L) && !isAttacking && _canAttack && IsGrounded)
        {
            if (_playAttack1) Attack.Play(0.4f, 0f, 0f);
            else Attack2.Play(0.4f, 0f, 0f);
            _playAttack1 = !_playAttack1;
            isAttacking = true;
            _canAttack = false;
            _heroAttack.Reset();
        }

        if (isAttacking)
        {
            _heroAttack.UpdateFrame(deltaTime);
            hitboxAttack = _currentDirection == "Left"
                ? new Rectangle(_hitboxRect.X - 20, _hitboxRect.Y, 20, _hitboxRect.Height)
                : new Rectangle(_hitboxRect.X + _hitboxRect.Width, _hitboxRect.Y, 20, _hitboxRect.Height);

            if (_heroAttack.IsAnimationComplete)
            {
                isAttacking = false;
                hitboxAttack = new Rectangle(0, 0, 0, 0);
            }
        }

        _previousKeyboardState = keyboardState;
    }

    public void Update(GameTime gameTime)
    {
        if (_dieAnimationFinished) return;
        
        float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (IsDying)
        {
            _heroDie.UpdateFrame(elapsed);
            _dieAnimationFinished = _heroDie.IsAnimationComplete;
            return;
        }

        if (isHurt)
        {
            _heroHurt.UpdateFrame(elapsed);
            if (_heroHurt.IsAnimationComplete) isHurt = false;
            return;
        }

        // Обновление анимаций в зависимости от состояния
        if (!IsGrounded)
        {
            if (_velocity.Y > 1)
                _heroFalling.UpdateFrame(elapsed);
            else
                _heroJump.UpdateFrame(elapsed);
        }
        else if (_velocity.X != 0)
        {
            _heroWalking.UpdateFrame(elapsed);
        }
        else
        {
            _heroIdle.UpdateFrame(elapsed);
        }
    }
    
    /// <summary>
    /// Метод, для получения урона
    /// </summary>
    /// <param name="damage">Количество получаемого урона</param>
    public void TakeDamage(int damage)
    {
        if (_playHurt1) Hurt.Play();
        else Hurt2.Play();
        _playHurt1 = !_playHurt1;
        isHurt = true;
        health -= damage;
        _heroHurt.Reset();
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        bool flip = _currentDirection == "Left";

        if (IsDying)
        {
            _heroDie.DrawFrame(spriteBatch, _position, flip);
            return;
        }

        if (isHurt)
        {
            _heroHurt.DrawFrame(spriteBatch, _position, flip);
            return;
        }

        if (IsGrounded)
        {
            if (isAttacking)
            {
                _heroAttack.DrawFrame(spriteBatch, _position, flip);
            }
            else if (_velocity.X != 0)
            {
                _heroWalking.DrawFrame(spriteBatch, _position, flip);
            }
            else
            {
                _heroIdle.DrawFrame(spriteBatch, _position, flip);
            }
        }
        else
        {
            if (_velocity.Y > 1)
                _heroFalling.DrawFrame(spriteBatch, _position, flip);
            else
                _heroJump.DrawFrame(spriteBatch, _position, flip);
        }

        if (isDebug)
        {
            spriteBatch.Draw(_debugTexture, _hitboxRect, Color.Red * 0.5f);
            if (isAttacking)
            {
                spriteBatch.Draw(_debugTexture, hitboxAttack, Color.Green * 0.5f);
            }
        }
    }
}