using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SomeTest.Maps;

/// <summary>
/// Интерфейс для игровых сцен, определяющий базовый функционал
/// </summary>
public interface IScene
{
    public void Load();
    public void Update(GameTime gameTime);
    public void Draw(SpriteBatch spriteBatch);
    public int LevelNumber { get; }
}