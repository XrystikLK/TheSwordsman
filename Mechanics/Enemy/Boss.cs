using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SomeTest;

public class Boss : Enemy
{
    public int displayedBossHealt = 480;
    private bool _getDamage = false;
    
    private float gravity = 800f;
    private Texture2D debugTexture;
    private float _rangeAttackTimer = 0f;
    private const float _rangeAttackCoolDown = 6.0f;
    private const float _projectileSpeed = 400f;
    private const int _projectileDamage = 10;
    private bool _isRangeAttacking = false;
    
    private Texture2D _projectileTexture;
    private Vector2 _projectilePosition;
    private Rectangle _projectileHitBox;
    private bool _isProjectileActive = false;
    public bool _isImmuneStage = false;
    public bool _isActivate = false;
    private bool _isApperanceStarting = false;
    private float _projectileDirection;
    public bool _isAlive => health <= 0;
    
    private Texture2D _laserTexture;
    private Rectangle _laserHitBox;
    private float _laserAttackCoolDown = 5.0f;
    private float _laserAttackTimer = 0f; 
    private float _laserDrawDuration = 1f;
    private bool _isLaserActive = false;
    private bool _isDeathAnimationStopped = false;
    
    public Boss(ContentManager content, GraphicsDevice graphicsDevice, Vector2 startPosition, Player player)
        : base(startPosition, health: 600, damage: 15, graphicsDevice, player)
    {
        _projectileTexture = content.Load<Texture2D>("Enemy/Boss/Projectile");
        _laserTexture = content.Load<Texture2D>("Enemy/Boss/Laser");

        var attackMeleeAnimation = new AnimatedTexture(Vector2.Zero, 0f, 1.3f, 0f);
        attackMeleeAnimation.Load(content, "Enemy/Boss/AttackMelee", frameCount: 7, framesPerSec: 9);
        
        var attackRangeAnimation = new AnimatedTexture(Vector2.Zero, 0f, 1.3f, 0f);
        attackRangeAnimation.Load(content, "Enemy/Boss/AttackRange", frameCount: 9, framesPerSec: 15);
        
        var appearanceAnimation = new AnimatedTexture(Vector2.Zero, 0f, 1.1f, 0f);
        appearanceAnimation.Load(content, "Enemy/Boss/Appearance", frameCount: 14, framesPerSec: 14);
        
        var dieAnimation = new AnimatedTexture(Vector2.Zero, 0f, 1.1f, 0f);
        dieAnimation.Load(content, "Enemy/Boss/Die", frameCount: 14, framesPerSec: 14);
        
        var hurtAnimation = new AnimatedTexture(Vector2.Zero, 0f, 1.3f, 0f);
        hurtAnimation.Load(content, "Enemy/Boss/Hurt", frameCount: 3, framesPerSec: 7);
        
        var immuneAnimation = new AnimatedTexture(Vector2.Zero, 0f, 1.3f, 0f);
        immuneAnimation.Load(content, "Enemy/Boss/Immune", frameCount: 8, framesPerSec: 10);
        
        var walkingAnimation = new AnimatedTexture(Vector2.Zero, 0f, 1.3f, 0f);
        walkingAnimation.Load(content, "Enemy/Boss/Walk", frameCount: 4, framesPerSec: 6);
        
        var laserAnimation = new AnimatedTexture(Vector2.Zero, 0f, 1.1f, 0f);
        laserAnimation.Load(content, "Enemy/Boss/LaserAttack", frameCount:7, framesPerSec: 7);
        
        
        animations.Add("Attack", attackMeleeAnimation);
        animations.Add("AttackRange", attackRangeAnimation);
        animations.Add("Die", dieAnimation);
        animations.Add("Appearance", appearanceAnimation);
        animations.Add("Walk", walkingAnimation);
        animations.Add("Immune", immuneAnimation);
        animations.Add("LoadLaser", laserAnimation);
        animations.Add("Hurt", hurtAnimation);
        
        debugTexture = new Texture2D(graphicsDevice, 1, 1);
        debugTexture.SetData(new[] { Color.White });
        hitbox = new Rectangle((int)position.X, (int)position.Y, 69, 100); // 67 95
        velocity = new Vector2(75f, 0);
        originalVelocity = velocity;
        _previousAnimation = "Idle";
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        float total = (float)gameTime.TotalGameTime.TotalSeconds; 
        // Обновление физики и позиции
        if (!isGrounded)
        {
            velocity.Y += gravity * deltaTime;
        }
        position += velocity * deltaTime;
        hitbox.X = (int)(position.X) + 30;
        hitbox.Y = (int)(position.Y) + 30;
        
        
        if (!_isActivate)
        {
            BossAwakening();
            return;
        }

        if (_isImmuneStage) return;
        Chase();
        MeleeInteractionLogic(gameTime, ref displayedBossHealt, frameGetDamage: 5, stopWhenIntersection: true);
        
        //Jumping();
        
        _previousAnimation = currentAnimation;
        
        // Обновление таймера атаки
        UpdateRangeAttackTimer(deltaTime);

        //UpdateLaserCoolDown(deltaTime);
        
        
        // Обновление снаряда
        UpdateProjectile(deltaTime);
        
        // Обработка состояний с приоритетами
        UpdateAnimationState();
    }

