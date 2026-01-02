using System;
using System.Collections.Generic;
using LifeSmith.Dialog;

namespace LifeSmith.Core
{
    public enum TimeOfDay
    {
        Day,
        Night
    }

    public class GameStateManager
    {
        private static GameStateManager _instance;
        public static GameStateManager Instance => _instance ??= new GameStateManager();

        // Game progression
        public int CurrentDay { get; set; } = 1;
        public TimeOfDay TimeOfDay { get; set; } = TimeOfDay.Day;

        // Player stats
        public int Money { get; set; } = 50; // Starting money
        public List<string> Inventory { get; set; } = new();
        public HashSet<string> UnlockedTools { get; set; } = new();
        
        // Job tracking
        public string CurrentJobHouseId { get; set; }
        public List<string> CompletedJobs { get; set; } = new();
        
        // Entry points prepared for current house
        public Dictionary<string, bool> PreparedEntryPoints { get; set; } = new();
        
        // Dialog system
        public DialogManager DialogManager { get; private set; }

        private GameStateManager() 
        {
            DialogManager = new DialogManager();
            DialogManager.CreateEmilyDialog(); // Initialize Emily's dialog
        }

        public void AdvanceTime()
        {
            if (TimeOfDay == TimeOfDay.Day)
            {
                TimeOfDay = TimeOfDay.Night;
            }
            else
            {
                TimeOfDay = TimeOfDay.Day;
                CurrentDay++;
            }
        }

        public void AddMoney(int amount)
        {
            Money += amount;
        }

        public bool SpendMoney(int amount)
        {
            if (Money >= amount)
            {
                Money -= amount;
                return true;
            }
            return false;
        }

        public void UnlockTool(string toolName)
        {
            UnlockedTools.Add(toolName);
        }

        public bool HasTool(string toolName)
        {
            return UnlockedTools.Contains(toolName);
        }

        public void PrepareEntryPoint(string entryPointType)
        {
            PreparedEntryPoints[entryPointType] = true;
        }

        public void ClearEntryPoints()
        {
            PreparedEntryPoints.Clear();
        }

        public void CopyFrom(GameStateManager other)
        {
            CurrentDay = other.CurrentDay;
            TimeOfDay = other.TimeOfDay;
            Money = other.Money;
            
            Inventory = new List<string>(other.Inventory);
            UnlockedTools = new HashSet<string>(other.UnlockedTools);
            
            CurrentJobHouseId = other.CurrentJobHouseId;
            CompletedJobs = new List<string>(other.CompletedJobs);
            PreparedEntryPoints = new Dictionary<string, bool>(other.PreparedEntryPoints);
            
            // Dialog state (If needed, copy relationship data here)
            // Ideally DialogManager should have its own Save/Load logic or expose data structure
        }
    }
}
