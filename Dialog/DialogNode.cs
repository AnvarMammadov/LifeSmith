using System.Collections.Generic;
using System.Linq;

namespace LifeSmith.Dialog
{
    /// <summary>
    /// Represents a single dialog node in the conversation tree
    /// </summary>
    public class DialogNode
    {
        public string Id { get; set; }
        public string CharacterName { get; set; }
        public string Text { get; set; }
        public string Expression { get; set; } // "happy", "sad", "angry", "neutral", "flirty", etc.
        public List<DialogChoice> Choices { get; set; }
        public bool IsEndNode { get; set; }
        
        public DialogNode(string id, string characterName, string text, string expression = "neutral")
        {
            Id = id;
            CharacterName = characterName;
            Text = text;
            Expression = expression;
            Choices = new List<DialogChoice>();
            IsEndNode = false;
        }

        public void AddChoice(DialogChoice choice)
        {
            Choices.Add(choice);
        }

        public List<DialogChoice> GetAvailableChoices(CharacterRelationship relationship)
        {
            return Choices.Where(c => c.IsAvailable(relationship)).ToList();
        }
    }
}
