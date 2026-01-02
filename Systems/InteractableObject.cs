using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace LifeSmith.Systems
{
    public class InteractableObject
    {
        public Rectangle Bounds { get; set; }
        public string Name { get; set; }
        public bool IsHovered { get; private set; }
        public Color IdleColor { get; set; } = Color.White;
        public Color HoverColor { get; set; } = Color.Yellow;
        public Action OnClick { get; set; }

        public InteractableObject(Rectangle bounds, string name, Action onClick = null)
        {
            Bounds = bounds;
            Name = name;
            OnClick = onClick;
        }

        public void Update()
        {
            var input = InputManager.Instance;
            IsHovered = input.IsMouseInRectangle(Bounds);

            if (IsHovered && input.WasLeftMouseButtonJustPressed)
            {
                OnClick?.Invoke();
            }
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D texture)
        {
            Color drawColor = IsHovered ? HoverColor : IdleColor;
            spriteBatch.Draw(texture, Bounds, drawColor);
        }

        // Overload for drawing with just color (no texture)
        public void DrawRectangle(SpriteBatch spriteBatch, Texture2D pixelTexture)
        {
            Color drawColor = IsHovered ? HoverColor : IdleColor;
            spriteBatch.Draw(pixelTexture, Bounds, drawColor);
        }
    }
}
