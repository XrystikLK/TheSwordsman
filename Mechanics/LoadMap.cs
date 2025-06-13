using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace SomeTest;

/// <summary>
/// Класс для загрузки и обработки тайловых карт
/// </summary>
public class LoadMap
{
    private Dictionary<Vector2, int> map; // Словарь для хранения тайлов карты
    private string _filePath; 
    private string _texturePath; // Путь к текстуре тайлов
    private Texture2D _textureAtlas;// Атлас текстур тайлов
    private int _tileSize;
    private List<Rectangle> intersections; // Список пересекающихся тайлов

    private Texture2D TestTexture;
    private readonly ContentManager content;

    /// <summary>
    /// Конструктор класса LoadMap
    /// </summary>
    /// <param name="filepath">Путь к файлу карты</param>
    /// <param name="texturePath">Путь к текстуре тайлов</param>
    /// <param name="content">Менеджер контента</param>
    /// <param name="graphicsDevice">Графическое устройство</param>
    /// <param name="tileSize">Размер тайла (по умолчанию 16)</param>
    public LoadMap(string texturePath, ContentManager content, GraphicsDevice graphicsDevice,  int tileSize=16)
    {
        this._texturePath = texturePath;
        this.map = new Dictionary<Vector2, int>();
        this._tileSize = tileSize;
        this.content = content;
        
        _textureAtlas = content.Load<Texture2D>(texturePath);
        intersections = new();
        TestTexture = new Texture2D(graphicsDevice, 1, 1);
        TestTexture.SetData(new[] { Color.White });
    }

    /// <summary>
    /// Загружает карту из файла
    /// </summary>
    /// <param name="_filePath">Путь к файлу карты</param>
    public void LoadMapp(string _filePath)
    {
        StreamReader reader = new(Path.Combine(content.RootDirectory, "Maps", _filePath));
        int y = 0;
        
        string line;
        while ((line = reader.ReadLine()) != null)
        {
            string[] items = line.Split(',');
            for (int x = 0; x < items.Length; x++)
            {
                if (int.TryParse(items[x], out int value) && value > -1)
                {
                    map[new Vector2(x, y)] = value;
                }
            }

            y++;
        }
    }
    /// <summary>
    /// Обновляет столкновения игрока с картой
    /// </summary>
    /// <param name="player">Игрок для проверки столкновений</param>
    public void Update(Player player)
    {
        if (map == null) return;
        intersections = getIntersectingTiles(player._hitboxRect);
        var allCollisions = ConvertTilesToWorldRectangles(intersections);
        var (intersectionWithOX, intersectionWithOY) = SplitCollisionsByType(allCollisions);

        
        if (allCollisions.Count == 0) player.IsGrounded = false;
        if (allCollisions.Count == 1)
        {
            if (player._hitboxRect.Bottom < allCollisions.First().Top + 5 || player._hitboxRect.Top + 5 > allCollisions.First().Bottom)
            {
                intersectionWithOX = allCollisions;
                intersectionWithOY.Clear();
            }
        }
        // Список столкновений игрока головой об текстуры
        var headCollision = intersectionWithOX
            .Where(collision => player._hitboxRect.Bottom + 5 > collision.Bottom)
            .ToList();
        // Обработка столкновения по оси OY
        if (intersectionWithOX.Count != 0)
        {
            if (headCollision.Count != 0)
            {
                player._position.Y = headCollision.First().Bottom - 11;
                player._velocity.Y = Math.Max(1f, player._velocity.Y);
            }
            else
            {
              player._position.Y = intersectionWithOX.First().Y - player._hitboxRect.Height - 10;
              player.IsGrounded = true;  
            }
            
        }
        //Иначе игрок должен падать 
        else
            player.IsGrounded = false;
        
        // Обработка столкновения по оси OX
        if (intersectionWithOY.Count >= 1)
        {
            if (player.IsGrounded)
            {
                if (player._velocity.X > 0)
                    player._position.X = intersectionWithOX.First().X - player._hitboxRect.Width + 7;
                else if (player._velocity.X < 0)
                    player._position.X = intersectionWithOX[0].X - _tileSize + 7;
            }
            // Обрабатывает столкновение игрока в воздухе
            else
            {
                if (player._velocity.X > 0)
                    player._position.X = intersectionWithOY.First().X - (player._hitboxRect.Width + 25);
                else if (player._velocity.X < 0)
                    player._position.X = intersectionWithOY[0].X - _tileSize + 7;
            }
        }
                
    }
    
