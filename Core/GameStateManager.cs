using System;
using System.Collections.Generic;
using LifeSmith.Dialog;
using LifeSmith.Systems;

namespace LifeSmith.Core
{
    // Old enum for backward compatibility (will be removed with unused scenes)
    public enum TimeOfDay
    {
        Day,
        Night
    }

    public enum TimeSlot
    {
        Morning,      // Time to go shopping or prepare
        Afternoon,    // Can visit shop or rest
        Evening,      // Ayako visits for tutoring (main gameplay)
        Night         // Day ends, advance to next morning
    }

    public class GameStateManager
    {
        private static GameStateManager _instance;
        public static GameStateManager Instance => _instance ??= new GameStateManager();

        // Game progression
        public int CurrentDay { get; set; } = 1;
        public TimeSlot CurrentTimeSlot { get; set; } = TimeSlot.Morning;
        
        // Player resources
        public int Money { get; set; } = 1000; // Starting money (¥1000)
        public List<string> Inventory { get; set; } = new();
        public List<string> GivenItems { get; set; } = new(); // Items given to Ayako
        
        // Stub properties for backward compatibility (old job system - will be removed with unused scenes)
        public Dictionary<string, bool> PreparedEntryPoints { get; set; } = new();
        public TimeOfDay TimeOfDay { get; set; } = TimeOfDay.Day; // Kept for backward compatibility
        public string CurrentJobHouseId { get; set; }
        public List<string> CompletedJobs { get; set; } = new();
        
        // Student (Ayako) stats
        public StudentStats AyakoStats { get; set; } = new();
        
        // Action point system (for evening tutoring sessions)
        public int ActionsRemaining { get; set; } = 3;
        public int BaseActionsPerEvening { get; set; } = 3;
        
        // Event tracking
        public HashSet<string> CompletedEvents { get; set; } = new();
        
        // Study boost from items
        public int CurrentStudyBoost { get; set; } = 0;
        
        // Dialog system
        public DialogManager DialogManager { get; private set; }

        private GameStateManager() 
        {
            DialogManager = new DialogManager();
            DialogManager.CreateEmilyDialog(); // Initialize Emily's dialog
        }

        public void AdvanceTime()
        {
            switch (CurrentTimeSlot)
            {
                case TimeSlot.Morning:
                    CurrentTimeSlot = TimeSlot.Afternoon;
                    break;
                case TimeSlot.Afternoon:
                    CurrentTimeSlot = TimeSlot.Evening;
                    // Reset action points for evening tutoring session
                    ActionsRemaining = BaseActionsPerEvening;
                    // Apply any extra actions from items (coffee, energy drinks)
                    if (Inventory.Contains("Coffee"))
                    {
                        ActionsRemaining += 1;
                        Inventory.Remove("Coffee");
                    }
                    if (Inventory.Contains("EnergyDrink"))
                    {
                        ActionsRemaining += 2;
                        Inventory.Remove("EnergyDrink");
                    }
                    break;
                case TimeSlot.Evening:
                    CurrentTimeSlot = TimeSlot.Night;
                    break;
                case TimeSlot.Night:
                    CurrentTimeSlot = TimeSlot.Morning;
                    CurrentDay++;
                    // Reset daily variables
                    CurrentStudyBoost = 0;
                    break;
            }
        }

        public void UseAction()
        {
            if (ActionsRemaining > 0)
            {
                ActionsRemaining--;
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

        public void GiveItemToAyako(string itemName)
        {
            if (!GivenItems.Contains(itemName))
            {
                GivenItems.Add(itemName);
            }
        }

        public bool HasGivenItem(string itemName)
        {
            return GivenItems.Contains(itemName);
        }

        public void MarkEventComplete(string eventId)
        {
            CompletedEvents.Add(eventId);
        }

        public bool IsEventComplete(string eventId)
        {
            return CompletedEvents.Contains(eventId);
        }

        // Stub methods for backward compatibility (old job system - will be removed)
        public void PrepareEntryPoint(string entryPointType) { }
        public void ClearEntryPoints() { }
        public void UnlockTool(string toolName) { }
        public bool HasTool(string toolName) { return false; }

        public void CopyFrom(GameStateManager other)
        {
            CurrentDay = other.CurrentDay;
            CurrentTimeSlot = other.CurrentTimeSlot;
            Money = other.Money;
            
            Inventory = new List<string>(other.Inventory);
            GivenItems = new List<string>(other.GivenItems);
            
            CompletedEvents = new HashSet<string>(other.CompletedEvents);
            
            ActionsRemaining = other.ActionsRemaining;
            BaseActionsPerEvening = other.BaseActionsPerEvening;
            CurrentStudyBoost = other.CurrentStudyBoost;
            
            // Deep copy student stats
            AyakoStats.Grade = other.AyakoStats.Grade;
            AyakoStats.Affection = other.AyakoStats.Affection;
            AyakoStats.Lust = other.AyakoStats.Lust;
            AyakoStats.Suspicion = other.AyakoStats.Suspicion;
            AyakoStats.Money = other.AyakoStats.Money;
        }
    }
}
