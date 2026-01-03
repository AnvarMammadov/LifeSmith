using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using LifeSmith.Core;
using LifeSmith.Systems;
using System.Collections.Generic;

namespace LifeSmith.Scenes
{
    public class PlayerApartmentScene : Scene
    {
        private Texture2D _pixelTexture;
        private Texture2D _background;
        private SpriteFont _font;
        private List<InteractableObject> _interactables = new();
        private bool _needsRefresh = false; // Flag to refresh interactables

        // UI Text
        private string _titleText = "Your Apartment";
        private string _moneyText;
        private string _instructions = "Click Phone to get job | Click Door to go to job | Click Bed to advance time";

        public override void LoadContent()
        {
            // Create a 1x1 white pixel texture for drawing rectangles
            _pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });

            // Load Background
            _background = TextureManager.Instance.GetBackground("home_smith");

            // Load SpriteFont
            _font = Game.Content.Load<SpriteFont>("DefaultFont");

            CreateInteractables();
        }

        private void CreateInteractables()
        {
            _interactables.Clear();

            // Laptop - Shop
            var laptopButton = new InteractableObject(
                new Rectangle(200, 450, 200, 150),
                "Laptop - Shop",
                OnLaptopClicked
            )
            {
                IdleColor = Color.Transparent,
                HoverColor = new Color(255, 255, 255, 50)
            };
            _interactables.Add(laptopButton);
            
            // Debug items (Keep visible for now or make transparent too)
            var debugMoney = new InteractableObject(new Rectangle(50, 650, 150, 50), "DEBUG: +$100")
            {
                 IdleColor = new Color(0, 0, 0, 100),
                 HoverColor = new Color(0, 0, 0, 200)
            };
            debugMoney.OnClick += () => {
                GameStateManager.Instance.AddMoney(100);
                System.Console.WriteLine($"Money: {GameStateManager.Instance.Money}");
            };
            _interactables.Add(debugMoney);
        }

        private void OnPhoneClicked()
        {
            System.Console.WriteLine("Phone clicked!");
            
            try
            {
                // Check if there's an available job
                if (JobManager.Instance.AvailableJobs.Count > 0 && JobManager.Instance.CurrentJob == null)
                {
                    var job = JobManager.Instance.AvailableJobs[0];
                    System.Console.WriteLine($"Accepting job: {job.Name}");
                    JobManager.Instance.AcceptJob(job);
                    _needsRefresh = true; // Set flag instead of calling CreateInteractables directly
                }
                else
                {
                    System.Console.WriteLine("No available jobs or already have a job");
                }
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"Exception in OnPhoneClicked: {ex.Message}");
            }
        }

        private void OnBedClicked()
        {
            // Advance time
            GameStateManager.Instance.AdvanceTime();
            
            // If it's night and we have a completed day job, go to night infiltration
            if (GameStateManager.Instance.TimeOfDay == TimeOfDay.Night && 
                JobManager.Instance.CurrentJob != null)
            {
                Game.SceneManager.ChangeScene(new NightInfiltrationScene());
            }
        }

        private void OnDoorClicked()
        {
            // Go to job site during day
            if (GameStateManager.Instance.TimeOfDay == TimeOfDay.Day && 
                JobManager.Instance.CurrentJob != null)
            {
                Game.SceneManager.ChangeScene(new JobSiteScene());
            }
        }
        
        private void OnLaptopClicked()
        {
            Game.SceneManager.ChangeScene(new ShopScene());
        }

        public override void Update(GameTime gameTime)
        {
            InputManager.Instance.Update();

            // Update all interactables
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

            // Update money display
            _moneyText = $"Money: ${GameStateManager.Instance.Money}";
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            SpriteBatch.Begin();

            // Draw Background (stretched to fit 1600x900)
            if (_background != null)
            {
                SpriteBatch.Draw(_background, new Rectangle(0, 0, 1600, 900), Color.White);
            }
            
            // Draw title and info at top
            SpriteBatch.DrawString(_font, _titleText, new Vector2(600, 20), Color.White);
            SpriteBatch.DrawString(_font, _moneyText ?? "$0", new Vector2(50, 20), Color.Yellow);
            SpriteBatch.DrawString(_font, $"Day {GameStateManager.Instance.CurrentDay} - {GameStateManager.Instance.TimeOfDay}", 
                new Vector2(1300, 20), Color.Cyan);

            // Draw interactables
            foreach (var interactable in _interactables)
            {
                interactable.DrawRectangle(SpriteBatch, _pixelTexture);
                
                // Draw labels only on hover for cleaner UI
                if (interactable.IsHovered)
                {
                    var textSize = _font.MeasureString(interactable.Name);
                    // Draw text background for readability
                    var textBg = new Rectangle(
                        interactable.Bounds.Center.X - (int)textSize.X / 2 - 5,
                        interactable.Bounds.Center.Y - (int)textSize.Y / 2 - 5,
                        (int)textSize.X + 10,
                        (int)textSize.Y + 10
                    );
                    SpriteBatch.Draw(_pixelTexture, textBg, new Color(0, 0, 0, 200));

                    var textPos = new Vector2(
                        interactable.Bounds.Center.X - textSize.X / 2,
                        interactable.Bounds.Center.Y - textSize.Y / 2
                    );
                    SpriteBatch.DrawString(_font, interactable.Name, textPos, Color.White);
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
