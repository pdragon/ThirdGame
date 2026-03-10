using Microsoft.Xna.Framework.Graphics;
using System;
using Vector2 = System.Numerics.Vector2;

namespace Client
{
    
    public enum TextureType
    {
        None,
        Grass,
        Mountain,
        Water,
        Player
    }
    public static class Config
    {
        const short CellSizeConst = 64;
        public static readonly Vector2 MapSize = new Vector2(60,60);
        public static readonly Vector2 CellSize = new Vector2(CellSizeConst, CellSizeConst);

        private static GraphicsDevice GDevice;
        public static GraphicsDevice GraphicsDevice
        {
            get { return GDevice; }
            set
            {
                // Проверяем, было ли уже присвоено значение
                if (GDevice != null)
                    throw new InvalidOperationException("GraphicsDevice может быть установлен только один раз.");
                GDevice = value;
            }
        }
    }
}
