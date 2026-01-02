namespace LifeSmith.Systems
{
    public class ShopItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; } // Placeholder for icon name
        public bool IsOneTimePurchase { get; set; } // e.g. tools vs consumables
        
        public ShopItem(string id, string name, int price, string description, bool isOneTime = true)
        {
            Id = id;
            Name = name;
            Price = price;
            Description = description;
            IsOneTimePurchase = isOneTime;
        }
    }
}
