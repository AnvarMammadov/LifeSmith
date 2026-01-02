using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using LifeSmith.Systems;
using System;
using System.Collections.Generic;

namespace LifeSmith.Dialog
{
    /// <summary>
    /// Visual novel style dialog box with typewriter effect
    /// </summary>
    public class DialogBoxUI
    {
        private Rectangle _bounds;
        private Texture2D _pixelTexture;
        
        // Typewriter effect
        private string _fullText;
        private string _displayedText;
        private float _typewriterTimer;
        private float _typewriterSpeed = 0.03f; // Seconds per character
        private int _currentCharIndex;
        private bool _isTypingComplete;
        
        // Current dialog
        private DialogNode _currentNode;
        private CharacterRelationship _currentRelationship;
        private List<DialogChoice> _availableChoices;
        private int _selectedChoiceIndex;
        
        // Visual settings
        private Color _boxColor = new Color(20, 20, 30, 230);
        private Color _textColor = Color.White;
        private Color _nameColor = new Color(255, 200, 100);
        private Color _choiceColor = Color.LightGray;
        private Color _selectedChoiceColor = new Color(255, 220, 100);
        
        public bool IsActive { get; private set; }
        public bool IsComplete => _currentNode != null && _currentNode.IsEndNode && _isTypingComplete;
        
        public DialogBoxUI(GraphicsDevice graphicsDevice, Rectangle bounds)
        {
            _bounds = bounds;
            _pixelTexture = new Texture2D(graphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });
            IsActive = false;
        }

        public void StartDialog(DialogNode node, CharacterRelationship relationship)
        {
            _currentNode = node;
            _currentRelationship = relationship;
            _fullText = node.Text;
            _displayedText = "";
            _currentCharIndex = 0;
            _typewriterTimer = 0f;
            _isTypingComplete = false;
            _selectedChoiceIndex = 0;
            _availableChoices = node.GetAvailableChoices(relationship);
            IsActive = true;
        }

        public void Update(GameTime gameTime, out DialogChoice selectedChoice)
        {
            selectedChoice = null;
            
            if (!IsActive) return;
            
            var input = InputManager.Instance;
            
            // Typewriter effect
            if (!_isTypingComplete)
            {
                _typewriterTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                
                while (_typewriterTimer >= _typewriterSpeed && _currentCharIndex < _fullText.Length)
                {
                    _displayedText += _fullText[_currentCharIndex];
                    _currentCharIndex++;
                    _typewriterTimer -= _typewriterSpeed;
                }
                
                if (_currentCharIndex >= _fullText.Length)
                {
                    _isTypingComplete = true;
                }
                
                // Skip typewriter on click
                if (input.WasLeftMouseButtonJustPressed || input.WasKeyJustPressed(Microsoft.Xna.Framework.Input.Keys.Space))
                {
                    _displayedText = _fullText;
                    _currentCharIndex = _fullText.Length;
                    _isTypingComplete = true;
                }
            }
            else
            {
                // Choice selection with arrow keys
                if (input.WasKeyJustPressed(Microsoft.Xna.Framework.Input.Keys.Up))
                {
                    _selectedChoiceIndex--;
                    if (_selectedChoiceIndex < 0) _selectedChoiceIndex = _availableChoices.Count - 1;
                }
                
                if (input.WasKeyJustPressed(Microsoft.Xna.Framework.Input.Keys.Down))
                {
                    _selectedChoiceIndex++;
                    if (_selectedChoiceIndex >= _availableChoices.Count) _selectedChoiceIndex = 0;
                }
                
                // Mouse hover selection
                var mousePos = input.MousePosition;
                for (int i = 0; i < _availableChoices.Count; i++)
                {
                    var choiceRect = GetChoiceRect(i);
                    if (choiceRect.Contains(mousePos))
                    {
                        _selectedChoiceIndex = i;
                    }
                }
                
                // Confirm selection
                if (input.WasLeftMouseButtonJustPressed || input.WasKeyJustPressed(Microsoft.Xna.Framework.Input.Keys.Enter))
                {
                    if (_availableChoices.Count > 0)
                    {
                        selectedChoice = _availableChoices[_selectedChoiceIndex];
                    }
                }
            }
        }

