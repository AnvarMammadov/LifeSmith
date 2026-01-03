using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using LifeSmith.Core;
using LifeSmith.Systems;
using System.Collections.Generic;

namespace LifeSmith.UI
{
    /// <summary>
    /// Modern inventory UI with grid layout for 1600x900 resolution.
    /// Visual novel-style design with proper spacing and aesthetics.
    /// </summary>
    public class InventoryUI
    {
        private UIPanel _panel;
        private Texture2D _pixelTexture;
        private SpriteFont _font;
        private bool _isVisible = false;
        
        // Layout constants for 1600x900
        private const int PANEL_WIDTH = 800;
        private const int PANEL_HEIGHT = 600;
        private const int PANEL_X = (1600 - PANEL_WIDTH) / 2; // Centered: 400
        private const int PANEL_Y = 150;
        
        // Grid setup
        private int _slotSize = 100;
        private int _padding = 12;
        private int _cols = 6;
        
        public InventoryUI(GraphicsDevice graphicsDevice, SpriteFont font)
        {
            _font = font;
            _pixelTexture = new Texture2D(graphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });
            
            // Create main panel with gradient
            _panel = new UIPanel(graphicsDevice, 
                new Rectangle(PANEL_X, PANEL_Y, PANEL_WIDTH, PANEL_HEIGHT));
            _panel.UseGradient = true;
            _panel.GradientTopColor = new Color(30, 35, 50, 240);
            _panel.GradientBottomColor = new Color(20, 25, 40, 250);
            _panel.BorderColor = new Color(80, 90, 120, 255);
            _panel.BorderThickness = 3;
        }

        public void Update()
        {
            if (InputManager.Instance.WasKeyJustPressed(Keys.Tab) || 
                InputManager.Instance.WasKeyJustPressed(Keys.I))
            {
                Toggle();
            }
        }

        public void Toggle()
        {
            _isVisible = !_isVisible;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!_isVisible) return;

            // Draw semi-transparent background overlay
            spriteBatch.Draw(_pixelTexture, new Rectangle(0, 0, 1600, 900), new Color(0, 0, 0, 180));

            // Draw Panel
            _panel.Draw(spriteBatch);
            
            // Draw Title
            string title = "INVENTORY";
            Vector2 titleSize = _font.MeasureString(title);
            spriteBatch.DrawString(_font, title, 
                new Vector2(_panel.Bounds.X + 30, _panel.Bounds.Y + 25), 
                new Color(220, 220, 255));
            
            // Draw Money
            string moneyText = $"Money: ${GameStateManager.Instance.Money}";
            Vector2 moneySize = _font.MeasureString(moneyText);
            spriteBatch.DrawString(_font, moneyText, 
                new Vector2(_panel.Bounds.Right - moneySize.X - 30, _panel.Bounds.Y + 25), 
                Color.LightGreen);

            // Draw Close Hint
            string closeHint = "Press TAB or I to close";
            Vector2 hintSize = _font.MeasureString(closeHint);
            spriteBatch.DrawString(_font, closeHint,
                new Vector2(_panel.Bounds.Center.X - hintSize.X / 2, _panel.Bounds.Y + 25),
                new Color(150, 150, 150));

            // Draw Items Grid
            var items = GameStateManager.Instance.Inventory;
            int startX = _panel.Bounds.X + 40;
            int startY = _panel.Bounds.Y + 80;
            
            for (int i = 0; i < items.Count; i++)
            {
                int row = i / _cols;
                int col = i % _cols;
                
                Rectangle slotRect = new Rectangle(
                    startX + col * (_slotSize + _padding),
                    startY + row * (_slotSize + _padding),
                    _slotSize,
                    _slotSize
                );
                
                // Check if slot is within panel bounds
                if (slotRect.Bottom > _panel.Bounds.Bottom - 20)
                    break; // Don't draw slots outside panel
                
                bool isHovered = slotRect.Contains(InputManager.Instance.MousePosition);
                
                // Draw Slot Background with border
                Color slotBgColor = isHovered ? new Color(60, 70, 90, 255) : new Color(40, 50, 70, 220);
                Color slotBorderColor = isHovered ? new Color(120, 140, 180, 255) : new Color(70, 80, 100, 255);
                
                // Border
                int borderSize = 2;
                spriteBatch.Draw(_pixelTexture, 
                    new Rectangle(slotRect.X - borderSize, slotRect.Y - borderSize, 
                        slotRect.Width + borderSize * 2, slotRect.Height + borderSize * 2), 
                    slotBorderColor);
                
                // Background
                spriteBatch.Draw(_pixelTexture, slotRect, slotBgColor);
                
                // Draw Item Icon (placeholder: first 2-3 letters)
                string itemName = items[i];
                string alias = itemName.Length > 3 ? itemName.Substring(0, 3).ToUpper() : itemName.ToUpper();
                
                Vector2 textSize = _font.MeasureString(alias);
                Vector2 textPos = new Vector2(
                    slotRect.Center.X - textSize.X / 2, 
                    slotRect.Center.Y - textSize.Y / 2
                );
                spriteBatch.DrawString(_font, alias, textPos, Color.White);
                
                // Tooltip on hover
                if (isHovered)
                {
                    string tooltip = itemName;
                    Vector2 tipSize = _font.MeasureString(tooltip);
                    Rectangle tipRect = new Rectangle(
                        slotRect.Right + 10, 
                        slotRect.Top, 
                        (int)tipSize.X + 20, 
                        (int)tipSize.Y + 20
                    );
                    
                    // Make sure tooltip doesn't go off screen
                    if (tipRect.Right > 1600)
                    {
                        tipRect.X = slotRect.Left - tipRect.Width - 10;
                    }
                    
                    // Tooltip background
                    spriteBatch.Draw(_pixelTexture, tipRect, new Color(10, 10, 20, 240));
                    // Tooltip border
                    spriteBatch.Draw(_pixelTexture, 
                        new Rectangle(tipRect.X - 1, tipRect.Y - 1, tipRect.Width + 2, tipRect.Height + 2),
                        new Color(100, 100, 120, 255));
                    spriteBatch.Draw(_pixelTexture, tipRect, new Color(10, 10, 20, 240));
                    
                    // Tooltip text
                    spriteBatch.DrawString(_font, tooltip, 
                        new Vector2(tipRect.X + 10, tipRect.Y + 10), Color.White);
                }
            }
            
            // Empty slots message if no items
            if (items.Count == 0)
            {
                string emptyMsg = "Your inventory is empty";
                Vector2 emptySize = _font.MeasureString(emptyMsg);
                spriteBatch.DrawString(_font, emptyMsg,
                    new Vector2(_panel.Bounds.Center.X - emptySize.X / 2, startY + 100),
                    new Color(120, 120, 140));
            }
        }

        public void Dispose()
        {
            _panel?.Dispose();
            _pixelTexture?.Dispose();
        }
    }
}
