using System.Collections.Generic;

namespace LifeSmith.Dialog
{
    /// <summary>
    /// Character relationship and trust tracking
    /// </summary>
    public class CharacterRelationship
    {
        public string CharacterName { get; set; }
        public float TrustLevel { get; set; } // 0.0 to 100.0
        public float Attraction { get; set; } // 0.0 to 100.0
        public Dictionary<string, bool> Flags { get; set; } // Story flags
        
        public CharacterRelationship(string name)
        {
            CharacterName = name;
            TrustLevel = 50f; // Start neutral
            Attraction = 20f; // Start low
            Flags = new Dictionary<string, bool>();
        }

        public void ModifyTrust(float amount)
        {
            TrustLevel += amount;
            if (TrustLevel < 0) TrustLevel = 0;
            if (TrustLevel > 100) TrustLevel = 100;
        }

        public void ModifyAttraction(float amount)
        {
            Attraction += amount;
            if (Attraction < 0) Attraction = 0;
            if (Attraction > 100) Attraction = 100;
        }

        public void SetFlag(string flagName, bool value)
        {
            Flags[flagName] = value;
        }

        public bool GetFlag(string flagName)
        {
            return Flags.ContainsKey(flagName) && Flags[flagName];
        }

        public string GetTrustLevel()
        {
            if (TrustLevel >= 80) return "Very High";
            if (TrustLevel >= 60) return "High";
            if (TrustLevel >= 40) return "Neutral";
            if (TrustLevel >= 20) return "Low";
            return "Very Low";
        }

        public string GetAttractionLevel()
        {
            if (Attraction >= 80) return "Loves You";
            if (Attraction >= 60) return "Very Attracted";
            if (Attraction >= 40) return "Interested";
            if (Attraction >= 20) return "Neutral";
            return "Not Interested";
        }
    }
}
