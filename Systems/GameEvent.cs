using System.Collections.Generic;

namespace LifeSmith.Systems
{
    public class GameEvent
    {
        public string Id { get; set; }
        public string TriggerType { get; set; } // "OnSceneEnter", "OnInteract", "Auto"
        public string TargetId { get; set; } // Which object/scene triggers this (e.g. "MainDoor")
        
        // Conditions
        public string RequiredTime { get; set; } // "Any", "Day", "Night"
        public int RequiredTrust { get; set; } = 0;
        public string RequiredItem { get; set; }
        public string RequiredFlag { get; set; } // A specific story flag like "met_emily"
        
        // Actions
        public string ActionType { get; set; } // "Dialog", "ChangeScene", "AddItem"
        public string ActionValue { get; set; } // Dialog ID, Scene Name, Item ID
        
        public bool IsRepeatable { get; set; } = false;
    }
}
