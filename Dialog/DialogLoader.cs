using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LifeSmith.Dialog
{
    // DTO classes for JSON deserialization
    public class DialogData
    {
        [JsonPropertyName("characterId")]
        public string CharacterId { get; set; }

        [JsonPropertyName("rootNodeId")]
        public string RootNodeId { get; set; }

        [JsonPropertyName("nodes")]
        public List<DialogNodeData> Nodes { get; set; }
    }

    public class DialogNodeData
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("characterName")]
        public string CharacterName { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("expression")]
        public string Expression { get; set; }

        [JsonPropertyName("isEndNode")]
        public bool IsEndNode { get; set; }

        [JsonPropertyName("choices")]
        public List<DialogChoiceData> Choices { get; set; }
    }

    public class DialogChoiceData
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("nextNodeId")]
        public string NextNodeId { get; set; }

        [JsonPropertyName("trustModifier")]
        public float TrustModifier { get; set; }

        [JsonPropertyName("attractionModifier")]
        public float AttractionModifier { get; set; }
        
        // Add flags support later if needed in JSON
    }

    /// <summary>
    /// Loads dialogs from JSON files
    /// </summary>
    public class DialogLoader
    {
        public static List<DialogNode> LoadDialogFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                System.Diagnostics.Debug.WriteLine($"Dialog file not found: {filePath}");
                return new List<DialogNode>();
            }

            try
            {
                string jsonString = File.ReadAllText(filePath);
                DialogData dialogData = JsonSerializer.Deserialize<DialogData>(jsonString);
                
                List<DialogNode> convertNodes = new List<DialogNode>();
                string characterPrefix = dialogData.CharacterId + "_"; // Prefix IDs to avoid collisions

                foreach (var nodeData in dialogData.Nodes)
                {
                    // Map DTO to runtime object
                    // We can choose to use simple IDs like "greeting" in JSON, 
                    // but map them to "emily_greeting" in the engine using the characterId prefix if needed.
                    // For this implementation let's try to stick to the IDs provided in JSON 
                    // assuming the JSON author manages uniqueness or we prefix them.
                    
                    // Let's use the ID from JSON directly for now based on the existing hardcoded example
                    // But usually prefixing is safer.
                    
                    // NOTE: The JSON I wrote uses simple IDs like "greeting". 
                    // The code expected "emily_greeting". 
                    // Let's implement prefixing logic to make it robust.
                    
                    string fullNodeId = dialogData.CharacterId + "_" + nodeData.Id;
                    
                    DialogNode node = new DialogNode(fullNodeId, nodeData.CharacterName, nodeData.Text, nodeData.Expression);
                    node.IsEndNode = nodeData.IsEndNode;

                    if (nodeData.Choices != null)
                    {
                        foreach (var choiceData in nodeData.Choices)
                        {
                            string nextNodeId = dialogData.CharacterId + "_" + choiceData.NextNodeId;
                            
                            DialogChoice choice = new DialogChoice(
                                choiceData.Text,
                                nextNodeId,
                                choiceData.TrustModifier,
                                choiceData.AttractionModifier
                            );
                            node.AddChoice(choice);
                        }
                    }
                    
                    convertNodes.Add(node);
                }

                return convertNodes;
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading dialog JSON: {ex.Message}");
                return new List<DialogNode>();
            }
        }
    }
}
