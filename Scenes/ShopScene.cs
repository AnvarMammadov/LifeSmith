using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using LifeSmith.Core;
using LifeSmith.Systems;
using System.Collections.Generic;

namespace LifeSmith.Scenes
{
    public class ShopScene : Scene
    {
        private Texture2D _pixelTexture;
        private SpriteFont _font;
        private List<ShopItem> _items;
        
        private Rectangle _backButtonRect;
        private List<Rectangle> _buyButtonRects;
        
        // Visual settings
        private Color _bgColor = new Color(30, 30, 40); // Dark shop theme
        private Color _itemBgColor = new Color(50, 50, 60);
        private Color _textColor = Color.White;
        private Color _priceColor = Color.Gold;
        private Color _purchasedColor = Color.Gray;

        public override void LoadContent()
        {
            _pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });
            _font = Game.Content.Load<SpriteFont>("DefaultFont");
            
            _items = ShopManager.Instance.AvailableItems;
            _buyButtonRects = new List<Rectangle>();
            
            _backButtonRect = new Rectangle(50, 650, 200, 50);
        }

        public override void Update(GameTime gameTime)
        {
            InputManager.Instance.Update();
            
            // Handle Back Button
            if (InputManager.Instance.IsLeftMouseButtonPressed)
            {
                if (_backButtonRect.Contains(InputManager.Instance.MousePosition))
                {
                    // Return to apartment
                    Game.SceneManager.ChangeScene(new PlayerApartmentScene());
                    return;
                }
            }
            
            // Handle Buy Buttons
            if (InputManager.Instance.WasLeftMouseButtonJustPressed)
            {
                for (int i = 0; i < _buyButtonRects.Count; i++)
                {
                    if (_buyButtonRects[i].Contains(InputManager.Instance.MousePosition))
                    {
                        var item = _items[i];
                        if (ShopManager.Instance.BuyItem(item.Id))
                        {
                            System.Console.WriteLine($"Bought {item.Name}!");
                        }
                        else
                        {
                           System.Console.WriteLine("Cannot buy!");
                        }
                    }
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(_bgColor);
            SpriteBatch.Begin();

            // Title
            SpriteBatch.DrawString(_font, "Tools & Equipment Shop", new Vector2(50, 30), _textColor);
            
            // Money display
            string moneyText = $"Money: ${GameStateManager.Instance.Money}";
            SpriteBatch.DrawString(_font, moneyText, new Vector2(1000, 30), Color.LightGreen);

            // Draw Items
            int startY = 100;
            int itemHeight = 80;
            int padding = 20;
            
            _buyButtonRects.Clear(); // Rebuild rects for dynamic positioning

            for (int i = 0; i < _items.Count; i++)
            {
                var item = _items[i];
                int yPos = startY + (i * (itemHeight + padding));
                Rectangle itemRect = new Rectangle(50, yPos, 1180, itemHeight);
                Rectangle buyBtnRect = new Rectangle(itemRect.Right - 150, yPos + 20, 120, 40);
                
                _buyButtonRects.Add(buyBtnRect); // Track button for input
                
                // Item Background
                SpriteBatch.Draw(_pixelTexture, itemRect, _itemBgColor);
                
                // Name & Desc
                SpriteBatch.DrawString(_font, item.Name, new Vector2(70, yPos + 10), Color.Cyan);
                SpriteBatch.DrawString(_font, item.Description, new Vector2(70, yPos + 40), Color.Gray, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0f);
                
                // Buy Button logic
                bool owned = item.IsOneTimePurchase && GameStateManager.Instance.HasTool(item.Id);
                bool canAfford = GameStateManager.Instance.Money >= item.Price;
                
                Color btnColor = owned ? _purchasedColor : (canAfford ? Color.Green : Color.Red);
                string btnText = owned ? "Owned" : $"Buy ${item.Price}";
                
                // Draw Button
                SpriteBatch.Draw(_pixelTexture, buyBtnRect, btnColor);
                
                Vector2 textSize = _font.MeasureString(btnText);
                Vector2 textPos = new Vector2(
                    buyBtnRect.Center.X - textSize.X / 2,
                    buyBtnRect.Center.Y - textSize.Y / 2
                );
                SpriteBatch.DrawString(_font, btnText, textPos, Color.White);
            }

            // Draw Back Button
            SpriteBatch.Draw(_pixelTexture, _backButtonRect, Color.Gray);
            string backText = "Back to Apartment";
            Vector2 backSize = _font.MeasureString(backText);
            SpriteBatch.DrawString(_font, backText, 
                new Vector2(_backButtonRect.Center.X - backSize.X/2, _backButtonRect.Center.Y - backSize.Y/2), 
                Color.White);

            SpriteBatch.End();
        }
        
        public override void UnloadContent()
        {
            _pixelTexture?.Dispose();
        }
    }
}
