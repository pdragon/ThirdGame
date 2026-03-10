using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Client.Assets
{
    public class Manager
    {
        public Manager() {
            
        }
        public static void Load(string[] args)
        {
        }

        public static Texture2D CreateColorTexture(Color color, int width, int height)
        {
            Texture2D texture = new Texture2D(Config.GraphicsDevice, width, height);
            Color[] data = new Color[width * height];

            for (int i = 0; i < data.Length; i++)
                data[i] = color;

            texture.SetData(data);
            return texture;
        }
    }
}
