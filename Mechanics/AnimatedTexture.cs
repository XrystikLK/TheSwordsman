using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// Класс, для загрузки анимированных текстур
/// </summary>
public class AnimatedTexture
{
    private int frameCount;
    private Texture2D myTexture;
    private float timePerFrame;
    public int frame;
    private float totalElapsed;
    private bool isPaused;
    private bool isHidden;
    public float Rotation, Scale, Depth;
    public Vector2 Origin;
    private bool _animationCompleted = false;

    /// <summary>
    /// Инициализирует новый экземпляр анимированной текстуры
    /// </summary>
    /// <param name="origin">Точка вращения и масштабирования</param>
    /// <param name="rotation">Начальный угол вращения в радианах</param>
    /// <param name="scale">Начальный масштабный коэффициент</param>
    /// <param name="depth">Начальное значение глубины (0 = передний план, 1 = задний план)</param>
    public AnimatedTexture(Vector2 origin, float rotation, float scale, float depth)
    {
        this.Origin = origin;
        this.Rotation = rotation;
        this.Scale = scale;
        this.Depth = depth;
    }

    /// <summary>
    /// Загружает текстуру и настраивает параметры анимации
    /// </summary>
    /// <param name="content">Менеджер контента для загрузки ресурсов</param>
    /// <param name="asset">Имя файла текстуры</param>
    /// <param name="frameCount">Общее количество кадров анимации</param>
    /// <param name="framesPerSec">Скорость анимации в кадрах в секунду</param>
    public void Load(ContentManager content, string asset, int frameCount, int framesPerSec)
    {
        this.frameCount = frameCount;
        myTexture = content.Load<Texture2D>(asset);
        timePerFrame = (float)1 / framesPerSec;
        frame = 0;
        totalElapsed = 0;
        isPaused = false;
    }

    /// <summary>
    /// Обновляет текущий кадр анимации на основе прошедшего времени
    /// </summary>
    /// <param name="elapsed">Время, прошедшее с последнего обновления в секундах</param>
    public void UpdateFrame(float elapsed)
    {
        if (isPaused)
            return;
        
        // Сбрасываем флаг завершения перед обновлением
        _animationCompleted = false;
    
        totalElapsed += elapsed;
        if (totalElapsed > timePerFrame)
        {
            frame++;
        
            // Проверяем, достигли ли конца анимации
            if (frame >= frameCount)
            {
                frame = 0; // Сбрасываем на первый кадр
                _animationCompleted = true; // Устанавливаем флаг завершения
            }
        
            totalElapsed -= timePerFrame;
        }
    }

    /// <summary>
    /// Отрисовывает текущий кадр анимации
    /// </summary>
    /// <param name="batch">Объект SpriteBatch для отрисовки</param>
    /// <param name="screenPos">Позиция отрисовки на экране</param>
    public void DrawFrame(SpriteBatch batch, Vector2 screenPos, bool isFlipped = false)
    {
        DrawFrame(batch, frame, screenPos, isFlipped);
    }

    /// <summary>
    /// Отрисовывает указанный кадр анимации
    /// </summary>
    /// <param name="batch">Объект SpriteBatch для отрисовки</param>
    /// <param name="frame">Номер конкретного кадра для отрисовки</param>
    /// <param name="screenPos">Позиция отрисовки на экране</param>
    public void DrawFrame(SpriteBatch batch, int frame, Vector2 screenPos, bool isFlipped = false)
    {
        if (isHidden) return;
        int frameWidth = myTexture.Width / frameCount;
        Rectangle sourceRect = new Rectangle(frameWidth * frame, 0, frameWidth, myTexture.Height);
        // Determine flip effect
        SpriteEffects effects = isFlipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

        batch.Draw(
            myTexture,
            screenPos,
            sourceRect,
            Color.White,
            Rotation,
            Origin,
            Scale,
            effects,
            Depth);
    }

    /// Получает состояние паузы анимации
    public bool IsPaused
    {
        get { return isPaused; }
    }

    /// Сбрасывает анимацию на первый кадр
    public void Reset()
    {
        frame = 0;
        totalElapsed = 0f;
    }
    /// Останавливает и сбрасывает анимацию
    public void Stop()
    {
        Pause();
        Reset();
    }
    /// Возобновляет воспроизведение анимации
    public void Play(bool loop = true)
    {
        isPaused = false;
    }
    /// Приостанавливает анимацию на текущем кадре
    public void Pause()
    {
        isPaused = true;
    }
    /// <summary>
    /// Временно скрывает анимацию (не отрисовывает)
    /// </summary>
    public void Hide()
    {
        isHidden = true;
    }

    /// <summary>
    /// Показывает анимацию (возвращает обычную отрисовку)
    /// </summary>
    public void Show()
    {
        isHidden = false;
    }
    public bool IsAnimationComplete => _animationCompleted;
}
