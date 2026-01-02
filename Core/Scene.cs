using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LifeSmith.Core
{
    public abstract class Scene
    {
        protected Game1 Game { get; private set; }
        protected GraphicsDevice GraphicsDevice { get; private set; }
        protected SpriteBatch SpriteBatch { get; private set; }

        public virtual void Initialize(Game1 game, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            Game = game;
            GraphicsDevice = graphicsDevice;
            SpriteBatch = spriteBatch;
        }

        public abstract void LoadContent();
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(GameTime gameTime);
        public abstract void UnloadContent();
    }
}
