using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using LifeSmith.Core;
using LifeSmith.Systems;
using LifeSmith.Scenes;

namespace LifeSmith.UI
{
    /// <summary>
    /// Visual novel-style navigation bar with properly spaced buttons.
    /// No overlapping, clean modern design for 1600x900 resolution.
    /// </summary>
    public class NavigationBarUI
    {
        private UIPanel _panel;
        private List<UIButton> _buttons = new();
        private Game1 _game;
        private SpriteFont _font;

        // Layout constants for 1600x900
        private const int PANEL_HEIGHT = 120;
        private const int PANEL_Y = 900 - PANEL_HEIGHT; // 780
        private const int BUTTON_WIDTH = 180;
        private const int BUTTON_HEIGHT = 80;
        private const int BUTTON_GAP = 30;
        private const int LEFT_MARGIN = 40;

        public Action OnInventoryToggle;

        public NavigationBarUI(Game1 game, SpriteFont font)
        {
            _game = game;
            _font = font;
            var device = game.GraphicsDevice;

            // Create bottom panel with gradient
            _panel = new UIPanel(device, new Rectangle(0, PANEL_Y, 1600, PANEL_HEIGHT));
            _panel.UseGradient = true;
            _panel.GradientTopColor = new Color(25, 30, 40, 240);
            _panel.GradientBottomColor = new Color(15, 20, 30, 250);
            _panel.BorderColor = new Color(70, 80, 100, 255);
            _panel.BorderThickness = 2;

            CreateButtons();
        }

        private void CreateButtons()
        {
            int yPos = PANEL_Y + 20; // 20px from top of panel
            int currentX = LEFT_MARGIN;

            // 1. Phone Button
            var phoneBtn = new UIButton(_game.GraphicsDevice, _font, 
                new Rectangle(currentX, yPos, BUTTON_WIDTH, BUTTON_HEIGHT), 
                "Phone");
            phoneBtn.OnClick += OnPhoneClicked;
            phoneBtn.IdleColor = new Color(45, 55, 75, 220);
            phoneBtn.HoverColor = new Color(65, 80, 110, 240);
            _buttons.Add(phoneBtn);
            currentX += BUTTON_WIDTH + BUTTON_GAP;

            // 2. Map Button
            var mapBtn = new UIButton(_game.GraphicsDevice, _font, 
                new Rectangle(currentX, yPos, BUTTON_WIDTH, BUTTON_HEIGHT), 
                "Map");
            mapBtn.OnClick += OnMapClicked;
            mapBtn.IdleColor = new Color(45, 55, 75, 220);
            mapBtn.HoverColor = new Color(65, 80, 110, 240);
            _buttons.Add(mapBtn);
            currentX += BUTTON_WIDTH + BUTTON_GAP;

            // 3. Inventory Button
            var invBtn = new UIButton(_game.GraphicsDevice, _font, 
                new Rectangle(currentX, yPos, BUTTON_WIDTH, BUTTON_HEIGHT), 
                "Inventory");
            invBtn.OnClick += () => OnInventoryToggle?.Invoke();
            invBtn.IdleColor = new Color(55, 45, 75, 220);
            invBtn.HoverColor = new Color(80, 65, 110, 240);
            _buttons.Add(invBtn);
            currentX += BUTTON_WIDTH + BUTTON_GAP;

            // 4. Wait Button
            var waitBtn = new UIButton(_game.GraphicsDevice, _font, 
                new Rectangle(currentX, yPos, BUTTON_WIDTH, BUTTON_HEIGHT), 
                "Wait (+Time)");
            waitBtn.OnClick += OnWaitClicked;
            waitBtn.IdleColor = new Color(45, 55, 75, 220);
            waitBtn.HoverColor = new Color(65, 80, 110, 240);
            _buttons.Add(waitBtn);
        }

        private void OnPhoneClicked()
        {
            // Accept first available job
            if (JobManager.Instance.AvailableJobs.Count > 0 && JobManager.Instance.CurrentJob == null)
            {
                var job = JobManager.Instance.AvailableJobs[0];
                JobManager.Instance.AcceptJob(job);
                System.Console.WriteLine($"Accepted job: {job.Name}");
            }
            else
            {
                System.Console.WriteLine("No jobs available or job already active.");
            }
        }

        private void OnMapClicked()
        {
            // Toggle between Home and Job
            if (_game.SceneManager.CurrentScene is PlayerApartmentScene)
            {
                if (JobManager.Instance.CurrentJob != null)
                    _game.SceneManager.ChangeScene(new JobSiteScene());
                else
                    System.Console.WriteLine("No job to travel to.");
            }
            else
            {
                _game.SceneManager.ChangeScene(new PlayerApartmentScene());
            }
        }

        private void OnWaitClicked()
        {
            GameStateManager.Instance.AdvanceTime();
            
            // Check for night transition
            if (GameStateManager.Instance.TimeOfDay == TimeOfDay.Night && 
                JobManager.Instance.CurrentJob != null &&
                _game.SceneManager.CurrentScene is PlayerApartmentScene)
            {
                _game.SceneManager.ChangeScene(new NightInfiltrationScene());
            }
        }

        public void Update()
        {
            foreach (var btn in _buttons)
            {
                btn.Update();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw panel background
            _panel.Draw(spriteBatch);

            // Draw all buttons
            foreach (var btn in _buttons)
            {
                btn.Draw(spriteBatch);
            }

            // Draw time/day info on the right side of the panel
            string timeInfo = $"Day {GameStateManager.Instance.CurrentDay} - {GameStateManager.Instance.TimeOfDay}";
            Vector2 timeSize = _font.MeasureString(timeInfo);
            Vector2 timePos = new Vector2(1600 - timeSize.X - 40, PANEL_Y + 20);
            spriteBatch.DrawString(_font, timeInfo, timePos, Color.Cyan);

            // Draw money below time
            string moneyInfo = $"${GameStateManager.Instance.Money}";
            Vector2 moneySize = _font.MeasureString(moneyInfo);
            Vector2 moneyPos = new Vector2(1600 - moneySize.X - 40, PANEL_Y + 50);
            spriteBatch.DrawString(_font, moneyInfo, moneyPos, Color.LightGreen);
        }

        public void Dispose()
        {
            _panel?.Dispose();
            foreach (var btn in _buttons)
            {
                btn.Dispose();
            }
        }
    }
}