    private void UpdateLaserCoolDown(float deltaTime)
    {
        _laserAttackTimer += deltaTime;
        if (_laserAttackTimer >= _laserAttackCoolDown)
        {
            _laserAttackTimer = 0;
            ShootLaser();
        }
        
    }

    private void ShootLaser()
    {
        _laserHitBox = new Rectangle((int)position.X + 10, (int)position.Y + 10, 400, 20);
        _isLaserActive = true;
    }
    
    /// <summary>
    /// Обновляет анимацию появления босса
    /// </summary>
    private void BossAwakening()
    {
        // Ждем пока игрок приблизится
        if (_player._hitboxRect.Intersects(hitbox))
        {
            _isApperanceStarting = true;
        }
        if (!_isActivate)
        {
            velocity = Vector2.Zero;
            currentAnimation = "Appearance";
            if (_isApperanceStarting)
            {
                animations["Appearance"].Play();
                if (animations["Appearance"].IsAnimationComplete)
                {
                    velocity = originalVelocity;
                    _isActivate = true;
                    _isApperanceStarting = false;
                }
            }
            else animations["Appearance"].Stop();
            
        }
    }
    
    /// <summary>
    /// Логика начала неуязвимого состояния босса
    /// </summary>
    public void StartImmuneStage()
    {
        velocity = Vector2.Zero;
        if (animations["Immune"].frame == 7)
        {
            animations["Immune"].Pause();
            _isImmuneStage = true;
        }
        currentAnimation = "Immune";
    }

    /// <summary>
    /// Логика остановки неуязвимого состояния босса
    /// </summary>
    public void StopImmuneStage()
    {
        //float direction = _player._position.X > position.X ? 1f : -1f;
        velocity = originalVelocity;
        _isImmuneStage = false;
    }
    
    /// <summary>
    /// Обновляет таймер дальней атаки
    /// </summary>
    private void UpdateRangeAttackTimer(float deltaTime)
    {
        if (!_isRangeAttacking)
        {
            _rangeAttackTimer += deltaTime;
        }
        
        if (_rangeAttackTimer >= _rangeAttackCoolDown && !_isRangeAttacking)
        {
            StartRangeAttack();
        }
    }
    
    /// <summary>
    /// Начинает анимацию дальней атаки
    /// </summary>
    private void StartRangeAttack()
    {
        _rangeAttackTimer = 0f;
        _isRangeAttacking = true;
        currentAnimation = "AttackRange";
        animations["AttackRange"].Reset();
    }
    
    /// <summary>
    /// Обновляет состояние снаряда
    /// </summary>
    private void UpdateProjectile(float deltaTime)
    {
        if (!_isProjectileActive) return;
        
        // Движение снаряда
        
        _projectilePosition.X += _projectileDirection * _projectileSpeed * deltaTime;
        _projectileHitBox.X = (int)_projectilePosition.X;
        _projectileHitBox.Y = (int)_projectilePosition.Y - 25;
        
        // Проверка выхода за пределы экрана
        if (_projectilePosition.X < 0 || _projectilePosition.X > 960)
        {
            _isProjectileActive = false;
            return;
        }
        
        // Проверка попадания в игрока
        if (_projectileHitBox.Intersects(_player._hitboxRect))
        {
            _player.TakeDamage(_projectileDamage);
            _isProjectileActive = false;
        }
    }
    
    /// <summary>
    /// Создает снаряд после завершения анимации атаки
    /// </summary>
    private void CreateProjectile()
    {
        _isProjectileActive = true;
        _projectileDirection = _player._position.X > position.X ? 1f : -1f; // Вычисляем направление один раз
        _projectilePosition = new Vector2(
            _player._position.X > position.X ? hitbox.Right : hitbox.Left,
            hitbox.Center.Y);
        _projectileHitBox = new Rectangle((int)_projectilePosition.X, (int)_projectilePosition.Y, 35, 14);
    }
    
    /// <summary>
    /// Обновляет текущую анимацию в зависимости от состояния
    /// </summary>
    private void UpdateAnimationState()
    {
        if (health <= 0)
        {
            
            currentAnimation = "Die";
            return;
        }
        
        // Анимация "появления"
        if (_isImmuneStage) return;
        if (_isRangeAttacking)
        {
            currentAnimation = "AttackRange";
            if (animations["AttackRange"].IsAnimationComplete)
            {
                _isRangeAttacking = false;
                CreateProjectile();
            }
            return;
        }
        if (_player._hitboxRect.Intersects(hitbox) && !isDying)
        {
            
            currentAnimation = "Attack";
            return;
        }
        
        if (_player.hitboxAttack.Intersects(hitbox) && !isDying && _player.isAttacking)
        {
            isHurting = true;
        }
        if (isHurting)
        {
            currentAnimation = "Hurt";
            if (animations["Hurt"].IsAnimationComplete)
            {
                isHurting = false;
            }
        }
        else if (velocity.X != 0)
        {
            currentAnimation = "Walk";
            return;
        }
        
        //currentAnimation = "Idle";
        
    }
    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        if (_isLaserActive)
        {
            spriteBatch.Draw(_laserTexture, _laserHitBox, Color.White);
        }
        if (_isProjectileActive) spriteBatch.Draw(_projectileTexture, _projectileHitBox, Color.White);
        //spriteBatch.Draw(debugTexture, hitbox, Color.Red * 0.5f);
    }
}