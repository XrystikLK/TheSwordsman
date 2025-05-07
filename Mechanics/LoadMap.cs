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
        if (map == null) return;
        intersections = getIntersectingTiles(player._hitboxRect);
        var allCollisions = new List<Rectangle>();
        foreach (var rect in intersections)
        {
            if (map.TryGetValue(new Vector2(rect.X, rect.Y), out int value))
            {
                Rectangle collision = new(rect.X * _tileSize, rect.Y * _tileSize, _tileSize, _tileSize);
                allCollisions.Add(collision);
            }
        }
        var intersectionWithOX = allCollisions
            .GroupBy(v => v.Y)                     
            .Where(g => g.Count() > 1)            
            .SelectMany(g => g)                    
            .ToList();;
        var intersectionWithOY = allCollisions
            .Except(intersectionWithOX)                         
            .ToList();
        
        if (allCollisions.Count == 0) player.IsGrounded = false;
        if (allCollisions.Count == 1)
        {
            var a = player._hitboxRect.Bottom;
            var b = allCollisions.First().Top;
            if (a < b + 5 || player._hitboxRect.Top + 5 > allCollisions.First().Bottom)
            {
                intersectionWithOX = allCollisions;
                intersectionWithOY.Clear();
            }
            else
            {
                Console.WriteLine(player._hitboxRect.Bottom - allCollisions.First().Top);
            }
        }
        // Список столкновений игрока головой об текстуры
        var headCollision = intersectionWithOX
            .Where(collision => player._hitboxRect.Bottom + 5 > collision.Bottom)
            .ToList();
        
        //Console.WriteLine($"OX: {intersectionWithOX.Count}, OY: {intersectionWithOY.Count}, Head: {headCollision.Count}");
        //foreach (var collision in intersectionWithOX) {Console.WriteLine(collision);}
        // Обработка столкновения по оси OY
        if (intersectionWithOX.Count != 0)
        {
            if (headCollision.Count != 0)
            {
                player._position.Y = headCollision.First().Bottom - 11;
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
        var allCollisions = new List<Rectangle>();
        
        foreach (var rect in intersections)
        {
            if (map.TryGetValue(new Vector2(rect.X, rect.Y), out int value))
            {
                Rectangle collision = new(rect.X * _tileSize, rect.Y * _tileSize, _tileSize, _tileSize);
                allCollisions.Add(collision);
            }
        }
        var intersectionWithOX = allCollisions
            .GroupBy(v => v.Y)                     
            .Where(g => g.Count() > 1)            
            .SelectMany(g => g)                    
            .ToList();;
        var intersectionWithOY = allCollisions
            .Except(intersectionWithOX)                         
            .ToList();
        
        if (allCollisions.Count == 0) enemy.isGrounded = false;
        if (allCollisions.Count == 1)
        {
            var a = enemy.hitbox.Bottom;
            var b = allCollisions.First().Top;
            if (a < b + 5)
            {
                intersectionWithOX = allCollisions;
                intersectionWithOY.Clear();
            }
        }
        
        // Обработка столкновения по оси OY
        if (intersectionWithOX.Count != 0)
        {
            var a = intersectionWithOX.First().Y;
            var b = enemy.hitbox.Height;
            
            enemy.SetPositionY(intersectionWithOX.First().Y - enemy.hitbox.Height - 5);
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
                    enemy.SetPositionX(intersectionWithOX.First().X - enemy.hitbox.Width + 17);
                else if (enemy.velocity.X < 0)
                    enemy.SetPositionX(intersectionWithOX[0].X - _tileSize + 2);
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
        if ( map== null ) return;
        int display_tilesize = _tileSize;
        int num_tiles_per_row = 15;
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
    public void ClearMap()
    {
        // Удаляем все тайлы из словаря
        map = null;
        // Сбрасываем список пересечений
        intersections = null;
    }
}