using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using LifeSmith.Systems;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LifeSmith.Minigames
{
    /// <summary>
    /// Mafia 2 style lock picking minigame
    /// Move mouse left/right to find sweet spot for each pin
    /// </summary>
    public class LockPickingMinigame
    {
        private List<LockPin> _pins;
        private int _currentPinIndex = 0;
        private Random _random;
        private float _elapsedTime = 0f;
        private bool _isComplete = false;
        
        // Visual settings
        private Rectangle _bounds;
        private Texture2D _pixelTexture;
        
        // Mouse tracking
        private int _previousMouseX;
        private float _mouseSensitivity = 0.003f; // How fast the pin moves with mouse
        
        public bool IsComplete => _isComplete;
        public float ElapsedTime => _elapsedTime;
        public int TotalPins => _pins.Count;
        public int LockedPins => _pins.Count(p => p.IsLocked);

        public LockPickingMinigame(GraphicsDevice graphicsDevice, int difficulty, Rectangle bounds)
        {
            _random = new Random();
            _bounds = bounds;
            
            // Create pixel texture
            _pixelTexture = new Texture2D(graphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });
            
            // Create pins based on difficulty
            _pins = new List<LockPin>();
            int pinCount = Math.Min(3 + difficulty, 7); // 3-7 pins based on difficulty
            float tolerance = 0.08f - (difficulty * 0.01f); // Harder = smaller tolerance
            
            for (int i = 0; i < pinCount; i++)
            {
                _pins.Add(new LockPin(_random, tolerance));
            }
            
            _previousMouseX = InputManager.Instance.MousePosition.X;
        }

        public void Update(GameTime gameTime)
        {
            if (_isComplete) return;
            
            _elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            var input = InputManager.Instance;
            var currentPin = _pins[_currentPinIndex];
            
            // Update position based on mouse movement
            int currentMouseX = input.MousePosition.X;
            int mouseDelta = currentMouseX - _previousMouseX;
            _previousMouseX = currentMouseX;
            
            if (!currentPin.IsLocked)
            {
                currentPin.UpdatePosition(mouseDelta * _mouseSensitivity);
            }
            
            // Lock pin when in sweet spot and spacebar/click pressed
            if (input.WasLeftMouseButtonJustPressed || input.WasKeyJustPressed(Microsoft.Xna.Framework.Input.Keys.Space))
            {
                if (currentPin.IsInSweetSpot() && !currentPin.IsLocked)
                {
                    currentPin.IsLocked = true;
                    _currentPinIndex++;
                    
                    // Check if all pins are locked
                    if (_currentPinIndex >= _pins.Count)
                    {
                        _isComplete = true;
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            // Draw background
            spriteBatch.Draw(_pixelTexture, _bounds, new Color(40, 40, 40, 200));
            
            // Draw title
            string title = "LOCK PICKING - Move mouse left/right to find sweet spot";
            spriteBatch.DrawString(font, title, new Vector2(_bounds.X + 50, _bounds.Y + 20), Color.White);
            
            // Draw timer
            string timerText = $"Time: {_elapsedTime:F1}s";
            spriteBatch.DrawString(font, timerText, new Vector2(_bounds.X + 50, _bounds.Y + 50), Color.Yellow);
            
            // Draw progress
            string progressText = $"Pins: {LockedPins}/{TotalPins}";
            spriteBatch.DrawString(font, progressText, new Vector2(_bounds.X + 200, _bounds.Y + 50), Color.Cyan);
            
            // Draw instruction
            string instruction = "Click or press SPACE when in green zone!";
            spriteBatch.DrawString(font, instruction, new Vector2(_bounds.X + 150, _bounds.Y + 80), Color.LightGreen);
            
            // Draw pins
            int pinY = _bounds.Y + 150;
            int pinSpacing = 60;
            
            for (int i = 0; i < _pins.Count; i++)
            {
                var pin = _pins[i];
                bool isCurrentPin = (i == _currentPinIndex);
                
                DrawPin(spriteBatch, font, pin, i, pinY + (i * pinSpacing), isCurrentPin);
            }
            
            if (_isComplete)
            {
                string completeText = "LOCK OPENED!";
                var textSize = font.MeasureString(completeText);
                var textPos = new Vector2(
                    _bounds.Center.X - textSize.X / 2,
                    _bounds.Center.Y - textSize.Y / 2
                );
                
                // Background box
                var boxRect = new Rectangle(
                    (int)(textPos.X - 20),
                    (int)(textPos.Y - 10),
                    (int)(textSize.X + 40),
                    (int)(textSize.Y + 20)
                );
                spriteBatch.Draw(_pixelTexture, boxRect, new Color(0, 200, 0, 220));
                
                spriteBatch.DrawString(font, completeText, textPos, Color.White);
            }
        }

        private void DrawPin(SpriteBatch spriteBatch, SpriteFont font, LockPin pin, int index, int y, bool isActive)
        {
            int trackX = _bounds.X + 100;
            int trackWidth = _bounds.Width - 200;
            int trackHeight = 40;
            
            // Draw track background
            var trackRect = new Rectangle(trackX, y, trackWidth, trackHeight);
            spriteBatch.Draw(_pixelTexture, trackRect, new Color(60, 60, 60));
            
            // Draw sweet spot zone (green)
            int sweetSpotX = trackX + (int)(pin.TargetPosition * trackWidth);
            int sweetSpotWidth = (int)(pin.Tolerance * 2 * trackWidth);
            var sweetSpotRect = new Rectangle(
                sweetSpotX - sweetSpotWidth / 2,
                y,
                sweetSpotWidth,
                trackHeight
            );
            
            Color sweetSpotColor = pin.IsInSweetSpot() ? new Color(0, 255, 0, 180) : new Color(0, 150, 0, 100);
            spriteBatch.Draw(_pixelTexture, sweetSpotRect, sweetSpotColor);
            
            // Draw current position indicator
            int indicatorX = trackX + (int)(pin.CurrentPosition * trackWidth);
            int indicatorWidth = 8;
            
            // Add shake effect based on proximity
            if (isActive && !pin.IsLocked)
            {
                float shakeIntensity = pin.GetShakeIntensity();
                int shakeOffset = (int)(_random.NextDouble() * shakeIntensity * 10 - shakeIntensity * 5);
                indicatorX += shakeOffset;
            }
            
            var indicatorRect = new Rectangle(
                indicatorX - indicatorWidth / 2,
                y - 5,
                indicatorWidth,
                trackHeight + 10
            );
            
            Color indicatorColor;
            if (pin.IsLocked)
                indicatorColor = Color.LightGreen;
            else if (isActive)
                indicatorColor = Color.Yellow;
            else
                indicatorColor = Color.Gray;
            
            spriteBatch.Draw(_pixelTexture, indicatorRect, indicatorColor);
            
            // Draw pin label
            string label = pin.IsLocked ? $"Pin {index + 1} [LOCKED]" : $"Pin {index + 1}";
            Color labelColor = pin.IsLocked ? Color.LightGreen : (isActive ? Color.Yellow : Color.Gray);
            spriteBatch.DrawString(font, label, new Vector2(trackX - 90, y + 10), labelColor);
        }

        public float GetBonusTimeMultiplier()
        {
            // Faster completion = more bonus time
            if (_elapsedTime < 10f) return 2.0f; // Very fast: 2x time bonus
            if (_elapsedTime < 20f) return 1.5f; // Fast: 1.5x time bonus
            if (_elapsedTime < 30f) return 1.2f; // Normal: 1.2x time bonus
            return 1.0f; // Slow: No bonus
        }

        public void Dispose()
        {
            _pixelTexture?.Dispose();
        }
    }
}
