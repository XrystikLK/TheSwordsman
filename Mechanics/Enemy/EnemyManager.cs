using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;


/// <summary>
/// Менеджер для управления врагами в игре
/// Обеспечивает добавление, обновление, отрисовку и удаление врагов.
/// </summary>
public class EnemyManager
{
    private List<Enemy> enemies;

    /// <summary>
    /// Инициализирует новый экземпляр менеджера врагов
    /// </summary>
    public EnemyManager()
    {
        enemies = new List<Enemy>();
    }

    /// <summary>
    /// Добавляет врага в менеджер для управления
    /// </summary>
    /// <param name="enemy">Экземпляр врага для добавления</param>
    public void AddEnemy(Enemy enemy)
    {
        enemies.Add(enemy);
    }

    /// <summary>
    /// Обновляет состояние всех врагов
    /// </summary>
    /// <param name="gameTime">Текущее игровое время</param>
    public void Update(GameTime gameTime)
    {
        // Обновляем всех врагов
        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            enemies[i].Update(gameTime);

            // Удаляем врагов
            if (enemies[i].IsRemoved)
            {
                enemies.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// Отрисовывает всех активных врагов.
    /// </summary>
    /// <param name="spriteBatch">Контекст отрисовки спрайтов</param>
    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var enemy in enemies)
        {
            enemy.Draw(spriteBatch);
        }
    }

    /// <summary>
    /// Очищает список всех врагов
    /// </summary>
    public void Clear()
    {
        enemies.Clear();
    }

    /// <summary>
    /// Возвращает список всех активных врагов.
    /// </summary>
    /// <returns>Список экземпляров Enemy</returns>
    public List<Enemy> GetEnemies()
    {
        return enemies;
    }
}