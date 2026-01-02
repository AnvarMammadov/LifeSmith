using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using LifeSmith.Systems;
using System;

namespace LifeSmith.Minigames
{
    public class TouchMinigame
    {
        private GraphicsDevice _graphicsDevice;
        private Texture2D _pixelTexture;
        private Rectangle _bounds;
        private string _targetName;
        
        // Progress
        public float PleasureLevel { get; private set; } = 0f;
        public float MaxPleasure { get; private set; } = 100f;
        public bool IsComplete => PleasureLevel >= MaxPleasure;
        
        // Interaction
        private Vector2 _lastMousePos;
        private float _rubSpeed = 0.5f; // Pleasure per pixel move
        private Color _barColor = Color.Pink;

        public TouchMinigame(GraphicsDevice graphicsDevice, string targetName, Rectangle bounds)
        {
            _graphicsDevice = graphicsDevice;
            _targetName = targetName;
            _bounds = bounds;
            
            _pixelTexture = new Texture2D(graphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });
        }

        public void Update(GameTime gameTime)
        {
            var input = InputManager.Instance;
            
            if (input.IsLeftMouseButtonPressed && _bounds.Contains(input.MousePosition))
            {
                // Rubbing mechanic: Calculate distance moved
                Vector2 currentPos = input.MousePosition.ToVector2();
                if (input.WasLeftMouseButtonJustPressed)
                {
                    _lastMousePos = currentPos;
                }
                
                float distance = Vector2.Distance(currentPos, _lastMousePos);
                if (distance > 0)
                {
                    PleasureLevel += distance * _rubSpeed * 0.1f;
                    if (PleasureLevel > MaxPleasure) PleasureLevel = MaxPleasure;
                    
                    _lastMousePos = currentPos;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            // Dim background
            spriteBatch.Draw(_pixelTexture, _bounds, new Color(0, 0, 0, 200));
            
            // Draw Target (Placeholder text/box)
            spriteBatch.DrawString(font, $"Touching: {_targetName}", new Vector2(_bounds.X + 20, _bounds.Y + 20), Color.White);
            
            // Draw "Touch Zone" (Placeholder for character art)
            Rectangle touchZone = new Rectangle(_bounds.Center.X - 100, _bounds.Center.Y - 100, 200, 300);
            spriteBatch.Draw(_pixelTexture, touchZone, new Color(255, 200, 200));
            spriteBatch.DrawString(font, "Rub Here!", new Vector2(touchZone.X + 50, touchZone.Y + 140), Color.Black);

            // Draw Progress Bar
            int barWidth = 600;
            int barHeight = 40;
            Rectangle barBg = new Rectangle(_bounds.Center.X - barWidth / 2, _bounds.Bottom - 100, barWidth, barHeight);
            Rectangle barFill = new Rectangle(barBg.X, barBg.Y, (int)(barWidth * (PleasureLevel / MaxPleasure)), barHeight);
            
            spriteBatch.Draw(_pixelTexture, barBg, Color.Gray);
            spriteBatch.Draw(_pixelTexture, barFill, _barColor);
            
            string progressText = $"Pleasure: {(int)PleasureLevel}%";
            Vector2 textSize = font.MeasureString(progressText);
            spriteBatch.DrawString(font, progressText, 
                new Vector2(barBg.Center.X - textSize.X / 2, barBg.Center.Y - textSize.Y / 2), 
                Color.White);

            if (IsComplete)
            {
                string completeText = "Satisfied! Click to finish.";
                spriteBatch.DrawString(font, completeText, 
                    new Vector2(_bounds.Center.X - 150, _bounds.Bottom - 50), Color.Gold);
            }
        }

        public void Dispose()
        {
            _pixelTexture?.Dispose();
        }
    }
}
