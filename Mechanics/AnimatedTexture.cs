using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// A helper class for handling animated textures.
/// </summary>
public class AnimatedTexture
{
    private int frameCount;
    private Texture2D myTexture;
    private float timePerFrame;
    private int frame;
    private float totalElapsed;
    private bool isPaused;
    public float Rotation, Scale, Depth;
    public Vector2 Origin;

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
        totalElapsed += elapsed;
        if (totalElapsed > timePerFrame)
        {
            frame++;
            frame %= frameCount;
            totalElapsed -= timePerFrame;
        }
    }

    /// <summary>
    /// Отрисовывает текущий кадр анимации
    /// </summary>
    /// <param name="batch">Объект SpriteBatch для отрисовки</param>
    /// <param name="screenPos">Позиция отрисовки на экране</param>
    public void DrawFrame(SpriteBatch batch, Vector2 screenPos)
    {
        DrawFrame(batch, frame, screenPos);
    }

    /// <summary>
    /// Отрисовывает указанный кадр анимации
    /// </summary>
    /// <param name="batch">Объект SpriteBatch для отрисовки</param>
    /// <param name="frame">Номер конкретного кадра для отрисовки</param>
    /// <param name="screenPos">Позиция отрисовки на экране</param>
    public void DrawFrame(SpriteBatch batch, int frame, Vector2 screenPos)
    {
        int FrameWidth = myTexture.Width / frameCount;
        Rectangle sourcerect = new Rectangle(FrameWidth * frame, 0,
            FrameWidth, myTexture.Height);
        batch.Draw(myTexture, screenPos, sourcerect, Color.White,
            Rotation, Origin, Scale, SpriteEffects.None, Depth);
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
    public void Play()
    {
        isPaused = false;
    }
    /// Приостанавливает анимацию на текущем кадре
    public void Pause()
    {
        isPaused = true;
    }
}
