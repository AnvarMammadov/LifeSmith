using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using LifeSmith.Core;
using LifeSmith.Systems;
using LifeSmith.Content;
using LifeSmith.Minigames;
using LifeSmith.Dialog;
using System.Collections.Generic;

namespace LifeSmith.Scenes
{
    public class JobSiteScene : Scene
    {
        private Texture2D _pixelTexture;
        private SpriteFont _font;
        private List<InteractableObject> _interactables = new();
        private House _currentHouse;
        private bool _lockPickingComplete = false;
        private float _explorationTimeRemaining = 60f; // seconds
        private bool _needsRefresh = false; // Flag to refresh interactables
        
        // Lock picking minigame
        private LockPickingMinigame _lockPickingMinigame;
        private bool _isLockPicking = false;
        
        // Dialog system
        private DialogBoxUI _dialogBox;
        private bool _dialogComplete = false;
        
        public override void LoadContent()
        {
            _pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });

            _font = Game.Content.Load<SpriteFont>("DefaultFont");
            _currentHouse = JobManager.Instance.CurrentJob;
            
            // Initialize dialog box
            var dialogBounds = new Rectangle(50, 450, 1180, 250);
            _dialogBox = new DialogBoxUI(GraphicsDevice, dialogBounds);
            
            // Start greeting dialog
            var firstNode = GameStateManager.Instance.DialogManager.GetNode("emily_greeting");
            var relationship = GameStateManager.Instance.DialogManager.GetRelationship(_currentHouse.ResidentName);
            _dialogBox.StartDialog(firstNode, relationship);
            
