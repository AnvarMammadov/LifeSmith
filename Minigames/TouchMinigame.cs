using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using LifeSmith.Systems;
using System.Collections.Generic;
using System.Linq;

namespace LifeSmith.Minigames
{
    /// <summary>
    /// Touch/Feel minigame for H-scenes
    /// Multi-stage progression with different active zones per stage
    /// </summary>
    public class TouchMinigame
    {
        private List<TouchZone> _zones;
        private int _currentStage = 0;
        private int _totalStages = 3;
        private float _overallProgress = 0f;
        private bool _isComplete = false;
        
        private Rectangle _bounds;
        private Texture2D _pixelTexture;
        
        // Visual settings
        private string _characterName;
        private string[] _stageMessages = new[]
        {
            "Stage 1: Gentle touching...",
            "Stage 2: Getting warmer...",
            "Stage 3: Almost there..."
        };

        public bool IsComplete => _isComplete;
        public float OverallProgress => _overallProgress;
        public int CurrentStage => _currentStage;

        public TouchMinigame(GraphicsDevice graphicsDevice, string characterName, Rectangle bounds)
        {
            _characterName = characterName;
            _bounds = bounds;
            
            _pixelTexture = new Texture2D(graphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });
            
            CreateZones();
            ActivateStageZones();
        }

        private void CreateZones()
        {
            _zones = new List<TouchZone>();
            
            int centerX = _bounds.Center.X;
            int centerY = _bounds.Center.Y;
            
            // Create touchable zones (placeholders - in real game these would be body parts)
            // Top zones (shoulders, face)
            _zones.Add(new TouchZone(
                new Rectangle(centerX - 150, centerY - 150, 100, 100),
                "Left Shoulder",
                0.2f
            ));
            
            _zones.Add(new TouchZone(
                new Rectangle(centerX + 50, centerY - 150, 100, 100),
                "Right Shoulder",
                0.2f
            ));
            
            // Middle zones (chest area - placeholder)
            _zones.Add(new TouchZone(
                new Rectangle(centerX - 100, centerY - 30, 80, 80),
                "Left Chest",
                0.25f
            ));
            
            _zones.Add(new TouchZone(
                new Rectangle(centerX + 20, centerY - 30, 80, 80),
                "Right Chest",
                0.25f
            ));
            
            // Lower zones (waist, legs)
            _zones.Add(new TouchZone(
                new Rectangle(centerX - 80, centerY + 80, 70, 90),
                "Waist",
                0.3f
            ));
            
            _zones.Add(new TouchZone(
                new Rectangle(centerX - 120, centerY + 180, 60, 100),
                "Left Leg",
                0.15f
            ));
            
            _zones.Add(new TouchZone(
                new Rectangle(centerX + 60, centerY + 180, 60, 100),
                "Right Leg",
                0.15f
            ));
        }

        private void ActivateStageZones()
        {
            // Deactivate all zones first
            foreach (var zone in _zones)
            {
                zone.IsActive = false;
                zone.Reset();
            }
            
            // Activate zones based on current stage
            switch (_currentStage)
            {
                case 0: // Stage 1: Shoulders and arms (gentle)
                    _zones[0].IsActive = true; // Left shoulder
                    _zones[1].IsActive = true; // Right shoulder
                    _zones[6].IsActive = true; // Right leg
                    break;
                    
                case 1: // Stage 2: Chest area
                    _zones[2].IsActive = true; // Left chest
                    _zones[3].IsActive = true; // Right chest
                    _zones[4].IsActive = true; // Waist
                    break;
                    
                case 2: // Stage 3: More intimate
                    _zones[2].IsActive = true; // Left chest
                    _zones[3].IsActive = true; // Right chest
                    _zones[4].IsActive = true; // Waist
                    _zones[5].IsActive = true; // Left leg
                    break;
            }
        }

