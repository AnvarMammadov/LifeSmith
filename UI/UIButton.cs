using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using LifeSmith.Systems;
using System;

namespace LifeSmith.UI
{
    /// <summary>
    /// Modern button component with hover states and smooth transitions.
    /// Designed for visual novel-style interfaces.
    /// </summary>
    public class UIButton
    {
        private Texture2D _pixelTexture;
        private SpriteFont _font;
        
        public Rectangle Bounds { get; set; }
        public string Text { get; set; }
        public bool IsEnabled { get; set; }
        
        // Visual states
        public Color IdleColor { get; set; }
        public Color HoverColor { get; set; }
        public Color PressedColor { get; set; }
        public Color DisabledColor { get; set; }
        public Color TextColor { get; set; }
        
        // Border
        public Color BorderColor { get; set; }
        public int BorderThickness { get; set; }
        
        // State tracking
        public bool IsHovered { get; private set; }
        private bool _wasPressed;
        
        // Event
        public Action OnClick;

        public UIButton(GraphicsDevice graphicsDevice, SpriteFont font, Rectangle bounds, string text)
        {
            _pixelTexture = new Texture2D(graphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });
            _font = font;
            
            Bounds = bounds;
            Text = text;
            IsEnabled = true;
            
            // Default visual novel colors
            IdleColor = new Color(40, 45, 60, 220);
            HoverColor = new Color(60, 70, 90, 240);
            PressedColor = new Color(30, 35, 50, 255);
            DisabledColor = new Color(30, 30, 30, 150);
            TextColor = Color.White;
            BorderColor = new Color(80, 90, 110, 255);
            BorderThickness = 2;
        }

        public void Update()
        {
            if (!IsEnabled)
            {
                IsHovered = false;
                _wasPressed = false;
                return;
            }

            Point mousePos = InputManager.Instance.MousePosition;
            IsHovered = Bounds.Contains(mousePos);
            
            if (IsHovered && InputManager.Instance.WasLeftMouseButtonJustPressed)
            {
                _wasPressed = true;
            }
            
            if (_wasPressed && !InputManager.Instance.IsLeftMouseButtonPressed)
            {
                if (IsHovered)
                {
                    OnClick?.Invoke();
                }
                _wasPressed = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Color bgColor = IdleColor;
            
            if (!IsEnabled)
            {
                bgColor = DisabledColor;
            }
            else if (_wasPressed && IsHovered)
            {
                bgColor = PressedColor;
            }
            else if (IsHovered)
            {
                bgColor = HoverColor;
            }

            // Draw border
            if (BorderThickness > 0)
            {
                // Top
                spriteBatch.Draw(_pixelTexture, 
                    new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, BorderThickness), 
                    BorderColor);
                // Bottom
                spriteBatch.Draw(_pixelTexture, 
                    new Rectangle(Bounds.X, Bounds.Bottom - BorderThickness, Bounds.Width, BorderThickness), 
                    BorderColor);
                // Left
                spriteBatch.Draw(_pixelTexture, 
                    new Rectangle(Bounds.X, Bounds.Y, BorderThickness, Bounds.Height), 
                    BorderColor);
                // Right
                spriteBatch.Draw(_pixelTexture, 
                    new Rectangle(Bounds.Right - BorderThickness, Bounds.Y, BorderThickness, Bounds.Height), 
                    BorderColor);
            }

            // Draw background
            spriteBatch.Draw(_pixelTexture, Bounds, bgColor);

            // Draw text (centered)
            if (!string.IsNullOrEmpty(Text) && _font != null)
            {
                Vector2 textSize = _font.MeasureString(Text);
                Vector2 textPos = new Vector2(
                    Bounds.Center.X - textSize.X / 2,
                    Bounds.Center.Y - textSize.Y / 2
                );
                
                Color finalTextColor = IsEnabled ? TextColor : new Color(100, 100, 100);
                spriteBatch.DrawString(_font, Text, textPos, finalTextColor);
            }
        }

        public void Dispose()
        {
            _pixelTexture?.Dispose();
        }
    }
}
