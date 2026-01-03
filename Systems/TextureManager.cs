using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using LifeSmith.Core;

namespace LifeSmith.Systems
{
    public class TextureManager
    {
        private static TextureManager _instance;
        public static TextureManager Instance => _instance ??= new TextureManager();

        private Dictionary<string, Texture2D> _textures = new Dictionary<string, Texture2D>();
        private Texture2D _placeholderTexture;
        private Game1 _game;

        public void Initialize(Game1 game)
        {
            _game = game;
            
            // Create a magenta placeholder texture (1x1)
            _placeholderTexture = new Texture2D(game.GraphicsDevice, 1, 1);
            _placeholderTexture.SetData(new[] { Color.Magenta });
        }

        public Texture2D Get(string name)
        {
            if (_textures.ContainsKey(name))
            {
                return _textures[name];
            }

            // Try to load
            try
            {
                var texture = _game.Content.Load<Texture2D>(name);
                _textures[name] = texture;
                return texture;
            }
            catch (System.Exception)
            {
                // Return placeholder silently or log
                return _placeholderTexture; 
            }
        }
        
        // Helper Methods for Organized Folders
        public Texture2D GetBackground(string name) => Get($"Images/Backgrounds/{name}");
        public Texture2D GetCharacter(string name) => Get($"Images/Characters/{name}");
        public Texture2D GetUI(string name) => Get($"Images/UI/{name}");
        public Texture2D GetItem(string name) => Get($"Images/Items/{name}");
        
        // Helper to load raw png from disk (Modding support)
        public Texture2D LoadFromFile(string path, GraphicsDevice graphicsDevice)
        {
             // Code to load png from stream... MonoGame has Texture2D.FromStream
             // Skipping for now, stick to Content Pipeline first or implement later.
             return null;
        }
    }
}