        public void Update(GameTime gameTime)
        {
            if (_isComplete) return;
            
            var input = InputManager.Instance;
            var mousePos = input.MousePosition;
            bool isMousePressed = input.IsLeftMouseButtonPressed;
            
            // Update all zones
            foreach (var zone in _zones)
            {
                zone.Update(mousePos, isMousePressed);
            }
            
            // Check if all active zones are complete
            var activeZones = _zones.Where(z => z.IsActive).ToList();
            if (activeZones.All(z => z.IsComplete()))
            {
                _currentStage++;
                
                if (_currentStage >= _totalStages)
                {
                    _isComplete = true;
                    _overallProgress = 1.0f;
                }
                else
                {
                    ActivateStageZones();
                }
            }
            
            // Update overall progress
            if (!_isComplete)
            {
                float stageProgress = 0f;
                if (activeZones.Count > 0)
                {
                    stageProgress = activeZones.Average(z => z.Progress);
                }
                _overallProgress = (_currentStage + stageProgress) / (float)_totalStages;
            }
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            // Draw background
            spriteBatch.Draw(_pixelTexture, _bounds, new Color(30, 30, 40, 230));
            
            // Draw title
            string title = $"Intimate Scene - {_characterName}";
            spriteBatch.DrawString(font, title, new Vector2(_bounds.X + 50, _bounds.Y + 20), Color.White);
            
            // Draw stage message
            if (_currentStage < _stageMessages.Length)
            {
                spriteBatch.DrawString(font, _stageMessages[_currentStage], 
                    new Vector2(_bounds.X + 50, _bounds.Y + 50), Color.LightPink);
            }
            
            // Draw overall progress bar
            int progressBarWidth = 400;
            int progressBarHeight = 30;
            var progressBarBg = new Rectangle(_bounds.X + 50, _bounds.Y + 80, progressBarWidth, progressBarHeight);
            var progressBarFill = new Rectangle(_bounds.X + 50, _bounds.Y + 80, 
                (int)(progressBarWidth * _overallProgress), progressBarHeight);
            
            spriteBatch.Draw(_pixelTexture, progressBarBg, new Color(60, 60, 60));
            spriteBatch.Draw(_pixelTexture, progressBarFill, new Color(255, 100, 150));
            
            string progressText = $"{(int)(_overallProgress * 100)}%";
            spriteBatch.DrawString(font, progressText, 
                new Vector2(_bounds.X + 480, _bounds.Y + 85), Color.White);
            
            // Draw instruction
            string instruction = "Click and hold on highlighted zones!";
            spriteBatch.DrawString(font, instruction, 
                new Vector2(_bounds.X + 150, _bounds.Y + 120), new Color(200, 200, 200));
            
            // Draw touch zones
            foreach (var zone in _zones)
            {
                Color zoneColor = zone.GetCurrentColor();
                spriteBatch.Draw(_pixelTexture, zone.Bounds, zoneColor);
                
                // Draw zone progress if active
                if (zone.IsActive && zone.Progress > 0)
                {
                    var progressRect = new Rectangle(
                        zone.Bounds.X,
                        zone.Bounds.Bottom - (int)(zone.Bounds.Height * zone.Progress),
                        zone.Bounds.Width,
                        (int)(zone.Bounds.Height * zone.Progress)
                    );
                    spriteBatch.Draw(_pixelTexture, progressRect, new Color(255, 150, 200, 200));
                }
                
                // Draw zone label (small)
                if (zone.IsActive)
                {
                    var labelSize = font.MeasureString(zone.Name);
                    // Scale down the text
                    spriteBatch.DrawString(font, zone.Name,
                        new Vector2(zone.Bounds.Center.X - labelSize.X / 2, zone.Bounds.Center.Y - labelSize.Y / 2),
                        Color.White, 0f, Vector2.Zero, 0.6f, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0f);
                }
            }
            
            // If complete, show message
            if (_isComplete)
            {
                string completeText = "Scene Complete!";
                var textSize = font.MeasureString(completeText);
                var textPos = new Vector2(
                    _bounds.Center.X - textSize.X / 2,
                    _bounds.Y + 150
                );
                
                // Background box
                var boxRect = new Rectangle(
                    (int)(textPos.X - 20),
                    (int)(textPos.Y - 10),
                    (int)(textSize.X + 40),
                    (int)(textSize.Y + 20)
                );
                spriteBatch.Draw(_pixelTexture, boxRect, new Color(255, 100, 150, 230));
                spriteBatch.DrawString(font, completeText, textPos, Color.White);
                
                string continueText = "Click anywhere to continue...";
                spriteBatch.DrawString(font, continueText,
                    new Vector2(_bounds.Center.X - font.MeasureString(continueText).X / 2, textPos.Y + 40),
                    Color.LightGray);
            }
        }

        public void Dispose()
        {
            _pixelTexture?.Dispose();
        }
    }
}
