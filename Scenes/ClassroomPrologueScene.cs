using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using LifeSmith.Core;
using LifeSmith.Systems;
using System.Collections.Generic;

namespace LifeSmith.Scenes
{
    /// <summary>
    /// Prologue scene - Ayako sleeping in classroom, upskirt view
    /// Player can LOOK or WAKE UP
    /// </summary>
    public class ClassroomPrologueScene : Scene
    {
        private Texture2D _pixelTexture;
        private SpriteFont _font;
        private List<InteractableObject> _interactables = new();
        
        // Visual state
        private bool _isMonochrome = true;
        private float _fadeAlpha = 1f;
        private bool _isFadingIn = true;
        private bool _isLooking = false;
        private float _lookTimer = 0f;
        private const float LookDuration = 3f;
        
        // Refresh flag to avoid collection modification during iteration
        private bool _needsRefresh = false;
        
        // Dialog state
        private bool _showingDialog = false;
        private int _dialogStep = 0;
        private List<string> _dialogLines = new();
        private bool _transitionToApartment = false;
        private float _transitionTimer = 0f;
        
        public override void LoadContent()
        {
            _pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });
            
            _font = Game.Content.Load<SpriteFont>("DefaultFont");
            
            // Setup dialog sequence
            _dialogLines.Add("Teacher: \"Ayako... Wake up.\"");
            _dialogLines.Add("Ayako: \"Huh...? Oh! Sorry, teacher!\"");
            _dialogLines.Add("Teacher: *Places exam paper on desk*");
            _dialogLines.Add("Ayako: *Sees the F grade* \"No... No no no...\"");
            _dialogLines.Add("Ayako: \"Please, teacher! My father will kill me if I fail!\"");
            _dialogLines.Add("Teacher: \"There might be a way... Private tutoring.\"");
            _dialogLines.Add("Ayako: \"I'll do anything! Please help me!\"");
            _dialogLines.Add("Teacher: \"Come to my apartment tonight. 8 PM. Don't be late.\"");
            _dialogLines.Add("Ayako: \"Thank you so much, teacher! I won't let you down!\"");
            
