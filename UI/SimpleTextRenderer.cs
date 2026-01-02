using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LifeSmith.UI
{
    /// <summary>
    /// Simple text renderer using pixel drawing (no font file needed)
    /// </summary>
    public class SimpleTextRenderer
    {
        private Texture2D _pixelTexture;
        private int _charWidth = 8;
        private int _charHeight = 12;

        public SimpleTextRenderer(GraphicsDevice graphicsDevice)
        {
            _pixelTexture = new Texture2D(graphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });
        }

        public void DrawText(SpriteBatch spriteBatch, string text, Vector2 position, Color color, float scale = 1f)
        {
            if (string.IsNullOrEmpty(text)) return;

            float xOffset = 0;
            foreach (char c in text)
            {
                if (c == '\n')
                {
                    position.Y += _charHeight * scale;
                    xOffset = 0;
                    continue;
                }

                // Draw a simple rectangle for each character
                var rect = new Rectangle(
                    (int)(position.X + xOffset),
                    (int)position.Y,
                    (int)(_charWidth * scale),
                    (int)(_charHeight * scale)
                );

                spriteBatch.Draw(_pixelTexture, rect, color);
                xOffset += (_charWidth + 2) * scale;
            }
        }

        public Vector2 MeasureString(string text, float scale = 1f)
        {
            if (string.IsNullOrEmpty(text))
                return Vector2.Zero;

            string[] lines = text.Split('\n');
            float width = 0;
            foreach (var line in lines)
            {
                float lineWidth = line.Length * (_charWidth + 2) * scale;
                if (lineWidth > width)
                    width = lineWidth;
            }

            float height = lines.Length * _charHeight * scale;
            return new Vector2(width, height);
        }

        public void Dispose()
        {
            _pixelTexture?.Dispose();
        }
    }
}
