using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = System.Numerics.Vector2;

namespace Client.Interactable
{
    public class Character: IDynamicInteractable
    {
        public Vector2 Position = new Vector2(10,10);
        public Texture2D Texture;
        public Character() { }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture,
                    new Rectangle((int)Position.X, (int)Position.Y,
                        (int)Config.CellSize.X, (int)Config.CellSize.Y),
                    Color.White);
        }
    }
}
