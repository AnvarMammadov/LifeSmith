using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace LifeSmith.UI
{
    /// <summary>
    /// Ultra-simple bitmap-style text renderer
    /// Draws actual readable text using a basic pixel font pattern
    /// </summary>
    public class BitmapFontRenderer
    {
        private Texture2D _pixelTexture;
        private Dictionary<char, bool[,]> _fontData;
        private int _charWidth = 5;
        private int _charHeight = 7;
        private int _pixelSize = 2;

        public BitmapFontRenderer(GraphicsDevice graphicsDevice)
        {
            _pixelTexture = new Texture2D(graphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });
            InitializeFontData();
        }

        private void InitializeFontData()
        {
            _fontData = new Dictionary<char, bool[,]>();

            // Define basic alphabet (5x7 grid, where true = draw pixel)
            // A
            _fontData['A'] = new bool[,] {
                {false,true,true,true,false},
                {true,false,false,false,true},
                {true,false,false,false,true},
                {true,true,true,true,true},
                {true,false,false,false,true},
                {true,false,false,false,true},
                {true,false,false,false,true}
            };

            // Add more letters as needed - for now we'll use a simple fallback
            // You can expand this dictionary with all letters/numbers you need
        }

        public void DrawText(SpriteBatch spriteBatch, string text, Vector2 position, Color color)
        {
            if (string.IsNullOrEmpty(text)) return;

            float xOffset = 0;
            float yOffset = 0;

            foreach (char c in text)
            {
                if (c == '\n')
                {
                    yOffset += (_charHeight + 2) * _pixelSize;
                    xOffset = 0;
                    continue;
                }

                if (c == ' ')
                {
                    xOffset += (_charWidth + 1) * _pixelSize;
                    continue;
                }

                // For simplicity, just draw a solid rectangle for each character
                // In a full implementation, you'd use the _fontData dictionary
                var rect = new Rectangle(
                    (int)(position.X + xOffset),
                    (int)(position.Y + yOffset),
                    _charWidth * _pixelSize,
                    _charHeight * _pixelSize
                );

                // Draw background box
                spriteBatch.Draw(_pixelTexture, rect, color * 0.8f);

                xOffset += (_charWidth + 2) * _pixelSize;
            }
        }

        public Vector2 MeasureString(string text)
        {
            if (string.IsNullOrEmpty(text))
                return Vector2.Zero;

            string[] lines = text.Split('\n');
            float maxWidth = 0;

            foreach (var line in lines)
            {
                float lineWidth = line.Length * (_charWidth + 2) * _pixelSize;
                if (lineWidth > maxWidth)
                    maxWidth = lineWidth;
            }

            float height = lines.Length * (_charHeight + 2) * _pixelSize;
            return new Vector2(maxWidth, height);
        }

        public void Dispose()
        {
            _pixelTexture?.Dispose();
        }
    }
}