    public void Update(Enemy enemy)
    {
        if (map == null) return;
        intersections = getIntersectingTiles(enemy.hitbox);
        var allCollisions = ConvertTilesToWorldRectangles(intersections);
        var (intersectionWithOX, intersectionWithOY) = SplitCollisionsByType(allCollisions);
        
        if (allCollisions.Count == 0) enemy.isGrounded = false;
        
        // Если 1 коллизия, то он считает, что столкновение происходит с осью OY
        // Здесь это исправляется
        if (allCollisions.Count == 1)
        {
            if (enemy.hitbox.Bottom < allCollisions.First().Top + 5 || enemy.hitbox.Top + 5 > allCollisions.First().Bottom)
            {
                intersectionWithOX = allCollisions;
                intersectionWithOY.Clear();
            }
        }

        //Console.WriteLine($"{intersectionWithOX.Count} {intersectionWithOY.Count}");
        // Обработка столкновения по оси OY
        if (intersectionWithOX.Count != 0)
        {
            var a = intersectionWithOX.First().Y;
            var b = enemy.hitbox.Height;
            
            enemy.SetPositionY(intersectionWithOX.First().Y - enemy.hitbox.Height - 3);
            enemy.isGrounded = true;
        }
        //Иначе игрок должен падать 
        else
            enemy.isGrounded = false;
        // Обработка столкновения по оси OX
        if (intersectionWithOY.Count >= 1)
        {
            enemy.isStack = true;
            if (enemy.isGrounded)
            {
                if (enemy.velocity.X > 0)
                    enemy.SetPositionX(intersectionWithOX.First().X - _tileSize - 4);
                else if (enemy.velocity.X < 0)
                    enemy.SetPositionX(intersectionWithOX[0].X - _tileSize - 2);
            }
            // Обрабатывает столкновение игрока в воздухе
            else
            {
                if (enemy.velocity.X > 0)
                    enemy.SetPositionX(intersectionWithOY.First().X - (enemy.hitbox.Width + 30));
                else if (enemy.velocity.X < 0)
                    enemy.SetPositionX(intersectionWithOY[0].X - _tileSize + 2);
            }
        }
        else enemy.isStack = false;
    }
    
    /// <summary>
    /// Разделяет столкновения на горизонтальные и вертикальные
    /// </summary>
    private (List<Rectangle> ground, List<Rectangle> walls) SplitCollisionsByType(List<Rectangle> collisions)
    {
        // Горизонтальные (земля/потолок) - тайлы на одной линии Y
        var ground = collisions
            .GroupBy(v => v.Y)
            .Where(g => g.Count() > 1)
            .SelectMany(g => g)
            .ToList();
    
        // Вертикальные (стены) - оставшиеся тайлы
        var walls = collisions.Except(ground).ToList();
    
        return (ground, walls);
    }
    
    /// <summary>
    /// Преобразует координаты тайлов в мировые координаты прямоугольников
    /// </summary>
    private List<Rectangle> ConvertTilesToWorldRectangles(List<Rectangle> tileRects)
    {
        var result = new List<Rectangle>();
        foreach (var rect in tileRects)
        {
            if (map.TryGetValue(new Vector2(rect.X, rect.Y), out _))
            {
                result.Add(new Rectangle(rect.X * _tileSize, rect.Y * _tileSize, _tileSize, _tileSize));
            }
        }
        return result;
    }
    
    /// <summary>
    /// Находит все тайлы, пересекающиеся с целевым прямоугольником
    /// </summary>
    /// <param name="target">Прямоугольник для проверки</param>
    /// <returns>Список пересекающихся тайлов</returns>
    public List<Rectangle> getIntersectingTiles(Rectangle target) {
        intersections.Clear();
        
        int startX = target.X / _tileSize;
        int endX = (target.X + target.Width) / _tileSize;
        int startY = target.Y / _tileSize;
        int endY = (target.Y + target.Height) / _tileSize;

        for (int x = startX; x <= endX; x++) {
            for (int y = startY; y <= endY; y++) {
                if (map.ContainsKey(new Vector2(x, y))) {
                    intersections.Add(new Rectangle(x, y, _tileSize, _tileSize));
                }
            }
        }

        return intersections;
    }
    
    /// <summary>
    /// Отрисовывает карту и отладочную информацию
    /// </summary>
    /// <param name="spriteBatch">Объект для отрисовки</param>
    public void Draw(SpriteBatch spriteBatch)
    {
        if (map == null) return;
    
        int RenderedTileSize = _tileSize;
        const int TextureTilesPerLine = 15;
        const int SourceTileDimension = 16;
    
        // Визуализация основного слоя карты
        foreach (KeyValuePair<Vector2, int> tile in map)
        {
            var destinationRect = new Rectangle(
                (int)tile.Key.X * RenderedTileSize,
                (int)tile.Key.Y * RenderedTileSize,
                RenderedTileSize,
                RenderedTileSize);
        
            int textureX = tile.Value % TextureTilesPerLine;
            int textureY = tile.Value / TextureTilesPerLine;
        
            var sourceRect = new Rectangle(
                textureX * SourceTileDimension,
                textureY * SourceTileDimension,
                SourceTileDimension,
                SourceTileDimension);
            
            spriteBatch.Draw(_textureAtlas, destinationRect, sourceRect, Color.White);
        }
    
        // Визуализация (для отладки)
        foreach (Rectangle collisionArea in intersections)
        {
            var debugRect = new Rectangle(
                collisionArea.X * _tileSize,
                collisionArea.Y * _tileSize,
                _tileSize,
                _tileSize);
            
            spriteBatch.Draw(TestTexture, debugRect, Color.Red * 0.5f);
        }
    }
    /// <summary>
    /// Очищает карту
    /// </summary>
    public void ClearMap()
    {
        map = null;
        intersections = null;
    }
}