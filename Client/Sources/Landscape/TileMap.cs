using System;
using System.IO;
using Client.Assets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Vector2 = System.Numerics.Vector2;

namespace Client.Landscape
{
    public class TileMap
    {
        public Dictionary<Vector2,Tile> Entities;
        public static readonly Lazy<Dictionary<TextureType, Texture2D>> TestTextures =
        new Lazy<Dictionary<TextureType, Texture2D>>(() => new Dictionary<TextureType, Texture2D>
        {
            [TextureType.Grass] = Manager.CreateColorTexture(Color.Green, 64, 32),
            [TextureType.Mountain] = Manager.CreateColorTexture(Color.Gray, 64, 32),
            [TextureType.Water] = Manager.CreateColorTexture(Color.Blue, 64, 32),
            //[TextureType.Player] = Manager.CreateColorTexture(Color.Red, 32, 48)
        });


        public void LoadContent()
        {
            //Entity = new Tile[20, 20] { };
            Entities = LoadMap($"./level1.tmap.txt");

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Tile Entity in Entities.Values)
            {
                spriteBatch.Draw(Entity.Texture,
                    new Rectangle((int)Entity.Position.X * (int)Config.CellSize.X, (int)Entity.Position.Y * (int)Config.CellSize.Y,
                        (int)Config.CellSize.X, (int)Config.CellSize.Y),
                    Color.White);
            } 
        }

        public Dictionary<Vector2, Tile> LoadMap(string filePath)
        {
            // Читаем все строки файла
            string[] lines = File.ReadAllLines(filePath);

            if (lines.Length == 0)
                throw new InvalidOperationException("Файл пуст.");

            int height = lines.Length;
            int width = lines[0].Length;

            Dictionary<Vector2, Tile> map = new Dictionary<Vector2, Tile>(); 
            for (int y = 0; y < height; y++)
            {
                string currentLine = lines[y];
                // Проверяем, что длина строки соответствует ожидаемой
                if (currentLine.Length != width)
                    throw new InvalidOperationException(
                        $"Строка {y + 1} имеет длину {currentLine.Length}, " +
                        $"ожидалось {width}. Файл должен быть прямоугольным.");

                for (int x = 0; x < width; x++)
                {
                    char c = currentLine[x];

                    // Проверяем, что символ является цифрой
                    if (c < '0' || c > '9')
                        throw new InvalidOperationException(
                            $"Недопустимый символ '{c}' в строке {y + 1}, позиция {x + 1}.");

                    // Преобразуем символ в число
                    //map[y, x] = c - '0';
                    Tile tile = new Tile();
                    tile.Position = new Vector2(y, x);
                    tile.Texture = TestTextures.Value[(TextureType)(c - '0')];
                    map[new Vector2(y, x)] = tile;
                }
            }
            return map;
        }
    }
}
