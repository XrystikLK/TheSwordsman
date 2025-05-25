using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SomeTest;

public class ArcaneArcher
    : Enemy
{
    private float gravity = 800f;
    private Texture2D debugTexture;
    private Rectangle _arrowHitBox;
    private Vector2 _arrowPosition;
    private Texture2D _arrowTexture;
    private bool _arrowActive = false; // Флаг активности стрелы
    private float _arrowDistance = 0f; // Пройденное расстояние стрелой
    private const float MaxArrowDistance = 450f; // Максимальная дальность полета стрелы
    private const int _attackRange = 400; 
    private float _arrowDirection;
    
    public ArcaneArcher(ContentManager content, GraphicsDevice graphicsDevice, Vector2 startPosition, Player player)
        : base(startPosition, health: 50, damage: 15, graphicsDevice, player)
    {
        _arrowTexture = content.Load<Texture2D>("Enemy/ArcaneArcher/Arrow");
        
        var idleAnimation = new AnimatedTexture(Vector2.Zero, 0f, 1.2f, 0f);
        idleAnimation.Load(content, "Enemy/ArcaneArcher/Idle", frameCount: 4, framesPerSec: 9);
        
        var attackAnimation = new AnimatedTexture(Vector2.Zero, 0f, 1.2f, 0f);
        attackAnimation.Load(content, "Enemy/ArcaneArcher/Attack", frameCount: 7, framesPerSec: 6);
        
        var dieAnimation = new AnimatedTexture(Vector2.Zero, 0f, 1.2f, 0f);
        dieAnimation.Load(content, "Enemy/ArcaneArcher/Die", frameCount: 8, framesPerSec: 10);
        
        var walkingAnimation = new AnimatedTexture(Vector2.Zero, 0f, 1.2f, 0f);
        walkingAnimation.Load(content, "Enemy/ArcaneArcher/Walk", frameCount: 8, framesPerSec: 12);
        
        var hurtAnimation = new AnimatedTexture(Vector2.Zero, 0f, 1.2f, 0f);
        hurtAnimation.Load(content, "Enemy/ArcaneArcher/Hurt", frameCount: 3, framesPerSec: 5);
        //
        animations.Add("Idle", idleAnimation);
        animations.Add("Attack", attackAnimation);
        animations.Add("Die", dieAnimation);
        animations.Add("Walk", walkingAnimation);
        animations.Add("Hurt", hurtAnimation);


        _arrowPosition = new Vector2(hitbox.Right, hitbox.Center.Y);
        _arrowHitBox = new Rectangle(hitbox.Right, hitbox.Center.Y, 37, 5);

        debugTexture = new Texture2D(graphicsDevice, 1, 1);
        debugTexture.SetData(new[] { Color.White });
        hitbox = new Rectangle((int)position.X, (int)position.Y, 40, 58);
        
        velocity = new Vector2(100f, 0);
        originalVelocity = velocity;
        _previousAnimation = "Idle";
    }

    public override void Update(GameTime gameTime)
    {
        //Console.WriteLine(_arrowHitBox);
        base.Update(gameTime);
        var distanceToPlayer = Vector2.Distance(_player._position, position);
        float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
        float total = (float)gameTime.TotalGameTime.TotalSeconds; 
        
        // Обновление позиции и состояния стрелы
        if (_arrowActive)
        {
            _arrowPosition.X += _arrowDirection * 600f * elapsed; // Увеличиваем скорость стрелы
            _arrowDistance += 600f * elapsed;
            _arrowHitBox.X = (int)_arrowPosition.X;
            _arrowHitBox.Y = (int)_arrowPosition.Y;
            
            // Проверка попадания в игрока
            if (_arrowHitBox.Intersects(_player._hitboxRect))
            {
                if (total - _lastDamageTimeHero >= DamageCooldown)
                {
                    _lastDamageTimeHero = total;
                    _player.TakeDamage(damage);
                }
                _arrowActive = false; // Стрела исчезает после попадания
            }
            // Стрела исчезает после пролета максимального расстояния
            else if (_arrowDistance >= MaxArrowDistance)
            {
                _arrowActive = false;
            }
        }

        // Логика атаки
        if (animations["Attack"].IsAnimationComplete && !_arrowActive && currentAnimation == "Attack")
        {
            // Создаем новую стрелу
            _arrowDirection = playerIsRight ? 1f : -1f;
            _arrowActive = true;
            _arrowDistance = 0f;
            _arrowPosition = new Vector2(hitbox.Right, hitbox.Center.Y);
            _arrowHitBox = new Rectangle((int)_arrowPosition.X, (int)_arrowPosition.Y, 37, 5);
        }
        Chase();
        if ((distanceToPlayer <= _attackRange || _player.hitboxAttack.Intersects(hitbox) || isHurting))
        {
            velocity.X = 0;
            // 4) Логика нанесения урона монстру
            if (_player.hitboxAttack.Intersects(hitbox)) 
            {
                // Возможно сделать систему отталкивания, но пока так
                if (total - _lastDamageTimeEnemy >= DamageCooldown)
                {
                    //Console.WriteLine("Enemy get damage ");
                    _lastDamageTimeEnemy = total;
                    TakeDamage(_player.damage);
                }
            }
            // 3) Логика столкновений и урона
            if (_player._hitboxRect.Intersects(_arrowHitBox))
            {
                if (total - _lastDamageTimeHero >= DamageCooldown)
                {
                    _lastDamageTimeHero = total;
                    _player.TakeDamage(damage);
                    //Нужно сделать чтобы игрока отталкивало назад при столкновении
                    Console.WriteLine("Enemy hit player");
                }
            
            }
        }
        //else velocity.X = originalVelocity.X;
        
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        //Jumping();
        // Применяем гравитацию, если не на земле
        if (!isGrounded)
        {
            velocity.Y += gravity * deltaTime;
        }

        _previousAnimation = currentAnimation;
        position += velocity * deltaTime;
        hitbox.X = (int)(position.X) + 25;
        hitbox.Y = (int)(position.Y);
        
        if (_player.hitboxAttack.Intersects(hitbox) && !isDying && _player.isAttacking)
        {
            isHurting = true;
        }
        if (isHurting)
        {
            currentAnimation = "Hurt";
            if (animations["Hurt"].IsAnimationComplete) isHurting = false;
        }
        else if (Math.Abs(distanceToPlayer) <= _attackRange)
        {
            currentAnimation = "Attack";
        }
        
        else if (velocity.X !=  0) currentAnimation = "Walk";
        
        if (health <= 0){
            currentAnimation = "Die";
            //gravity = 0;
        };
    }
    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        if (_arrowActive)
        {
            spriteBatch.Draw(
                _arrowTexture,
                _arrowHitBox,
                null,
                Color.White,
                0f,
                Vector2.Zero,
                playerIsRight ?  SpriteEffects.None:SpriteEffects.FlipHorizontally,
                0f
            );
        }
        //spriteBatch.Draw(debugTexture, hitbox, Color.Red * 0.5f);
    }
}