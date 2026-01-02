using System;
using System.IO;
using System.Text.Json;
using LifeSmith.Core;

namespace LifeSmith.Systems
{
    public class SaveManager
    {
        private static string SaveFileName = "savegame.json";
        
        public static void SaveGame()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonString = JsonSerializer.Serialize(GameStateManager.Instance, options);
                
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SaveFileName);
                File.WriteAllText(path, jsonString);
                
                System.Console.WriteLine($"Game saved to: {path}");
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Failed to save game: {ex.Message}");
            }
        }

        public static bool LoadGame()
        {
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SaveFileName);
                if (!File.Exists(path))
                {
                    System.Console.WriteLine("No save file found.");
                    return false;
                }

                string jsonString = File.ReadAllText(path);
                var gameState = JsonSerializer.Deserialize<GameStateManager>(jsonString);
                
                // We need to apply the loaded state to the Singleton instance
                // Since Singleton usually creates itself, we might need a method to overwrite it
                // Or just copy properties. For simplicity, let's copy key properties.
                
                GameStateManager.Instance.CopyFrom(gameState);
                
                // Re-initialize non-serializable parts if any (like DialogManager is re-created but state needs to be synced?)
                // Actually DialogManager state (relationships) is inside GameStateManager so it should be fine if serialized properly.
                
                System.Console.WriteLine("Game loaded successfully.");
                return true;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Failed to load game: {ex.Message}");
                return false;
            }
        }
        
        public static bool HasSaveFile()
        {
           string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SaveFileName);
           return File.Exists(path);
        }
    }
}
