using System.Collections.Generic;
using System.Linq;
using LifeSmith.Core;

namespace LifeSmith.Systems
{
    public class EventManager
    {
        private static EventManager _instance;
        public static EventManager Instance => _instance ??= new EventManager();

        private List<GameEvent> _allEvents;
        private HashSet<string> _triggeredEvents = new HashSet<string>();

        private EventManager()
        {
            LoadEvents();
        }

        private void LoadEvents()
        {
            _allEvents = new List<GameEvent>();
            // Placeholder: In future, load from JSON
            // Example: Emily greets you first time
            _allEvents.Add(new GameEvent 
            {
                Id = "evt_intro_emily",
                TriggerType = "OnInteract",
                TargetId = "Door",
                RequiredTime = "Day",
                ActionType = "Dialog",
                ActionValue = "emily_greeting",
                IsRepeatable = false
            });
        }

        public GameEvent CheckForEvent(string triggerType, string targetId)
        {
            foreach (var evt in _allEvents)
            {
                // Check basic triggers
                if (evt.TriggerType != triggerType) continue;
                if (!string.IsNullOrEmpty(evt.TargetId) && evt.TargetId != targetId) continue;
                
                // Check history
                if (!evt.IsRepeatable && _triggeredEvents.Contains(evt.Id)) continue;

                // Check Conditions
                if (!CheckConditions(evt)) continue;

                return evt;
            }
            return null;
        }

        private bool CheckConditions(GameEvent evt)
        {
            // Time Condition
            if (!string.IsNullOrEmpty(evt.RequiredTime) && evt.RequiredTime != "Any")
            {
                if (evt.RequiredTime == "Day" && GameStateManager.Instance.TimeOfDay != TimeOfDay.Day) return false;
                if (evt.RequiredTime == "Night" && GameStateManager.Instance.TimeOfDay != TimeOfDay.Night) return false;
            }

            // Item Condition
            if (!string.IsNullOrEmpty(evt.RequiredItem))
            {
                if (!GameStateManager.Instance.HasTool(evt.RequiredItem) && 
                    !GameStateManager.Instance.Inventory.Contains(evt.RequiredItem)) 
                    return false;
            }

            return true;
        }

        public void ExecuteEvent(GameEvent evt)
        {
            System.Console.WriteLine($"Executing Event: {evt.Id}");
            
            if (!evt.IsRepeatable)
                _triggeredEvents.Add(evt.Id);

            switch (evt.ActionType)
            {
                case "Dialog":
                    // Dialog request
                    System.Console.WriteLine($"Event Triggered Dialog: {evt.ActionValue}");
                    break;
                    
                case "AddItem":
                    GameStateManager.Instance.Inventory.Add(evt.ActionValue);
                    System.Console.WriteLine($"Added item: {evt.ActionValue}");
                    break;
            }
        }
    }
}
