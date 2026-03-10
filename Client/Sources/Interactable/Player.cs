
using System.Numerics;

namespace Client.Interactable
{
    public class Player:Character
    {
        public void Move(Vector2 movement)
        {
            //movement *= 0.1f;
            Position += movement;
        }
    }
}
