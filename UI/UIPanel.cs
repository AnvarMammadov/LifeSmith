using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LifeSmith.UI
{
    /// <summary>
    /// Reusable panel component for consistent UI styling across the game.
    /// Provides visual novel-style panels with gradients and semi-transparency.
    /// </summary>
    public class UIPanel
    {
        private Texture2D _pixelTexture;
        public Rectangle Bounds { get; set; }
        
        // Visual properties
        public Color BackgroundColor { get; set; }
        public Color BorderColor { get; set; }
        public int BorderThickness { get; set; }
        public float Opacity { get; set; }
        
        // Gradient support
        public bool UseGradient { get; set; }
        public Color GradientTopColor { get; set; }
        public Color GradientBottomColor { get; set; }

        public UIPanel(GraphicsDevice graphicsDevice, Rectangle bounds)
        {
            _pixelTexture = new Texture2D(graphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });
            
            Bounds = bounds;
            BackgroundColor = new Color(20, 20, 30, 220); // Dark semi-transparent
            BorderColor = new Color(60, 60, 80, 255);
            BorderThickness = 2;
            Opacity = 1f;
            UseGradient = false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw border if enabled
            if (BorderThickness > 0)
            {
                // Top border
                spriteBatch.Draw(_pixelTexture, 
                    new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, BorderThickness), 
                    BorderColor * Opacity);
                // Bottom border
                spriteBatch.Draw(_pixelTexture, 
                    new Rectangle(Bounds.X, Bounds.Bottom - BorderThickness, Bounds.Width, BorderThickness), 
                    BorderColor * Opacity);
                // Left border
                spriteBatch.Draw(_pixelTexture, 
                    new Rectangle(Bounds.X, Bounds.Y, BorderThickness, Bounds.Height), 
                    BorderColor * Opacity);
                // Right border
                spriteBatch.Draw(_pixelTexture, 
                    new Rectangle(Bounds.Right - BorderThickness, Bounds.Y, BorderThickness, Bounds.Height), 
                    BorderColor * Opacity);
            }

            // Draw background
            if (UseGradient)
            {
                DrawGradient(spriteBatch);
            }
            else
            {
                spriteBatch.Draw(_pixelTexture, Bounds, BackgroundColor * Opacity);
            }
        }

        private void DrawGradient(SpriteBatch spriteBatch)
        {
            // Simple vertical gradient using horizontal strips
            int steps = 20;
            int stripHeight = Bounds.Height / steps;
            
            for (int i = 0; i < steps; i++)
            {
                float ratio = i / (float)steps;
                Color blendedColor = Color.Lerp(GradientTopColor, GradientBottomColor, ratio);
                
                Rectangle stripRect = new Rectangle(
                    Bounds.X,
                    Bounds.Y + (i * stripHeight),
                    Bounds.Width,
                    stripHeight + 1 // +1 to avoid gaps
                );
                
                spriteBatch.Draw(_pixelTexture, stripRect, blendedColor * Opacity);
            }
        }

        public void Dispose()
        {
            _pixelTexture?.Dispose();
        }
    }
}
