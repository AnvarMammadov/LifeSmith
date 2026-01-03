using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using LifeSmith.Core;
using LifeSmith.Systems;
using System.Collections.Generic;

namespace LifeSmith.UI
{
    public class InventoryUI
    {
        private Texture2D _pixelTexture;
        private SpriteFont _font;
        private bool _isVisible = false;
        private Rectangle _panelBounds;
        
        // Grid setup
        private int _slotSize = 80;
        private int _padding = 10;
        private int _cols = 5;
        
        public InventoryUI(GraphicsDevice graphicsDevice, SpriteFont font)
        {
            _font = font;
            _pixelTexture = new Texture2D(graphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });
            
            // Center panel (600x400)
            _panelBounds = new Rectangle(
                (1280 - 600) / 2, 
                (720 - 400) / 2, 
                600, 
                400
            );
        }

        public void Update()
        {
            if (InputManager.Instance.WasKeyJustPressed(Keys.Tab) || 
                InputManager.Instance.WasKeyJustPressed(Keys.I))
            {
                _isVisible = !_isVisible;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!_isVisible) return;

            // Draw semi-transparent background overlay
            spriteBatch.Draw(_pixelTexture, new Rectangle(0, 0, 1280, 720), new Color(0, 0, 0, 150));

            // Draw Panel Background
            spriteBatch.Draw(_pixelTexture, _panelBounds, new Color(40, 40, 40));
            
            // Draw Title
            spriteBatch.DrawString(_font, "INVENTORY", 
                new Vector2(_panelBounds.X + 20, _panelBounds.Y + 20), Color.White);
            
            // Draw Money
            string moneyText = $"Money: ${GameStateManager.Instance.Money}";
            Vector2 moneySize = _font.MeasureString(moneyText);
            spriteBatch.DrawString(_font, moneyText, 
                new Vector2(_panelBounds.Right - moneySize.X - 20, _panelBounds.Y + 20), Color.LightGreen);

            // Draw Items Grid
            var items = GameStateManager.Instance.Inventory;
            int startX = _panelBounds.X + 30;
            int startY = _panelBounds.Y + 60;
            
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
                
                // Draw Slot Background
                bool isHovered = slotRect.Contains(InputManager.Instance.MousePosition);
                spriteBatch.Draw(_pixelTexture, slotRect, isHovered ? Color.Gray : Color.DarkGray);
                
                // Draw Item Icon (Placeholder: First letter)
                string itemName = items[i];
                string alias = itemName.Length > 2 ? itemName.Substring(0, 2).ToUpper() : itemName;
                
                // Try to get icon from TextureManager (will return placeholder if not found)
                // spriteBatch.Draw(TextureManager.Instance.Get(itemName), slotRect, Color.White); 
                // For now, text representation
                Vector2 textSize = _font.MeasureString(alias);
                spriteBatch.DrawString(_font, alias, 
                    new Vector2(slotRect.Center.X - textSize.X / 2, slotRect.Center.Y - textSize.Y / 2), 
                    Color.White);
                
                // Tooltip
                if (isHovered)
                {
                    string tooltip = itemName; // Can look up description later
                    Vector2 tipSize = _font.MeasureString(tooltip);
                    Rectangle tipRect = new Rectangle(
                        slotRect.Right + 5, slotRect.Top, 
                        (int)tipSize.X + 10, (int)tipSize.Y + 10);
                    
                    spriteBatch.Draw(_pixelTexture, tipRect, Color.Black);
                    spriteBatch.DrawString(_font, tooltip, 
                        new Vector2(tipRect.X + 5, tipRect.Y + 5), Color.White);
                }
            }
        }
    }
}
