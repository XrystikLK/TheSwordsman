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

public class LoadMap
{
    private Dictionary<Vector2, int> map;
    private string _filePath;
    private string _texturePath;
    private Texture2D _textureAtlas;
    private int _tileSize;
    private List<Rectangle> intersections;

    private Texture2D TestTexture;
    
    public LoadMap(string filepath, string texturePath, ContentManager content,GraphicsDevice graphicsDevice,  int tileSize=16)
    {
        this._filePath = filepath;
        this._texturePath = texturePath;
        this.map = new Dictionary<Vector2, int>();
        this._tileSize = tileSize;
        
        _textureAtlas = content.Load<Texture2D>(texturePath);
        intersections = new();
        TestTexture = new Texture2D(graphicsDevice, 1, 1);
        TestTexture.SetData(new[] { Color.White });
    }

    public void LoadMapp(string _filePath)
    {
        StreamReader reader = new(_filePath);
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

    public void Update(Player player)
    {
        intersections = getIntersectingTiles(player._hitboxRect);
        var allCollisions = new List<Vector2>();
        foreach (var rect in intersections)
        {
            if (map.TryGetValue(new Vector2(rect.X, rect.Y), out int value))
            {
                Rectangle collision = new(rect.X * _tileSize, rect.Y * _tileSize, _tileSize, _tileSize);
                allCollisions.Add(new Vector2(rect.X * _tileSize, rect.Y * _tileSize));
                
                // }
            }
            
        }
        
        var uniqueCount = new HashSet<float>(allCollisions.Select(i => i.Y)).Count;
        // Нужно сделать разделение на координаты по OX OY
        var intersectionWithOX = allCollisions
            .GroupBy(v => v.Y)                     // Группируем по Y
            .Where(g => g.Count() > 1)             // Берем только группы с 2+ элементами
            .SelectMany(g => g)                     // "Разгруппировываем"
            .ToList();;
        var intersectionWithOY = allCollisions
            .Except(intersectionWithOX)                         // Исключаем элементы из sameY
            .ToList();
        
        foreach (var collision in allCollisions)
        { 
            Console.WriteLine($"OX - {intersectionWithOX.Count} OY - {intersectionWithOY.Count}");
            Console.WriteLine("-----------------------------------------------------------");
            if ((intersectionWithOX.Count >= 1 && intersectionWithOY.Count >= 1) || intersectionWithOX.Count <= 1)
            {
                player._position.Y = player._position.Y;
            }
            else
            {
                player._position.Y = collision.Y - 72;
            }
            
            // Находиться ли игрок на земле
            if (intersectionWithOX.Count == 0)
            {
                
            }
            
            if ((allCollisions.Count > 3 && uniqueCount > 1)) // || intersectionWithOX.Count == 0
            {
                player._position.X = allCollisions[0].X - player._hitboxRect.Width + _tileSize + 2;
                // Console.WriteLine($"{allCollisions[0].X} -----------------------------------------------------");
            }
        }
        //Console.WriteLine("-----------------------------------------------------------");
    
        
        
    }
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
    
    public void Draw(SpriteBatch spriteBatch)
    {
        int display_tilesize = _tileSize;
        int num_tiles_per_row = 9;
        int pixel_tilesize = 16;
        
        foreach (var item in map)
        {
            Rectangle drect = new(
                (int)item.Key.X * display_tilesize, (int)item.Key.Y * display_tilesize,
                display_tilesize, display_tilesize);
            
            int x = item.Value % num_tiles_per_row;
            int y = item.Value / num_tiles_per_row;
            Rectangle src = new(
                x * pixel_tilesize, y * pixel_tilesize,
                pixel_tilesize, pixel_tilesize);
            spriteBatch.Draw(_textureAtlas, drect, src, Color.White);
        }

        foreach (var rect in intersections)
        {
            spriteBatch.Draw(TestTexture, new Rectangle(rect.X * _tileSize, rect.Y * _tileSize, _tileSize, _tileSize), Color.Red * 0.5f);
        }
    }
}