        private Rectangle GetChoiceRect(int index)
        {
            // Center choices in the middle of the screen, not inside the dialog box
            // Assuming 1280x720 resolution
            int startY = 300; 
            int choiceHeight = 50;
            int padding = 10;
            
            int choiceY = startY + (index * (choiceHeight + padding));
            // Center horizontally: 1280/2 = 640. Width 800. X = 640 - 400 = 240.
            return new Rectangle(240, choiceY, 800, choiceHeight);
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            if (!IsActive) return;
            
            // Draw dialog box background
            spriteBatch.Draw(_pixelTexture, _bounds, _boxColor);
            
            // Draw character name
            if (_currentNode != null)
            {
                string nameText = $"{_currentNode.CharacterName}";
                // Draw name background for better visibility
                var nameSize = font.MeasureString(nameText);
                var nameRect = new Rectangle(_bounds.X, _bounds.Y - 30, (int)nameSize.X + 60, 30);
                spriteBatch.Draw(_pixelTexture, nameRect, _nameColor);
                
                spriteBatch.DrawString(font, nameText, 
                    new Vector2(_bounds.X + 30, _bounds.Y - 25), Color.Black);
                
                // Draw Expression
                string exprText = $"({_currentNode.Expression})";
                spriteBatch.DrawString(font, exprText,
                    new Vector2(_bounds.X + nameRect.Width + 10, _bounds.Y - 25), Color.LightGray);
            }
            
            // Draw relationship stats (small)
            if (_currentRelationship != null)
            {
                string trustText = $"Trust: {_currentRelationship.GetTrustLevel()} ({(int)_currentRelationship.TrustLevel})";
                string attractionText = $"Attraction: {_currentRelationship.GetAttractionLevel()} ({(int)_currentRelationship.Attraction})";
                
                spriteBatch.DrawString(font, trustText,
                    new Vector2(_bounds.Right - 350, _bounds.Y - 25),
                    Color.Cyan, 0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0f);
                    
                spriteBatch.DrawString(font, attractionText,
                    new Vector2(_bounds.Right - 350, _bounds.Y - 10),
                    Color.Pink, 0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0f);
            }
            
            // Draw dialog text with word wrap
            DrawWrappedText(spriteBatch, font, _displayedText, 
                new Vector2(_bounds.X + 30, _bounds.Y + 40), 
                _bounds.Width - 60, _textColor);
            
            // Draw typing indicator if not complete
            if (!_isTypingComplete)
            {
                spriteBatch.DrawString(font, "...", 
                    new Vector2(_bounds.Right - 50, _bounds.Bottom - 30), 
                    _textColor);
            }
            else if (_availableChoices.Count > 0)
            {
                // Draw choices overlay (darken background slightly)
                Rectangle fullscreen = new Rectangle(0, 0, 1280, 720);
                spriteBatch.Draw(_pixelTexture, fullscreen, new Color(0, 0, 0, 100)); // Dim background
            
                for (int i = 0; i < _availableChoices.Count; i++)
                {
                    var choice = _availableChoices[i];
                    var choiceRect = GetChoiceRect(i);
                    
                    // Draw choice background
                    bool isSelected = i == _selectedChoiceIndex;
                    Color bgColor = isSelected ? 
                        new Color(100, 100, 150, 240) : new Color(40, 40, 60, 200);
                        
                    // Add hover effect border
                    if (isSelected)
                    {
                        Rectangle border = new Rectangle(choiceRect.X - 2, choiceRect.Y - 2, choiceRect.Width + 4, choiceRect.Height + 4);
                        spriteBatch.Draw(_pixelTexture, border, Color.White);
                    }
                    
                    spriteBatch.Draw(_pixelTexture, choiceRect, bgColor);
                    
                    // Center text in choice box
                    Vector2 textSize = font.MeasureString(choice.Text);
                    Vector2 textPos = new Vector2(
                        choiceRect.Center.X - textSize.X * 0.4f, // Scale is 0.8
                        choiceRect.Center.Y - textSize.Y * 0.4f
                    );
                    
                    Color textColor = isSelected ? _selectedChoiceColor : _choiceColor;
                    spriteBatch.DrawString(font, choice.Text, 
                        textPos, 
                        textColor, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0f);
                }
            }
        }

        private void DrawWrappedText(SpriteBatch spriteBatch, SpriteFont font, string text, 
            Vector2 position, float maxWidth, Color color)
        {
            string[] words = text.Split(' ');
            string line = "";
            float yOffset = 0;
            
            foreach (string word in words)
            {
                string testLine = line + word + " ";
                Vector2 size = font.MeasureString(testLine);
                
                if (size.X > maxWidth && line != "")
                {
                    spriteBatch.DrawString(font, line, 
                        new Vector2(position.X, position.Y + yOffset), color);
                    line = word + " ";
                    yOffset += size.Y;
                }
                else
                {
                    line = testLine;
                }
            }
            
            if (line != "")
            {
                spriteBatch.DrawString(font, line, 
                    new Vector2(position.X, position.Y + yOffset), color);
            }
        }

        public void Close()
        {
            IsActive = false;
        }

        public void Dispose()
        {
            _pixelTexture?.Dispose();
        }
    }
}
