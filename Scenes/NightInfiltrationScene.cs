using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using LifeSmith.Core;
using LifeSmith.Systems;
using LifeSmith.Content;
using LifeSmith.Minigames;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LifeSmith.Scenes
{
    public class NightInfiltrationScene : Scene
    {
        private Texture2D _pixelTexture;
        private SpriteFont _font;
        private List<InteractableObject> _interactables = new();
        private House _currentHouse;
        private Random _random = new();
        
        private bool _entrySuccessful = false;
        private List<EntryPoint> _preparedEntries;
        private string _statusMessage = "Choose an entry point...";
        private bool _needsRefresh = false; // Flag to refresh interactables
        
        // Touch minigame
        private TouchMinigame _touchMinigame;
        private bool _isTouchMinigame = false;

        public override void LoadContent()
        {
            _pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });

            _font = Game.Content.Load<SpriteFont>("DefaultFont");
            _currentHouse = JobManager.Instance.CurrentJob;
            _preparedEntries = _currentHouse.EntryPoints.Where(e => e.IsPrepared).ToList();

            if (_preparedEntries.Count == 0)
            {
                _statusMessage = "No entry points prepared! Mission failed...";
            }
            else
            {
                CreateEntryPointButtons();
            }
        }

        private void CreateEntryPointButtons()
        {
            _interactables.Clear();

            if (!_entrySuccessful)
            {
                int yPos = 150;
                
                // Check for copied key (Advanced Entry)
                string keyId = $"key_{_currentHouse.ResidentName}";
                if (GameStateManager.Instance.Inventory.Contains(keyId))
                {
                    var keyButton = new InteractableObject(
                        new Rectangle(500, yPos, 600, 70),
                        "Use Copied Key (Front Door) - 100%",
                        OnKeyEntry
                    )
                    {
                        IdleColor = new Color(255, 215, 0), // Gold
                        HoverColor = new Color(255, 255, 100)
                    };
                    _interactables.Add(keyButton);
                    yPos += 90;
                }

                foreach (var entry in _preparedEntries)
                {
                    var entryButton = new InteractableObject(
                        new Rectangle(500, yPos, 600, 70),
                        $"{entry.Name} - {(int)(entry.BaseSuccessChance * 100)}% chance",
                        () => OnAttemptEntry(entry)
                    )
                    {
                        IdleColor = new Color(100, 100, 200),
                        HoverColor = new Color(150, 150, 255)
                    };
                    _interactables.Add(entryButton);
                    yPos += 90;
                }

                // Return home button (mission abort) - adjusted for 1600x900, above nav bar
                var returnButton = new InteractableObject(
                    new Rectangle(100, 700, 200, 60),
                    "Abort Mission",
                    OnReturnHome
                )
                {
                    IdleColor = new Color(100, 100, 100),
                    HoverColor = new Color(180, 180, 180)
                };
                _interactables.Add(returnButton);
            }
            else
            {
                // Successfully inside - collect items and H-scene
                var collectButton = new InteractableObject(
                    new Rectangle(550, 250, 500, 80),
                    "Collect Valuables",
                    OnCollectItems
                )
                {
                    IdleColor = new Color(200, 150, 50),
                    HoverColor = new Color(255, 200, 100)
                };
                _interactables.Add(collectButton);

                // H-Scene button (placeholder)
                var hSceneButton = new InteractableObject(
                    new Rectangle(550, 350, 500, 80),
                    "Approach Character",
                    OnHSceneStart
                )
                {
                    IdleColor = new Color(200, 50, 100),
                    HoverColor = new Color(255, 100, 150)
                };
                _interactables.Add(hSceneButton);

                // Escape button
                var escapeButton = new InteractableObject(
                    new Rectangle(550, 500, 500, 80),
                    "Escape",
                    OnEscape
                )
                {
                    IdleColor = new Color(50, 150, 50),
                    HoverColor = new Color(100, 255, 100)
                };
                _interactables.Add(escapeButton);
            }
        }

        private void OnAttemptEntry(EntryPoint entry)
        {
            bool success = entry.AttemptEntry(_random);
            
            if (success)
            {
                _statusMessage = $"Success! Entered through {entry.Name}";
                _entrySuccessful = true;
                _needsRefresh = true; // Set flag instead of calling directly
            }
            else
            {
                _statusMessage = $"Failed! {entry.Name} was blocked. Try another...";
                // Remove this entry from options
                _preparedEntries.Remove(entry);
                
                if (_preparedEntries.Count == 0)
                {
                    _statusMessage = "All entry points failed! Mission failed...";
                }
                
                _needsRefresh = true; // Set flag instead of calling directly
            }
        }

        private void OnKeyEntry()
        {
            _statusMessage = "Success! Unlocked Front Door with key.";
            _entrySuccessful = true;
            _needsRefresh = true;
        }

        private void OnCollectItems()
        {
            // Give money reward
            GameStateManager.Instance.AddMoney(_currentHouse.MoneyReward);
            _statusMessage = $"Collected ${_currentHouse.MoneyReward} and valuables!";
        }

        private void OnHSceneStart()
        {
            // Start touch minigame (adjusted for 1600x900)
            var minigameBounds = new Rectangle(200, 50, 1200, 700);
            _touchMinigame = new TouchMinigame(GraphicsDevice, _currentHouse.ResidentName, minigameBounds);
            _isTouchMinigame = true;
        }

        private void OnEscape()
        {
            // Complete the job and return
            JobManager.Instance.CompleteJob();
            GameStateManager.Instance.ClearEntryPoints();
            Game.SceneManager.ChangeScene(new PlayerApartmentScene());
        }

        private void OnReturnHome()
        {
            // Failed mission - return without rewards
            GameStateManager.Instance.ClearEntryPoints();
            Game.SceneManager.ChangeScene(new PlayerApartmentScene());
        }

        public override void Update(GameTime gameTime)
        {
            InputManager.Instance.Update();

            // Handle touch minigame
            if (_isTouchMinigame)
            {
                _touchMinigame?.Update(gameTime);
                
                if (_touchMinigame.IsComplete)
                {
                    // Check if user clicked to continue
                    if (InputManager.Instance.WasLeftMouseButtonJustPressed)
                    {
                        _isTouchMinigame = false;
                        _touchMinigame.Dispose();
                        _touchMinigame = null;
                        _statusMessage = "H-Scene completed!";
                    }
                }
                
                return; // Don't process other input while in minigame
            }

            foreach (var interactable in _interactables)
            {
                interactable.Update();
            }

            // Refresh after iteration is complete
            if (_needsRefresh)
            {
                CreateEntryPointButtons();
                _needsRefresh = false;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(20, 20, 40)); // Dark blue background (night)

            SpriteBatch.Begin();

            // Draw touch minigame if active
            if (_isTouchMinigame)
            {
                _touchMinigame?.Draw(SpriteBatch, _font);
                SpriteBatch.End();
                return; // Don't draw anything else
            }

            // Draw title (adjusted for 1600x900)
            SpriteBatch.DrawString(_font, $"Night Infiltration - {_currentHouse.ResidentName}'s House", 
                new Vector2(400, 20), Color.White);
            SpriteBatch.DrawString(_font, _statusMessage, new Vector2(300, 80), Color.Yellow);

            // Draw interactables
            foreach (var interactable in _interactables)
            {
                interactable.DrawRectangle(SpriteBatch, _pixelTexture);
                
                var textSize = _font.MeasureString(interactable.Name);
                var textPos = new Vector2(
                    interactable.Bounds.Center.X - textSize.X / 2,
                    interactable.Bounds.Center.Y - textSize.Y / 2
                );
                SpriteBatch.DrawString(_font, interactable.Name, textPos, Color.White);
            }

            SpriteBatch.End();
        }

        public override void UnloadContent()
        {
            _pixelTexture?.Dispose();
        }
    }
}