            CreateInteractables();
        }
        
        private void CreateInteractables()
        {
            _interactables.Clear();
            
            if (!_showingDialog)
            {
                // LOOK button - lower left
                var lookButton = new InteractableObject(
                    new Rectangle(200, 700, 150, 60),
                    "LOOK",
                    OnLookClicked
                )
                {
                    IdleColor = new Color(100, 100, 100, 180),
                    HoverColor = new Color(150, 150, 150, 220)
                };
                _interactables.Add(lookButton);
                
                // WAKE UP button - lower right
                var wakeButton = new InteractableObject(
                    new Rectangle(1250, 700, 150, 60),
                    "WAKE UP",
                    OnWakeUpClicked
                )
                {
                    IdleColor = new Color(100, 100, 100, 180),
                    HoverColor = new Color(150, 150, 150, 220)
                };
                _interactables.Add(wakeButton);
            }
        }
        
        private void OnLookClicked()
        {
            _isLooking = true;
            _lookTimer = 0f;
            _needsRefresh = true; // Remove buttons while looking
        }
        
        private void OnWakeUpClicked()
        {
            _showingDialog = true;
            _dialogStep = 0;
            _needsRefresh = true;
        }
        
        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            // Fade in at start
            if (_isFadingIn)
            {
                _fadeAlpha -= deltaTime * 0.5f;
                if (_fadeAlpha <= 0)
                {
                    _fadeAlpha = 0;
                    _isFadingIn = false;
                }
                return; // Don't process input during fade in
            }
            
            // Handle transition to apartment
            if (_transitionToApartment)
            {
                _transitionTimer += deltaTime;
                _fadeAlpha = _transitionTimer * 0.5f;
                
                if (_fadeAlpha >= 1f)
                {
                    // Transition complete - change scene
                    Game.SceneManager.ChangeScene(new PlayerApartmentScene());
                }
                return;
            }
            
            InputManager.Instance.Update();
            
            // Handle looking state
            if (_isLooking)
            {
                _lookTimer += deltaTime;
                if (_lookTimer >= LookDuration)
                {
                    _isLooking = false;
                    CreateInteractables(); // Restore buttons
                }
                return;
            }
            
            // Handle dialog progression
            if (_showingDialog)
            {
                if (InputManager.Instance.WasLeftMouseButtonJustPressed || 
                    InputManager.Instance.IsKeyJustPressed(Keys.Space))
                {
                    _dialogStep++;
                    
                    if (_dialogStep >= _dialogLines.Count)
                    {
                        // Dialog complete - transition to apartment
                        _transitionToApartment = true;
                        _transitionTimer = 0f;
                    }
                }
                return;
            }
            
            // Update interactables
            foreach (var interactable in _interactables)
            {
                interactable.Update();
            }
            
            // Refresh after iteration is complete
            if (_needsRefresh)
            {
                _interactables.Clear();
                if (!_isLooking && !_showingDialog)
                {
                    CreateInteractables(); // Restore buttons if needed
                }
                _needsRefresh = false;
            }
        }
        
        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            SpriteBatch.Begin();
            
            // Background - monochrome classroom
            Rectangle classroomBg = new Rectangle(0, 0, 1600, 900);
            Color bgColor = _isMonochrome ? new Color(60, 60, 65) : new Color(80, 85, 90);
            SpriteBatch.Draw(_pixelTexture, classroomBg, bgColor);
            
            // Window (light source)
            Rectangle window = new Rectangle(1200, 100, 300, 400);
            SpriteBatch.Draw(_pixelTexture, window, new Color(120, 120, 130, 150));
            
            // Desk
            Rectangle desk = new Rectangle(400, 500, 600, 300);
            SpriteBatch.Draw(_pixelTexture, desk, new Color(40, 40, 45));
            
            // Ayako's body (placeholder - upper part)
            Rectangle body = new Rectangle(600, 400, 200, 250);
            SpriteBatch.Draw(_pixelTexture, body, new Color(180, 150, 130)); // Skin tone
            
            // Uniform (dark)
            Rectangle uniform = new Rectangle(620, 450, 160, 150);
            SpriteBatch.Draw(_pixelTexture, uniform, new Color(30, 30, 50));
            
            // Legs (from low camera angle) - THIS IS THE KEY VIEW
            Rectangle leftLeg = new Rectangle(500, 600, 80, 250);
            Rectangle rightLeg = new Rectangle(720, 600, 80, 250);
            SpriteBatch.Draw(_pixelTexture, leftLeg, new Color(180, 150, 130));
            SpriteBatch.Draw(_pixelTexture, rightLeg, new Color(180, 150, 130));
            
            // Skirt (slightly lifted view)
            Rectangle skirt = new Rectangle(520, 580, 260, 80);
            SpriteBatch.Draw(_pixelTexture, skirt, new Color(50, 50, 80));
            
            // Underwear peek (subtle, slightly visible)
            Rectangle underwear = new Rectangle(620, 620, 60, 30);
            SpriteBatch.Draw(_pixelTexture, underwear, new Color(200, 200, 220, 200));
            
            // If looking, emphasize the view
            if (_isLooking)
            {
                // Darken everything except the legs area
                Rectangle darkOverlay = new Rectangle(0, 0, 1600, 550);
                SpriteBatch.Draw(_pixelTexture, darkOverlay, new Color(0, 0, 0, 100));
                
                string lookText = "...";
                Vector2 lookSize = _font.MeasureString(lookText);
                SpriteBatch.DrawString(_font, lookText, 
                    new Vector2(800 - lookSize.X / 2, 400), 
                    Color.Gray);
            }
            
            // Draw interactables
            foreach (var interactable in _interactables)
            {
                interactable.DrawRectangle(SpriteBatch, _pixelTexture);
                
                // Draw button text
                Vector2 textSize = _font.MeasureString(interactable.Name);
                Vector2 textPos = new Vector2(
                    interactable.Bounds.Center.X - textSize.X / 2,
                    interactable.Bounds.Center.Y - textSize.Y / 2
                );
                SpriteBatch.DrawString(_font, interactable.Name, textPos, Color.White);
            }
            
            // Draw dialog
            if (_showingDialog && _dialogStep < _dialogLines.Count)
            {
                // Dialog box
                Rectangle dialogBox = new Rectangle(100, 700, 1400, 150);
                SpriteBatch.Draw(_pixelTexture, dialogBox, new Color(0, 0, 0, 230));
                
                // Border
                Rectangle dialogBorder = new Rectangle(95, 695, 1410, 160);
                SpriteBatch.Draw(_pixelTexture, new Rectangle(95, 695, 1410, 3), Color.White);
                SpriteBatch.Draw(_pixelTexture, new Rectangle(95, 852, 1410, 3), Color.White);
                SpriteBatch.Draw(_pixelTexture, new Rectangle(95, 695, 3, 160), Color.White);
                SpriteBatch.Draw(_pixelTexture, new Rectangle(1502, 695, 3, 160), Color.White);
                
                // Text
                string currentLine = _dialogLines[_dialogStep];
                SpriteBatch.DrawString(_font, currentLine, new Vector2(120, 730), Color.White);
                
                // Continue prompt
                string prompt = "[Click or Press SPACE to continue]";
                Vector2 promptSize = _font.MeasureString(prompt);
                SpriteBatch.DrawString(_font, prompt, 
                    new Vector2(1480 - promptSize.X, 820), 
                    Color.Gray);
            }
            
            // Transition overlay
            if (_transitionToApartment || _isFadingIn)
            {
                Rectangle fadeOverlay = new Rectangle(0, 0, 1600, 900);
                SpriteBatch.Draw(_pixelTexture, fadeOverlay, new Color(0, 0, 0, _fadeAlpha));
                
                if (_transitionToApartment && _transitionTimer > 0.5f && _transitionTimer < 2f)
                {
                    string transText = "Later that evening...";
                    Vector2 transSize = _font.MeasureString(transText);
                    SpriteBatch.DrawString(_font, transText, 
                        new Vector2(800 - transSize.X / 2, 450), 
                        Color.White * (1f - (_transitionTimer - 0.5f) / 1.5f));
                }
            }
            
            SpriteBatch.End();
        }
        
        public override void UnloadContent()
        {
            _pixelTexture?.Dispose();
        }
    }
}
