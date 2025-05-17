using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SomeTest;

public class Boss : Enemy
{
    private float gravity = 800f;
    private Texture2D debugTexture;
    private float _rangeAttackTimer = 0f;
    private const float _rangeAttackCoolDown = 10.0f;
    private bool _isRangeAttacking = false;
    private Texture2D _projectileTexture;
    private Vector2 _projectilePosition;
    private Rectangle _projectileHitBox;
    private bool _isProjectileActive = false;
    public Boss(ContentManager content, GraphicsDevice graphicsDevice, Vector2 startPosition, Player player)
        : base(startPosition, health: 25, damage: 10, graphicsDevice, player)
    {
        _projectileTexture = content.Load<Texture2D>("Enemy/Boss/Projectile");
        

        var attackMeleeAnimation = new AnimatedTexture(Vector2.Zero, 0f, 1.3f, 0f);
        attackMeleeAnimation.Load(content, "Enemy/Boss/AttackMelee", frameCount: 7, framesPerSec: 15);
        
        var attackRangeAnimation = new AnimatedTexture(Vector2.Zero, 0f, 1.3f, 0f);
        attackRangeAnimation.Load(content, "Enemy/Boss/AttackRange", frameCount: 9, framesPerSec: 11);
        
        var defeatOrAppearanceAnimation = new AnimatedTexture(Vector2.Zero, 0f, 1.3f, 0f);
        defeatOrAppearanceAnimation.Load(content, "Enemy/Boss/Defeat_Appearance", frameCount: 14, framesPerSec: 16);
        
        var immuneAnimation = new AnimatedTexture(Vector2.Zero, 0f, 1.3f, 0f);
        immuneAnimation.Load(content, "Enemy/Boss/Immune", frameCount: 8, framesPerSec: 10);
        
        var walkingAnimation = new AnimatedTexture(Vector2.Zero, 0f, 1.3f, 0f);
        walkingAnimation.Load(content, "Enemy/Boss/Walk", frameCount: 4, framesPerSec: 6);
        
        
        animations.Add("Attack", attackMeleeAnimation);
        animations.Add("AttackRange", attackRangeAnimation);
        animations.Add("Die", defeatOrAppearanceAnimation);
        animations.Add("Walk", walkingAnimation);
        animations.Add("Immune", immuneAnimation);
        
        debugTexture = new Texture2D(graphicsDevice, 1, 1);
        debugTexture.SetData(new[] { Color.White });
        hitbox = new Rectangle((int)position.X, (int)position.Y, 67, 95);
        velocity = new Vector2(5f, 0);
        originalVelocity = velocity;
        _previousAnimation = "Idle";
        

    }

    public override void Update(GameTime gameTime)
{
    base.Update(gameTime);
    MeleeInteractionLogic(gameTime);
    float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
    
    // Обновление физики и позиции
    Chase();
    Jumping();
    
    if (!isGrounded)
    {
        velocity.Y += gravity * deltaTime;
    }

    _previousAnimation = currentAnimation;
    position += velocity * deltaTime;
    hitbox.X = (int)(position.X) + 30;
    hitbox.Y = (int)(position.Y) + 20;
    
    // Обновление таймера атаки (только когда не атакуем)
    if (!_isRangeAttacking)
    {
        _rangeAttackTimer += deltaTime;
    }
    
    // Логика получения урона
    if (_player.hitboxAttack.Intersects(hitbox) && !isDying && _player.isAttacking)
    {
        isHurting = true;
    }

    // Обновление снаряда
    if (_isProjectileActive)
    {
        float direction = _player._position.X > position.X ? 1f : -1f; // Определяем направление к игроку
        _projectilePosition.X += direction * 450f * deltaTime;
        _projectileHitBox.X = (int)_projectilePosition.X;
        _projectileHitBox.Y = (int)_projectilePosition.Y;
        
        // Проверка выхода за пределы экрана
        if (_projectilePosition.X < 0 || _projectilePosition.X > 960)
        {
            _isProjectileActive = false;
        }
        
        // Проверка попадания в игрока
        if (_projectileHitBox.Intersects(_player._hitboxRect))
        {
            _player.TakeDamage(5); // Урон от снаряда
            _isProjectileActive = false;
        }
    }
    
    // Обработка состояний с приоритетами
    if (health <= 0)
    {
        currentAnimation = "Die";
    }
    else if (isHurting)
    {
        currentAnimation = "Hurt";
        if (animations["Hurt"].IsAnimationComplete) 
            isHurting = false;
    }
    else if (_isRangeAttacking)
    {
        currentAnimation = "AttackRange";
        if (animations["AttackRange"].IsAnimationComplete)
        {
            _isRangeAttacking = false;
            _isProjectileActive = true;
            Console.WriteLine("Анимация завершилась");
            // Здесь можно добавить логику создания снаряда
        }
    }
    else if (_rangeAttackTimer >= _rangeAttackCoolDown)
    {
        _rangeAttackTimer = 0f;
        _isRangeAttacking = true;
        currentAnimation = "AttackRange";
        _projectilePosition = new Vector2(hitbox.Right, hitbox.Center.Y);
        _projectileHitBox = new Rectangle((int)_projectilePosition.X, (int)_projectilePosition.Y, 35, 14);
        animations["AttackRange"].Reset(); // Важно сбросить анимацию!
        Console.WriteLine("STAAAAAAAAAAAAAAAAAAAART");
    }
    else if (_player._hitboxRect.Intersects(hitbox) && !isDying)
    {
        currentAnimation = "Attack";
    }
    else if (velocity.X != 0)
    {
        currentAnimation = "Walk";
    }
    else
    {
        currentAnimation = "Idle"; // Добавь это состояние, если нужно
    }
    
    //Console.WriteLine(currentAnimation); // Для отладки
}
    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        if (_isProjectileActive) spriteBatch.Draw(_projectileTexture, _projectileHitBox, Color.White);
        spriteBatch.Draw(debugTexture, hitbox, Color.Red * 0.5f);
    }
}