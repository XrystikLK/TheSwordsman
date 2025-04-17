using System;
using System.Collections.Generic;
using System.IO;
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

    public void Update(GameTime gameTime, Rectangle playerHitbox, Vector2 playerPosition)
    {
        intersections = getIntersectingTilesHorizontal(playerHitbox);

        foreach (var rect in intersections)
        {
            if (map.TryGetValue(new Vector2(rect.X, rect.Y), out int value))
            {
                
                Rectangle collision = new(rect.X * _tileSize, rect.Y * _tileSize, _tileSize, _tileSize);
                if (playerPosition.X > 0.0f)
                {
                    playerHitbox.X = collision.Left - playerHitbox.Width;
                }
                else if (playerPosition.X < 0.0f)
                {
                    playerHitbox.X = collision.Right;
                }
                Console.WriteLine(playerHitbox.X + "," + playerHitbox.Y);
            }
        }
        
        intersections = getIntersectingTilesVertical(playerHitbox);
        
        foreach (var rect in intersections)
        {
            if (map.TryGetValue(new Vector2(rect.X, rect.Y), out int value))
            {
                Rectangle collision = new(rect.X * _tileSize, rect.Y * _tileSize, _tileSize, _tileSize);
                if (playerPosition.Y > 0.0f)
                {
                    playerHitbox.Y = collision.Top - playerHitbox.Height;
                }
                else if (playerPosition.Y < 0.0f)
                {
                    playerHitbox.Y = collision.Bottom;
                }
                Console.WriteLine(playerHitbox.X + "," + playerHitbox.Y);
            }
        }
    }

    public List<Rectangle> getIntersectingTilesHorizontal(Rectangle target) {
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
    public List<Rectangle> getIntersectingTilesVertical(Rectangle target) {
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