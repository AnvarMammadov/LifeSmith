using System;
using System.Collections.Generic;

namespace LifeSmith.Dialog
{
    /// <summary>
    /// A single choice in a dialog
    /// </summary>
    public class DialogChoice
    {
        public string Text { get; set; }
        public string NextNodeId { get; set; }
        public float TrustModifier { get; set; } // How much this choice affects trust
        public float AttractionModifier { get; set; } // How much this choice affects attraction
        public Dictionary<string, bool> RequiredFlags { get; set; } // Flags needed to show this choice
        public Dictionary<string, bool> SetFlags { get; set; } // Flags to set when chosen
        
        public DialogChoice(string text, string nextNodeId, float trustMod = 0f, float attractionMod = 0f)
        {
            Text = text;
            NextNodeId = nextNodeId;
            TrustModifier = trustMod;
            AttractionModifier = attractionMod;
            RequiredFlags = new Dictionary<string, bool>();
            SetFlags = new Dictionary<string, bool>();
        }

        public bool IsAvailable(CharacterRelationship relationship)
        {
            foreach (var flag in RequiredFlags)
            {
                if (relationship.GetFlag(flag.Key) != flag.Value)
                    return false;
            }
            return true;
        }

        public void ApplyEffects(CharacterRelationship relationship)
        {
            relationship.ModifyTrust(TrustModifier);
            relationship.ModifyAttraction(AttractionModifier);
            
            foreach (var flag in SetFlags)
            {
                relationship.SetFlag(flag.Key, flag.Value);
            }
        }
    }
}
