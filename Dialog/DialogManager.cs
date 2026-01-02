using System.Collections.Generic;
using System.Linq;

namespace LifeSmith.Dialog
{
    /// <summary>
    /// Manages dialog trees and character relationships
    /// </summary>
    public class DialogManager
    {
        private Dictionary<string, DialogNode> _nodes;
        private Dictionary<string, CharacterRelationship> _relationships;
        
        public DialogManager()
        {
            _nodes = new Dictionary<string, DialogNode>();
            _relationships = new Dictionary<string, CharacterRelationship>();
        }

        public void AddNode(DialogNode node)
        {
            _nodes[node.Id] = node;
        }

        public DialogNode GetNode(string nodeId)
        {
            return _nodes.ContainsKey(nodeId) ? _nodes[nodeId] : null;
        }

        public CharacterRelationship GetRelationship(string characterName)
        {
            if (!_relationships.ContainsKey(characterName))
            {
                _relationships[characterName] = new CharacterRelationship(characterName);
            }
            return _relationships[characterName];
        }

        public void ApplyChoice(DialogChoice choice, string characterName)
        {
            var relationship = GetRelationship(characterName);
            choice.ApplyEffects(relationship);
        }

        // Initialize dialogs
        public void LoadAllDialogs()
        {
            // Path relative to execution directory
            string dialogsDir = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Content", "Dialogs");
            
            // Ensure directory exists
            if (!System.IO.Directory.Exists(dialogsDir))
            {
                System.Console.WriteLine($"Dialog directory not found: {dialogsDir}, creating it.");
                System.IO.Directory.CreateDirectory(dialogsDir);
            }

            if (System.IO.Directory.Exists(dialogsDir))
            {
                var files = System.IO.Directory.GetFiles(dialogsDir, "*.json");
                System.Console.WriteLine($"Found {files.Length} dialog files.");
                
                foreach (var file in files)
                {
                    System.Console.WriteLine($"Loading dialog: {file}");
                    var nodes = DialogLoader.LoadDialogFromFile(file);
                    foreach (var node in nodes)
                    {
                        AddNode(node);
                    }
                }
            }
        }

        // Kept for compatibility but now loads from JSON
        public void CreateEmilyDialog()
        {
            LoadAllDialogs();
            
            // Fallback: If JSON loading failed (empty nodes), create hardcoded version
            if (_nodes.Count == 0)
            {
                System.Console.WriteLine("JSON loading returned no nodes, falling back to hardcoded dialog.");
                CreateHardcodedEmilyDialog();
            }
        }
        
        private void CreateHardcodedEmilyDialog()
        {
             // Opening node
            var node1 = new DialogNode("emily_greeting", "Emily", 
                "Oh, you're the locksmith? Thank you for coming so quickly!", "happy");
            
            node1.AddChoice(new DialogChoice(
                "No problem, happy to help! (Friendly)",
                "emily_friendly",
                trustMod: 5f,
                attractionMod: 2f
            ));
            
            node1.AddChoice(new DialogChoice(
                "It's my job. Let's get started. (Professional)",
                "emily_professional",
                trustMod: 3f,
                attractionMod: 0f
            ));
            
            node1.AddChoice(new DialogChoice(
                "You look lovely today... (Flirty)",
                "emily_flirty",
                trustMod: -2f,
                attractionMod: 5f
            ));
            
            AddNode(node1);

            // Friendly path
            var node2 = new DialogNode("emily_friendly", "Emily",
                "You seem like a nice person! I was really worried about this lock.", "happy");
            
            node2.AddChoice(new DialogChoice(
                "Don't worry, I'll have it open in no time!",
                "emily_work",
                trustMod: 3f
            ));
            
            AddNode(node2);

            // Professional path
            var node3 = new DialogNode("emily_professional", "Emily",
                "Of course. The lock is right here. I really appreciate your help.", "neutral");
            
            node3.AddChoice(new DialogChoice(
                "Let me take a look at it.",
                "emily_work",
                trustMod: 2f
            ));
            
            AddNode(node3);

            // Flirty path
            var node4 = new DialogNode("emily_flirty", "Emily",
                "Oh... um, thank you. *blushes* Should we focus on the lock?", "flirty");
            
            node4.AddChoice(new DialogChoice(
                "You're right, sorry. Let me get to work. (Apologize)",
                "emily_work",
                trustMod: 1f,
                attractionMod: 2f
            ));
            
            node4.AddChoice(new DialogChoice(
                "Of course, but I meant it. (Double down)",
                "emily_bold",
                trustMod: -3f,
                attractionMod: 8f
            ));
            
            AddNode(node4);

            // Bold flirty response
            var node5 = new DialogNode("emily_bold", "Emily",
                "*giggles nervously* You're quite forward... Let's see if you're as good with locks as you are with words.", "flirty");
            
            node5.AddChoice(new DialogChoice(
                "I'm good with my hands in general. (Confident)",
                "emily_work",
                attractionMod: 5f
            ));
            
            AddNode(node5);

            // Work node (end of intro dialog)
            var node6 = new DialogNode("emily_work", "Emily",
                "Alright, I'll let you work. Just call me if you need anything!", "happy");
            node6.IsEndNode = true;
            
            AddNode(node6);
        }
    }
}