            CreateInteractables();
        }

        private void CreateInteractables()
        {
            _interactables.Clear();

            if (!_lockPickingComplete)
            {
                // Lock picking area
                var lockButton = new InteractableObject(
                    new Rectangle(500, 300, 200, 100),
                    "Pick Lock",
                    OnLockClicked
                )
                {
                    IdleColor = new Color(180, 180, 0),
                    HoverColor = new Color(255, 255, 100)
                };
                _interactables.Add(lockButton);
            }
            else
            {
                // Entry points to prepare
                int yPos = 100;
                foreach (var entryPoint in _currentHouse.EntryPoints)
                {
                    if (!entryPoint.IsDiscovered)
                    {
                        var discoverButton = new InteractableObject(
                            new Rectangle(50, yPos, 250, 60),
                            $"Discover {entryPoint.Name}",
                            () => OnDiscoverEntryPoint(entryPoint)
                        )
                        {
                            IdleColor = new Color(100, 100, 150),
                            HoverColor = new Color(150, 150, 255)
                        };
                        _interactables.Add(discoverButton);
                    }
                    else if (!entryPoint.IsPrepared)
                    {
                        var prepareButton = new InteractableObject(
                            new Rectangle(50, yPos, 250, 60),
                            $"Prepare {entryPoint.Name}",
                            () => OnPrepareEntryPoint(entryPoint)
                        )
                        {
                            IdleColor = new Color(150, 100, 50),
                            HoverColor = new Color(255, 150, 100)
                        };
                        _interactables.Add(prepareButton);
                    }
                    else
                    {
                        // Already prepared - just show it
                        var preparedButton = new InteractableObject(
                            new Rectangle(50, yPos, 250, 60),
                            $"{entryPoint.Name} [READY]",
                            null
                        )
                        {
                            IdleColor = new Color(50, 200, 50),
                            HoverColor = new Color(100, 255, 100)
                        };
                        _interactables.Add(preparedButton);
                    }
                    yPos += 70;
                }

                // Clickable items (bra, photos, etc.)
                var braItem = new InteractableObject(
                    new Rectangle(600, 150, 80, 60),
                    "Bra",
                    () => OnItemClicked("Bra")
                )
                {
                    IdleColor = new Color(255, 100, 150),
                    HoverColor = new Color(255, 150, 200)
                };
                _interactables.Add(braItem);

                _interactables.Add(braItem);
                
                // Key Copying (Advanced Mechanic)
                string keyId = $"key_{_currentHouse.ResidentName}";
                if (GameStateManager.Instance.HasTool("key_copier"))
                {
                    if (!GameStateManager.Instance.Inventory.Contains(keyId))
                    {
                        var copyKeyButton = new InteractableObject(
                            new Rectangle(800, 150, 150, 60),
                            "Copy Key",
                            OnCopyKey
                        )
                        {
                            IdleColor = new Color(255, 215, 0), // Gold
                            HoverColor = new Color(255, 255, 100)
                        };
                        _interactables.Add(copyKeyButton);
                    }
                    else
                    {
                         // Show copied indicator
                         var copiedIndicator = new InteractableObject(
                            new Rectangle(800, 150, 150, 60),
                            "Key Copied!",
                            null
                        )
                        {
                            IdleColor = Color.Gray,
                            HoverColor = Color.Gray
                        };
                        _interactables.Add(copiedIndicator);
                    }
                }

                // Return home button
                var returnButton = new InteractableObject(
                    new Rectangle(1000, 600, 150, 60),
                    "Return Home",
                    OnReturnHome
                )
                {
                    IdleColor = new Color(100, 100, 100),
                    HoverColor = new Color(180, 180, 180)
                };
                _interactables.Add(returnButton);
            }
        }

        private void OnLockClicked()
        {
            // 1. Check for Story Events (First encounter, special events, etc.)
            var evt = EventManager.Instance.CheckForEvent("OnInteract", "Door");
            if (evt != null)
            {
                EventManager.Instance.ExecuteEvent(evt);
                
                if (evt.ActionType == "Dialog")
                {
                    // Start dialog
                    System.Console.WriteLine($"Starting Dialog Event: {evt.ActionValue}");
                    var dialogManager = GameStateManager.Instance.DialogManager;
                    var node = dialogManager.GetNode(evt.ActionValue);
                    if (node != null)
                    {
                        var relationship = dialogManager.GetRelationship("Emily"); // Hardcoded char for now
                        _dialogBox.StartDialog(node, relationship);
                        return; // Don't start minigame yet
                    }
                }
            }

            // 2. Start lock picking minigame (Default action)
            var minigameBounds = new Rectangle(200, 100, 880, 520);
            
            // Check for upgrades
            float extraTolerance = 0f;
            if (GameStateManager.Instance.HasTool("advanced_lockpick"))
            {
                extraTolerance = 0.05f; // Significant bonus
            }
            
            _lockPickingMinigame = new LockPickingMinigame(GraphicsDevice, _currentHouse.LockDifficulty, minigameBounds, extraTolerance);
            _isLockPicking = true;
        }

        private void OnDiscoverEntryPoint(EntryPoint entryPoint)
        {
            entryPoint.Discover();
            _needsRefresh = true; // Set flag instead of calling directly
        }

        private void OnPrepareEntryPoint(EntryPoint entryPoint)
        {
            entryPoint.Prepare();
            GameStateManager.Instance.PrepareEntryPoint(entryPoint.Type.ToString());
            _needsRefresh = true; // Set flag instead of calling directly
        }

        private void OnItemClicked(string itemName)
        {
            if (!_currentHouse.DiscoveredItems.Contains(itemName))
            {
                _currentHouse.DiscoveredItems.Add(itemName);
            }
        }
        
        private void OnCopyKey()
        {
            string keyId = $"key_{_currentHouse.ResidentName}"; // Using Name as ID for simplicity
            GameStateManager.Instance.Inventory.Add(keyId);
            System.Console.WriteLine($"Key copied: {keyId}");
            _needsRefresh = true;
        }

        private void OnReturnHome()
        {
            Game.SceneManager.ChangeScene(new PlayerApartmentScene());
        }

        public override void Update(GameTime gameTime)
        {
            InputManager.Instance.Update();

            // Handle dialog system
            if (_dialogBox.IsActive && !_dialogComplete)
            {
                _dialogBox.Update(gameTime, out DialogChoice selectedChoice);
                
                if (selectedChoice != null)
                {
                    // Apply choice effects
                    var relationship = GameStateManager.Instance.DialogManager.GetRelationship(_currentHouse.ResidentName);
                    GameStateManager.Instance.DialogManager.ApplyChoice(selectedChoice, _currentHouse.ResidentName);
                    
                    // Get next node
                    var nextNode = GameStateManager.Instance.DialogManager.GetNode(selectedChoice.NextNodeId);
                    
                    if (nextNode != null)
                    {
                        _dialogBox.StartDialog(nextNode, relationship);
                        
                        // Check if dialog is ending
                        if (nextNode.IsEndNode)
                        {
                            _dialogComplete = true;
                        }
                    }
                }
                
                // Close dialog when complete
                if (_dialogBox.IsComplete && !_dialogBox.IsActive)
                {
                    _dialogComplete = true;
                }
                
                return; // Don't process other input while in dialog
            }

            // Handle lock picking minigame
            if (_isLockPicking)
            {
                _lockPickingMinigame?.Update(gameTime);
                
                if (_lockPickingMinigame.IsComplete)
                {
                    // Minigame complete! Award time based on performance
                    _lockPickingComplete = true;
                    float bonusMultiplier = _lockPickingMinigame.GetBonusTimeMultiplier();
                    _explorationTimeRemaining = 60f * bonusMultiplier;
                    _isLockPicking = false;
                    _lockPickingMinigame.Dispose();
                    _lockPickingMinigame = null;
                    _needsRefresh = true;
                }
                
                return; // Don't process other input while picking
            }

            if (_lockPickingComplete)
            {
                _explorationTimeRemaining -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_explorationTimeRemaining <= 0)
                {
                    // Time's up - force return
                    OnReturnHome();
                    return;
                }
            }

            foreach (var interactable in _interactables)
            {
                interactable.Update();
            }

            // Refresh after iteration is complete
            if (_needsRefresh)
            {
                CreateInteractables();
                _needsRefresh = false;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(200, 180, 150)); // Beige background

            SpriteBatch.Begin();

            // Draw lock picking minigame if active
            if (_isLockPicking)
            {
                _lockPickingMinigame?.Draw(SpriteBatch, _font);
                SpriteBatch.End();
                return; // Don't draw anything else
            }

            // Draw title
            SpriteBatch.DrawString(_font, $"{_currentHouse.ResidentName}'s House", new Vector2(450, 20), Color.Black);
            
            if (_lockPickingComplete)
            {
                SpriteBatch.DrawString(_font, $"Time remaining: {(int)_explorationTimeRemaining}s", 
                    new Vector2(50, 620), Color.Red);
            }

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
            
            // Draw dialog box (overlay)
            _dialogBox?.Draw(SpriteBatch, _font);

            SpriteBatch.End();
        }

        public override void UnloadContent()
        {
            _pixelTexture?.Dispose();
        }
    }
}
