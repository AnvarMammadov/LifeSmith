using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using LifeSmith.Systems;
using LifeSmith.Core;
using System;

namespace LifeSmith.Minigames
{
    /// <summary>
    /// Timing-based minigame for studying with Ayako
    /// Player must press SPACE when indicator is in green zone
    /// </summary>
    public class StudyMinigame
    {
        private GraphicsDevice _graphicsDevice;
        private Texture2D _pixelTexture;
        private Rectangle _bounds;
        
        // Minigame state
        public bool IsActive { get; private set; } = true;
        public bool IsSuccess { get; private set; } = false;
        private int _currentRound = 0;
        private const int TotalRounds = 5;
        
        // Timing bar
        private float _indicatorPosition = 0f; // 0 to 1
        private float _indicatorSpeed = 0.8f; // Speed of movement
        private bool _movingRight = true;
        
        // Success zones
        private float _greenZoneStart = 0.4f;
        private float _greenZoneEnd = 0.6f;
        
        // Results
        private int _successfulRounds = 0;
        private float _roundCooldown = 0f;
        private string _resultMessage = "";
        
        public StudyMinigame(GraphicsDevice graphicsDevice, Rectangle bounds)
        {
            _graphicsDevice = graphicsDevice;
            _bounds = bounds;
            
            _pixelTexture = new Texture2D(graphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });
        }

        public void Update(GameTime gameTime)
        {
            if (!IsActive) return;
            
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            // Handle cooldown between rounds
            if (_roundCooldown > 0)
            {
                _roundCooldown -= deltaTime;
                if (_roundCooldown <= 0)
                {
                    _currentRound++;
                    _resultMessage = "";
                    
                    if (_currentRound >= TotalRounds)
                    {
                        // Minigame complete
                        IsActive = false;
                        IsSuccess = _successfulRounds >= 3; // Need 3/5 to succeed
                        return;
                    }
                }
                return;
            }
            
            // Move indicator
            if (_movingRight)
            {
                _indicatorPosition += _indicatorSpeed * deltaTime;
                if (_indicatorPosition >= 1f)
                {
                    _indicatorPosition = 1f;
                    _movingRight = false;
                }
            }
            else
            {
                _indicatorPosition -= _indicatorSpeed * deltaTime;
                if (_indicatorPosition <= 0f)
                {
                    _indicatorPosition = 0f;
                    _movingRight = true;
                }
            }
            
            // Check for player input
            var input = InputManager.Instance;
            if (input.IsKeyJustPressed(Microsoft.Xna.Framework.Input.Keys.Space))
            {
                // Check if in green zone
                if (_indicatorPosition >= _greenZoneStart && _indicatorPosition <= _greenZoneEnd)
                {
                    _successfulRounds++;
                    _resultMessage = "Perfect!";
                }
                else
                {
                    _resultMessage = "Missed...";
                }
                
                // Reset for next round
                _indicatorPosition = 0f;
                _movingRight = true;
                _roundCooldown = 0.5f; // Half second between rounds
            }
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            // Dark overlay
            spriteBatch.Draw(_pixelTexture, _bounds, new Color(0, 0, 0, 220));
            
            // Title
            string title = $"Study Session - Round {_currentRound + 1}/{TotalRounds}";
            Vector2 titleSize = font.MeasureString(title);
            spriteBatch.DrawString(font, title, 
                new Vector2(_bounds.Center.X - titleSize.X / 2, _bounds.Y + 50), 
                Color.White);
            
            // Instructions
            string instructions = "Press SPACE when in GREEN zone!";
            Vector2 instSize = font.MeasureString(instructions);
            spriteBatch.DrawString(font, instructions, 
                new Vector2(_bounds.Center.X - instSize.X / 2, _bounds.Y + 100), 
                Color.Yellow);
            
            // Timing bar background
            int barWidth = 800;
            int barHeight = 60;
            Rectangle barBg = new Rectangle(_bounds.Center.X - barWidth / 2, _bounds.Center.Y, barWidth, barHeight);
            spriteBatch.Draw(_pixelTexture, barBg, Color.DarkGray);
            
            // Green zone
            int greenZoneX = barBg.X + (int)(barWidth * _greenZoneStart);
            int greenZoneWidth = (int)(barWidth * (_greenZoneEnd - _greenZoneStart));
            Rectangle greenZone = new Rectangle(greenZoneX, barBg.Y, greenZoneWidth, barHeight);
            spriteBatch.Draw(_pixelTexture, greenZone, new Color(50, 200, 50, 150));
            
            // Indicator (moving bar)
            if (_roundCooldown <= 0)
            {
                int indicatorX = barBg.X + (int)(barWidth * _indicatorPosition);
                Rectangle indicator = new Rectangle(indicatorX - 5, barBg.Y - 10, 10, barHeight + 20);
                spriteBatch.Draw(_pixelTexture, indicator, Color.Cyan);
            }
            
            // Result message
            if (!string.IsNullOrEmpty(_resultMessage))
            {
                Vector2 resultSize = font.MeasureString(_resultMessage);
                Color resultColor = _resultMessage.Contains("Perfect") ? Color.LimeGreen : Color.Red;
                spriteBatch.DrawString(font, _resultMessage, 
                    new Vector2(_bounds.Center.X - resultSize.X / 2, _bounds.Center.Y - 80), 
                    resultColor);
            }
            
            // Score display
            string score = $"Successful: {_successfulRounds}/{TotalRounds}";
            spriteBatch.DrawString(font, score, 
                new Vector2(_bounds.X + 50, _bounds.Bottom - 100), 
                Color.White);
            
            // Completion message
            if (!IsActive)
            {
                string completeMsg = IsSuccess ? 
                    "Great job! Ayako learned a lot!" : 
                    "Could be better... Try again next time.";
                Vector2 completeSize = font.MeasureString(completeMsg);
                spriteBatch.DrawString(font, completeMsg, 
                    new Vector2(_bounds.Center.X - completeSize.X / 2, _bounds.Bottom - 150), 
                    Color.Gold);
                
                string clickMsg = "Click anywhere to continue...";
                Vector2 clickSize = font.MeasureString(clickMsg);
                spriteBatch.DrawString(font, clickMsg, 
                    new Vector2(_bounds.Center.X - clickSize.X / 2, _bounds.Bottom - 100), 
                    Color.Gray);
            }
        }

        public int GetGradeBonus()
        {
            if (!IsActive)
            {
                int baseBonus = _successfulRounds * 2; // Each successful round = +2 grade
                int studyBoost = GameStateManager.Instance.CurrentStudyBoost;
                return baseBonus + studyBoost;
            }
            return 0;
        }

        public void Dispose()
        {
            _pixelTexture?.Dispose();
        }
    }
}
