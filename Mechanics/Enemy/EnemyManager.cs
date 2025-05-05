using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

public class EnemyManager
{
    private List<Enemy> enemies;

    public EnemyManager()
    {
        enemies = new List<Enemy>();
    }

    public void AddEnemy(Enemy enemy)
    {
        enemies.Add(enemy);
    }

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

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var enemy in enemies)
        {
            enemy.Draw(spriteBatch);
        }
    }

    public void Clear()
    {
        enemies.Clear();
    }

    public List<Enemy> GetEnemies()
    {
        return enemies;
    }
}