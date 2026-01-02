using System;

namespace LifeSmith.Systems
{
    public enum EntryPointType
    {
        Window,
        BackDoor,
        SkyLight,
        Padlock
    }

    public class EntryPoint
    {
        public EntryPointType Type { get; set; }
        public string Name { get; set; }
        public float BaseSuccessChance { get; set; }
        
        // IsPrepared flag should be linked to persistent GameState
        // For now, simpler auto-property, but we must ensure it's synced with GameStateManager
        public bool IsPrepared 
        { 
            get 
            {
                return LifeSmith.Core.GameStateManager.Instance.PreparedEntryPoints.ContainsKey(Name);
            }
        }
        
        public bool IsDiscovered { get; private set; } = false;

        public EntryPoint(EntryPointType type, string name, float chance)
        {
            Type = type;
            Name = name;
            BaseSuccessChance = chance;
        }
        
        public void Discover()
        {
            IsDiscovered = true;
        }
        
        public void Prepare()
        {
            LifeSmith.Core.GameStateManager.Instance.PrepareEntryPoint(Name);
        }

        public bool AttemptEntry(Random rng)
        {
            // If prepared, use base chance. If not, failed (or very low chance?)
            // Design: Must be prepared to even try? Or prepared = higher chance?
            
            // Current Logic: Only prepared entries appear in NightScene list.
            if (!IsPrepared) return false;

            double roll = rng.NextDouble();
            // Simple RNG: Roll < Chance = Success
            return roll < BaseSuccessChance;
        }
    }
}
