using Microsoft.Xna.Framework;

namespace LifeSmith.Minigames
{
    /// <summary>
    /// Represents a touchable zone in the touch minigame
    /// </summary>
    public class TouchZone
    {
        public Rectangle Bounds { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public float Progress { get; set; } // 0.0 to 1.0
        public float ProgressSpeed { get; set; } // How fast it fills when touched
        public Color IdleColor { get; set; }
        public Color ActiveColor { get; set; }
        public Color HoverColor { get; set; }
        
        private bool _isHovered;
        private bool _isTouching;

        public TouchZone(Rectangle bounds, string name, float progressSpeed = 0.3f)
        {
            Bounds = bounds;
            Name = name;
            ProgressSpeed = progressSpeed;
            Progress = 0f;
            IsActive = false;
            
            IdleColor = new Color(100, 100, 100, 150);
            ActiveColor = new Color(255, 100, 150, 200);
            HoverColor = new Color(255, 150, 200, 180);
        }

        public void Update(Point mousePos, bool isMousePressed)
        {
            _isHovered = Bounds.Contains(mousePos);
            
            if (!IsActive)
            {
                return;
            }
            
            if (_isHovered && isMousePressed)
            {
                _isTouching = true;
                Progress += ProgressSpeed * 0.016f; // Assuming ~60 FPS
                if (Progress > 1f) Progress = 1f;
            }
            else
            {
                _isTouching = false;
                // Slowly decay if not touching
                Progress -= 0.1f * 0.016f;
                if (Progress < 0f) Progress = 0f;
            }
        }

        public Color GetCurrentColor()
        {
            if (!IsActive)
                return IdleColor;
            
            if (_isTouching)
                return ActiveColor;
            
            if (_isHovered)
                return HoverColor;
            
            return new Color(150, 150, 150, 180);
        }

        public bool IsComplete()
        {
            return Progress >= 1.0f;
        }

        public void Reset()
        {
            Progress = 0f;
            _isHovered = false;
            _isTouching = false;
        }
    }
}
