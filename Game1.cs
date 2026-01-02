using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using LifeSmith.Core;
using LifeSmith.Scenes;
using LifeSmith.Systems;

namespace LifeSmith
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public SceneManager SceneManager { get; private set; }

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            // Set resolution to 1280x720
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Initialize TextureManager
            TextureManager.Instance.Initialize(this);

            // Initialize scene manager
            SceneManager = new SceneManager(this, GraphicsDevice, _spriteBatch);
            
            // Check for save file
            if (SaveManager.HasSaveFile())
            {
                System.Console.WriteLine("Save file found. Press F9 to load.");
            }
            
            // Start with player apartment scene
            SceneManager.ChangeScene(new PlayerApartmentScene());
        }

        protected override void Update(GameTime gameTime)
        {
            var kstate = Keyboard.GetState();
            
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || 
                kstate.IsKeyDown(Keys.Escape))
                Exit();

            // Debug Save/Load
            if (kstate.IsKeyDown(Keys.F5))
            {
                SaveManager.SaveGame();
            }
            
            if (kstate.IsKeyDown(Keys.F9))
            {
                if (SaveManager.LoadGame())
                {
                    // Refresh scene to reflect loaded state
                    // Simple way: Reload current scene type
                    var currentSceneType = SceneManager.CurrentScene.GetType();
                    // SceneManager.ChangeScene((Scene)System.Activator.CreateInstance(currentSceneType));
                    // Even simpler: Go to apartment
                    SceneManager.ChangeScene(new PlayerApartmentScene());
                }
            }

            // Update scene manager
            SceneManager?.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Draw current scene
            SceneManager?.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}
