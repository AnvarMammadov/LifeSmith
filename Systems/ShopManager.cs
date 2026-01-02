using System.Collections.Generic;
using LifeSmith.Core;

namespace LifeSmith.Systems
{
    public class ShopManager
    {
        private static ShopManager _instance;
        public static ShopManager Instance => _instance ??= new ShopManager();

        public List<ShopItem> AvailableItems { get; private set; }

        private ShopManager()
        {
            InitializeItems();
        }

        private void InitializeItems()
        {
            AvailableItems = new List<ShopItem>
            {
                new ShopItem("advanced_lockpick", "Advanced Lockpick Set", 150, "Wider sweet spot tolerance. Makes lockpicking easier."),
                new ShopItem("flashlight", "Tactical Flashlight", 100, "Better visibility during night infiltration."),
                new ShopItem("tension_wrench", "Titanium Tension Wrench", 200, "Prevents lockpicks from breaking easily (More time)."),
                new ShopItem("skeleton_key", "Skeleton Key (Placeholder)", 500, "Instantly opens simple locks. (Consumable)", false),
                new ShopItem("camera", "Mini Spy Camera", 300, "Gather more intel during day visits.")
            };
        }

        public bool BuyItem(string itemId)
        {
            var item = AvailableItems.Find(i => i.Id == itemId);
            if (item == null) return false;

            // Check if player already has it (if one-time purchase)
            if (item.IsOneTimePurchase && GameStateManager.Instance.HasTool(itemId))
                return false;

            // Check money
            if (GameStateManager.Instance.SpendMoney(item.Price))
            {
                if (item.IsOneTimePurchase)
                {
                    GameStateManager.Instance.UnlockTool(itemId);
                }
                else
                {
                    GameStateManager.Instance.Inventory.Add(itemId);
                }
                return true;
            }

            return false;
        }
    }
}
