using Client.Assets;
using Client.Landscape;
using Client.Interactable;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = System.Numerics.Vector2;

namespace Client
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager Graphics;
        private const float CameraSpeed = 300f;
        private SpriteBatch SpriteBatch;
        private TileMap TileMap;
        private Player TestPlayer;
        private Camera Camera;
        private int PreviousScrollValue;
        //private MouseState previousMouseState;

        public Game1()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            // Устанавливаем желаемую ширину и высоту окна
            Graphics.PreferredBackBufferWidth = 1280;
            Graphics.PreferredBackBufferHeight = 784;
            Graphics.ApplyChanges();

            IsMouseVisible = true;
            Config.GraphicsDevice = GraphicsDevice;
            TileMap = new TileMap();
            TestPlayer = new Player();
            TestPlayer.Texture = Manager.CreateColorTexture(Color.Red, 32, 48);
        }

        protected override void Initialize()
        {
            PreviousScrollValue = Mouse.GetState().ScrollWheelValue;
            Camera = new Camera(GraphicsDevice,
                (int)(Config.MapSize.X * Config.CellSize.X),
                (int)(Config.MapSize.Y * Config.CellSize.Y)
            );
            base.Initialize();
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            TileMap.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            Input(gameTime);
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            SpriteBatch.Begin(transformMatrix: Camera.ViewMatrix);
            TileMap.Draw(SpriteBatch);
            TestPlayer.Draw(SpriteBatch);
            SpriteBatch.End();

            base.Draw(gameTime);
        }

        private void Input(GameTime gameTime)
        {
            var currentKeyboard = Keyboard.GetState();
            var mouse = Mouse.GetState();
            var currentMouse = Mouse.GetState();
            //Point? click = null;
            var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 camDelta = Vector2.Zero;
            if (currentKeyboard.IsKeyDown(Keys.Left)) camDelta.X -= CameraSpeed * dt;
            if (currentKeyboard.IsKeyDown(Keys.Right)) camDelta.X += CameraSpeed * dt;
            if (currentKeyboard.IsKeyDown(Keys.Up)) camDelta.Y -= CameraSpeed * dt;
            if (currentKeyboard.IsKeyDown(Keys.Down)) camDelta.Y += CameraSpeed * dt;

            float scroll = mouse.ScrollWheelValue - PreviousScrollValue;
            Camera.ZoomIn(scroll * 0.001f);
            PreviousScrollValue = mouse.ScrollWheelValue;

            Camera.Update(gameTime);
            Camera.Move(camDelta);

            Vector2 movement = Vector2.Zero;

            foreach (Keys key in currentKeyboard.GetPressedKeys())
            {
                switch (key)
                {
                    case Keys.W: movement.Y -= 1; break;
                    case Keys.S: movement.Y += 1; break;
                    case Keys.A: movement.X -= 1; break;
                    case Keys.D: movement.X += 1; break;
                }
            }

            // Чтобы сохранить скорость по диагонали (например, чтобы не двигаться быстрее),
            // нормализуем вектор:
            if (movement != Vector2.Zero)
                movement = Vector2.Normalize(movement);


            TestPlayer.Move(movement);
        }
    }
}
