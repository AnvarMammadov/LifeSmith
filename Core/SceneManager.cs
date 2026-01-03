using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using LifeSmith.UI;

namespace LifeSmith.Core
{
    public class SceneManager
    {
        private Scene _currentScene;
        private Scene _nextScene;
        private Game1 _game;
        private GraphicsDevice _graphicsDevice;
        private SpriteBatch _spriteBatch;

        private InventoryUI _inventoryUI;
        private SpriteFont _globalFont;

        public SceneManager(Game1 game, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            _game = game;
            _graphicsDevice = graphicsDevice;
            _spriteBatch = spriteBatch;
            
            // Core UI systems
            try
            {
                _globalFont = game.Content.Load<SpriteFont>("DefaultFont");
                _inventoryUI = new InventoryUI(graphicsDevice, _globalFont);
            }
            catch
            {
                System.Console.WriteLine("Warning: Failed to load DefaultFont for InventoryUI");
            }
        }

        public Scene CurrentScene => _currentScene;

        public void ChangeScene(Scene newScene)
        {
            _nextScene = newScene;
        }

        public void Update(GameTime gameTime)
        {
            // Handle scene transition
            if (_nextScene != null)
            {
                System.Console.WriteLine($"Scene transition: Unloading current scene");
                try
                {
                    _currentScene?.UnloadContent();
                    System.Console.WriteLine($"Initializing new scene: {_nextScene.GetType().Name}");
                    _currentScene = _nextScene;
                    _currentScene.Initialize(_game, _graphicsDevice, _spriteBatch);
                    System.Console.WriteLine("Loading new scene content");
                    _currentScene.LoadContent();
                    _nextScene = null;
                    System.Console.WriteLine("Scene transition complete");
                }
                catch (System.Exception ex)
                {
                    System.Console.WriteLine($"Exception during scene transition: {ex.Message}");
                    System.Console.WriteLine($"Stack trace: {ex.StackTrace}");
                    throw;
                }
            }

            _currentScene?.Update(gameTime);
            _inventoryUI?.Update();
        }

        public void Draw(GameTime gameTime)
        {
            _currentScene?.Draw(gameTime);
            
            // Draw Global UI Overlay
            if (_inventoryUI != null)
            {
                _spriteBatch.Begin();
                _inventoryUI.Draw(_spriteBatch);
                _spriteBatch.End();
            }
        }
    }
}
