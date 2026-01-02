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
        public bool IsPrepared { get; set; }
        public float BaseSuccessChance { get; set; }
        public bool IsDiscovered { get; set; }

        public EntryPoint(EntryPointType type, string name, float baseSuccessChance)
        {
            Type = type;
            Name = name;
            BaseSuccessChance = baseSuccessChance;
            IsPrepared = false;
            IsDiscovered = false;
        }

        public bool AttemptEntry(Random random)
        {
            if (!IsPrepared)
                return false;

            // Roll RNG against success chance
            double roll = random.NextDouble();
            return roll <= BaseSuccessChance;
        }

        public void Prepare()
        {
            IsPrepared = true;
        }

        public void Discover()
        {
            IsDiscovered = true;
        }
    }
}
