using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// Класс, для загрузки анимированных текстур
/// </summary>
public class AnimatedTexture
{
    private int _totalFrames;
    private Texture2D _textureAsset;
    private float _frameInterval;
    public int frame { get; private set; }
    private float _elapsedTime;
    private bool _isAnimationPaused;
    private bool _isRenderingDisabled;
    public float Rotation, Scale, Depth;
    public Vector2 Pivot;
    private bool _hasCompletedCycle = false;

    /// <summary>
    /// Инициализирует новый экземпляр анимированной текстуры
    /// </summary>
    /// <param name="origin">Точка вращения и масштабирования</param>
    /// <param name="rotation">Начальный угол вращения в радианах</param>
    /// <param name="scale">Начальный масштабный коэффициент</param>
    /// <param name="depth">Начальное значение глубины (0 = передний план, 1 = задний план)</param>
    public AnimatedTexture(Vector2 origin, float rotation, float scale, float depth)
    {
        this.Pivot = origin;
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
        this._totalFrames = frameCount;
        _textureAsset = content.Load<Texture2D>(asset);
        _frameInterval = (float)1 / framesPerSec;
        frame = 0;
        _elapsedTime = 0;
        _isAnimationPaused = false;
    }

    /// <summary>
    /// Обновляет текущий кадр анимации на основе прошедшего времени
    /// </summary>
    /// <param name="elapsed">Время, прошедшее с последнего обновления в секундах</param>
    public void UpdateFrame(float elapsed)
    {
        if (_isAnimationPaused)
            return;
        
        _hasCompletedCycle = false;
    
        _elapsedTime += elapsed;
        if (_elapsedTime > _frameInterval)
        {
            frame++;
        
            if (frame >= _totalFrames)
            {
                frame = 0;
                _hasCompletedCycle = true;
            }
        
            _elapsedTime -= _frameInterval;
        }
    }

    /// <summary>
    /// Отрисовывает текущий кадр анимации
    /// </summary>
    /// <param name="batch">Объект SpriteBatch для отрисовки</param>
    /// <param name="screenPos">Позиция отрисовки на экране</param>
    public void DrawFrame(SpriteBatch batch, Vector2 screenPos, bool isFlipped = false)
    {
        DrawSpecificFrame(batch, frame, screenPos, isFlipped);
    }

    /// <summary>
    /// Отрисовывает указанный кадр анимации
    /// </summary>
    /// <param name="batch">Объект SpriteBatch для отрисовки</param>
    /// <param name="frame">Номер конкретного кадра для отрисовки</param>
    /// <param name="screenPos">Позиция отрисовки на экране</param>
    public void DrawSpecificFrame(SpriteBatch batch, int frame, Vector2 screenPos, bool isFlipped = false)
    {
        if (_isRenderingDisabled) return;
        int singleFrameWidth = _textureAsset.Width / _totalFrames;
        Rectangle sourceArea = new Rectangle(singleFrameWidth * frame, 0, singleFrameWidth, _textureAsset.Height);
        SpriteEffects flipEffect = isFlipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

        batch.Draw(
            _textureAsset,
            screenPos,
            sourceArea,
            Color.White,
            Rotation,
            Pivot,
            Scale,
            flipEffect,
            Depth);
    }
    
    /// Сбрасывает анимацию на первый кадр
    public void Reset()
    {
        frame = 0;
        _elapsedTime = 0f;
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
        _isAnimationPaused = false;
    }

    /// Приостанавливает анимацию на текущем кадре
    public void Pause()
    {
        _isAnimationPaused = true;
    }
    

    public bool IsAnimationComplete => _hasCompletedCycle;
}