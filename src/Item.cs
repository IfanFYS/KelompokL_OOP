public class Item
{
    public string Name { get; set; }
    public string Type { get; set; } // Weapon, Armor, Consumable
    public int Price { get; set; }
    public int Value { get; set; } // Attack for weapons, Defense for armor, HP for consumables
    public bool IsEquipped { get; set; }

    public Item(string name, string type, int price, int value)
    {
        Name = name;
        Type = type;
        Price = price;
        Value = value;
        IsEquipped = false;
    }

    public string GetDescription()
    {
        string equippedStatus = IsEquipped ? "[Equipped] " : "";
        switch (Type)
        {
            case "Weapon":
                return $"{equippedStatus}{Name} - Attack: +{Value}";
            case "Armor":
                return $"{equippedStatus}{Name} - Defense: +{Value}";
            case "Consumable":
                return $"{Name} - Restores: {Value} HP";
            default:
                return Name;
        }
    }
}