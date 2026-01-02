using System.Collections.Generic;
using LifeSmith.Systems;

namespace LifeSmith.Content
{
    public class House
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        
        // Character info
        public string ResidentName { get; set; }
        public string ResidentDescription { get; set; }
        
        // Security and loot
        public int LockDifficulty { get; set; }
        public int MoneyReward { get; set; }
        public List<string> ValuableItems { get; set; } = new();
        
        // Entry points for night infiltration
        public List<EntryPoint> EntryPoints { get; set; } = new();
        
        // Clickable items discovered during day
        public List<string> DiscoveredItems { get; set; } = new();

        public House(string id, string name, string address, int lockDifficulty)
        {
            Id = id;
            Name = name;
            Address = address;
            LockDifficulty = lockDifficulty;
            
            // Initialize entry points with different success rates
            EntryPoints.Add(new EntryPoint(EntryPointType.Window, "Window", 0.5f)); // 50% chance if prepared
            EntryPoints.Add(new EntryPoint(EntryPointType.Padlock, "Back Padlock", 0.7f)); // 70% chance
            EntryPoints.Add(new EntryPoint(EntryPointType.SkyLight, "Skylight", 0.9f)); // 90% chance - safer but harder to discover
        }
    }
}
