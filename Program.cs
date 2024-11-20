using System;
using System.Collections.Generic;
using System.Linq;

namespace PersonaWrathOfNyx
{
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
    public class Character
    {
        public string Name { get; set; }
        public int HP { get; set; }
        public int MaxHP { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int Level { get; set; }
        public int TartarusEssence { get; set; }
        public int TartarusEssenceOnDeath { get; set; }

        public int Chance { get; set; }
        public int TotalAttack => Attack + (EquippedWeapon?.Value ?? 0);
        public int TotalDefense => Defense + (EquippedArmor?.Value ?? 0);

        public Item EquippedWeapon { get; set; }
        public Item EquippedArmor { get; set; }

        public Character(string name, int hp, int attack, int defense)
        {
            Name = name;
            HP = hp;
            MaxHP = hp;
            Attack = attack;
            Defense = defense;
            Level = 1;
            TartarusEssence = 0;
            TartarusEssenceOnDeath = 0;
            Chance = 0;
            EquippedWeapon = null;
            EquippedArmor = null;
        }
    }
    public class Boss : Character
    {
        public string Title { get; set; }
        public List<string> SpecialMoves { get; set; }

        public Boss(string name, string title, int hp, int attack, int defense) 
            : base(name, hp, attack, defense)
        {
            Title = title;
            SpecialMoves = new List<string>();
        }
    }
    public class Summon : Character
    {
        public string Type { get; set; }
        public List<string> Skills { get; set; }

        public Summon(string name, string type, int hp, int attack, int defense) 
            : base(name, hp, attack, defense)
        {
            Type = type;
            Skills = new List<string>();
        }
    }
public class PointOfInterest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsExplored { get; set; }
    public List<Character> Enemies { get; set; }
    public List<Item> Items { get; set; }
    public List<Summon> Summons { get; set; }

    public PointOfInterest(string name, string description)
    {
        Name = name;
        Description = description;
        IsExplored = false;
        Enemies = new List<Character>();
        Items = new List<Item>();
        Summons = new List<Summon>();
    }
}

 public class Location
{
    public string Name { get; set; }
    public bool IsExplored { get; set; }
    public List<PointOfInterest> PointsOfInterest { get; set; }
    public Boss LevelBoss { get; set; }
    public bool IsBossDefeated { get; set; }

    public Location(string name)
    {
        Name = name;
        IsExplored = false;
        PointsOfInterest = new List<PointOfInterest>();
        LevelBoss = null;
        IsBossDefeated = false;
    }
}

    public class Game
    {
        private Character player;
        private List<Summon> availableSummons;
        private List<Summon> activeParty;
        private List<Location> currentLevel;
        private List<Item> inventory;
        private List<Item> merchantInventory;
        private int currentLocationIndex;
        private bool isInHub;

        public Game()
        {
            InitializeGame();
        }
        private readonly string ENEMY_SPRITE = @"
         .---.
         /     \
        | () () |
         \  ^  /
          |||||
          |||||
        ";

        private readonly string PLAYER_SPRITE = @"
        _____
       /     \
      | ^   ^ |
       \  -  /
        |||||
       //|||\\
        ";

        private readonly string EXPLORE_SPRITE = @"
        /\
       /  \      *
      /____\    / \
        ||     /___\
        ||      ||
        ~~~~~~~~~~~~~~~~
        ";

        private void InitializeGame()
        {
            // Initialize player
            player = new Character("Traveler", 100, 15, 10);
            
            // Initialize available summons
            availableSummons = new List<Summon>
            {
                new Summon("Cerberus", "Hell Hound", 80, 20, 8),
                new Summon("Charon", "Ferryman", 60, 15, 12),
                new Summon("Hecate", "Witch", 70, 25, 5)
            };

            // Initialize active party (starts empty)
            activeParty = new List<Summon>();
            inventory = new List<Item>();
            InitializeMerchantInventory();
            // Initialize first level
            InitializeLevel("Nyx Palace - Entrance");
        }
        private void InitializeMerchantInventory()
        {
            merchantInventory = new List<Item>
            {
                new Item("Iron Sword", "Weapon", 100, 5),
                new Item("Steel Sword", "Weapon", 250, 10),
                new Item("Dark Blade", "Weapon", 500, 15),
                new Item("Leather Armor", "Armor", 150, 5),
                new Item("Chain Mail", "Armor", 300, 10),
                new Item("Shadow Plate", "Armor", 600, 15),
                new Item("Health Potion", "Consumable", 50, 30),
                new Item("Greater Health Potion", "Consumable", 100, 60),
                new Item("Max Health Potion", "Consumable", 200, 100)
            };
        }
private void InitializeLevel(string levelName)
{
    currentLevel = new List<Location>
    {
        new Location($"{levelName} - Hall")
        {
            PointsOfInterest = new List<PointOfInterest>
            {
                new PointOfInterest("Mysterious Statue", "An ancient statue emanating dark energy.")
                {
                    Enemies = new List<Character> { new Character("Shadow Warrior", 50, 12, 8) }
                },
                new PointOfInterest("Hidden Passage", "A narrow corridor leading to unknown depths.")
                {
                    Items = new List<Item> { new Item("Health Potion", "Consumable", 50, 30), new Item("Copper Sword", "Weapon", 40, 5), }
                },
                new PointOfInterest("Summoning Circle", "A magical circle pulsing with power.")
                {
                    Summons = new List<Summon> { new Summon("Orpheus", "Musician", 70, 18, 10) }
                }
            },
            LevelBoss = new Boss("Projection of Thanatos", "The Fake Death", 200, 30, 20)
        },
        new Location($"{levelName} - Cavern")
        {
            PointsOfInterest = new List<PointOfInterest>
            {
                new PointOfInterest("Glowing Crystal", "A large crystal radiating soft light.")
                {
                    Items = new List<Item> { new Item("Mana Elixir", "Consumable", 40, 25) },
                    Enemies = new List<Character> { new Character("Crystal Golem", 80, 15, 12) }
                },
                new PointOfInterest("Echoing Chamber", "A vast chamber where every sound resonates.")
                {
                    Enemies = new List<Character> { new Character("Phantom Echo", 60, 20, 10) }
                },
                new PointOfInterest("Abandoned Camp", "Remains of an old adventurer's campsite.")
                {
                    Items = new List<Item> { new Item("Iron Sword", "Weapon", 100, 5), }
                }
            },
            LevelBoss = new Boss("Stone Guardian", "The Keeper of the Cavern", 250, 35, 18)
        },
        new Location($"{levelName} - Fortress")
        {
            PointsOfInterest = new List<PointOfInterest>
            {
                new PointOfInterest("Watchtower", "A towering structure with an ominous view.")
                {
                    Enemies = new List<Character> { new Character("Dark Archer", 45, 10, 8) }
                },
                new PointOfInterest("Armory", "Filled with weapons of old and new.")
                {
                    Items = new List<Item> { new Item("Iron Shield", "Armor", 30, 20) }
                },
                new PointOfInterest("Prison Cell", "An eerie cell with faint whispering sounds.")
                {
                    Summons = new List<Summon> { new Summon("Wraith", "Spirit", 60, 16, 12) }
                }
            },
            LevelBoss = new Boss("Dark Commander", "Leader of the Shadow Army", 300, 40, 25)
        },
        new Location($"{levelName} - Abyss")
        {
            PointsOfInterest = new List<PointOfInterest>
            {
                new PointOfInterest("Fallen Bridge", "A destroyed bridge over a bottomless pit.")
                {
                    Enemies = new List<Character> { new Character("Abyss Stalker", 70, 18, 14) }
                },
                new PointOfInterest("Shadow Altar", "An altar covered in dark runes.")
                {
                    Items = new List<Item> { new Item("Dark Amulet", "Artifact", 50, 40) },
                    Summons = new List<Summon> { new Summon("Dread Fiend", "Beast", 90, 22, 18) }
                },
                new PointOfInterest("Void Gate", "A swirling portal leading to unknown realms.")
                {
                    Enemies = new List<Character> { new Character("Void Sentinel", 100, 25, 20) }
                }
            },
            LevelBoss = new Boss("Avatar of the Void", "Harbinger of Despair", 400, 50, 30)
        }
    };
    currentLocationIndex = 0;
}

        public void StartGame()
        {
            Console.WriteLine("Welcome to Persona: Wrath of Nyx");
            Console.WriteLine("You are the Traveler, a being carrying fragments of Thanatos...");
            
            while (true)
            {
                DisplayMainMenu();
                string choice = Console.ReadLine();
                ProcessMainMenuChoice(choice);
            }
        }

        private void DisplayMainMenu()
        {
            if (isInHub)
            {
                Console.WriteLine("\n=== FireLink Shrine ===");
                Console.WriteLine($"Tartarus Essence: {player.TartarusEssence}");
                Console.WriteLine($"HP: {player.HP}/{player.MaxHP}");
                Console.WriteLine($"Attack Points: {player.TotalAttack}");
                Console.WriteLine("\n1. Visit Merchant");
                Console.WriteLine("2. Visit Temple of Ascension");
                Console.WriteLine("3. View Inventory");
                Console.WriteLine("4. Return to Quest");
                Console.WriteLine("5. Exit Game");
            }
            else
            {
                Console.WriteLine("\n=== Current Location: " + currentLevel[currentLocationIndex].Name + " ===");
                Console.WriteLine($"Tartarus Essence: {player.TartarusEssence}");
                Console.WriteLine($"HP: {player.HP}/{player.MaxHP}");
                Console.WriteLine("\n1. Explore Location");
                Console.WriteLine("2. View Party");
                Console.WriteLine("3. Manage Summons");
                Console.WriteLine("4. Check Status");
                Console.WriteLine("5. Move to Next Area");
                Console.WriteLine("6. Travel to FireLink Shrine");
                Console.WriteLine("7. Exit Game");
            }
        }

       private void ProcessMainMenuChoice(string choice)
        {
            Console.Clear(); 
            if (isInHub)
            {
                switch (choice)
                {
                    case "1":
                        VisitMerchant();
                        break;
                    case "2":
                        VisitTemple();
                        break;
                    case "3":
                        ViewInventory();
                        break;
                    case "4":
                        isInHub = false;
                        Console.WriteLine("Returning to your quest...");
                        break;
                    case "5":
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
            else
            {
                switch (choice)
                {
                    case "1":
                        ExploreLocation();
                        break;
                    case "2":
                        ViewParty();
                        break;
                    case "3":
                        ManageSummons();
                        break;
                    case "4":
                        CheckStatus();
                        break;
                    case "5":
                        MoveToNextArea();
                        break;
                    case "6":
                        TravelToHub();
                        break;
                    case "7":
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }
        private void VisitMerchant()
        {
            Console.WriteLine("\n=== Merchant's Shop ===");
            Console.WriteLine($"Your Tartarus Essence: {player.TartarusEssence}");
            Console.WriteLine("\nAvailable Items:");
            
            for (int i = 0; i < merchantInventory.Count; i++)
            {
                var item = merchantInventory[i];
                Console.WriteLine($"{i + 1}. {item.Name} ({item.Type}) - {item.Price} Essence");
                switch (item.Type)
                {
                    case "Weapon":
                        Console.WriteLine($"   Attack: +{item.Value}");
                        break;
                    case "Armor":
                        Console.WriteLine($"   Defense: +{item.Value}");
                        break;
                    case "Consumable":
                        Console.WriteLine($"   Restores: {item.Value} HP");
                        break;
                }
            }

            Console.WriteLine("\nEnter item number to purchase (0 to exit):");
            string choice = Console.ReadLine();

            if (choice == "0") return;

            if (int.TryParse(choice, out int index) && index <= merchantInventory.Count)
            {
                var selectedItem = merchantInventory[index - 1];
                
                if (player.TartarusEssence >= selectedItem.Price)
                {
                    player.TartarusEssence -= selectedItem.Price;
                    inventory.Add(selectedItem);
                    Console.WriteLine($"Purchased {selectedItem.Name}!");
                }
                else
                {
                    Console.WriteLine("Not enough Tartarus Essence!");
                }
            }
        }
        private void VisitTemple()
        {
            Console.WriteLine("\n=== Temple of Ascension ===");
            Console.WriteLine($"Current Level: {player.Level}");
            Console.WriteLine($"Your Tartarus Essence: {player.TartarusEssence}");
            
            int requiredEssence = 5 * player.Level;
            Console.WriteLine($"\nRequired Essence for next level: {requiredEssence}");
            Console.WriteLine("Benefits per level:");
            Console.WriteLine("- Max HP +10");
            Console.WriteLine("- Attack +2");
            Console.WriteLine("- Defense +1");
            
            Console.WriteLine("\nDo you wish to level up? (Y/N)");
            string choice = Console.ReadLine()?.ToUpper();

            if (choice == "Y")
            {
                if (player.TartarusEssence >= requiredEssence)
                {
                    player.TartarusEssence -= requiredEssence;
                    player.Level++;
                    player.MaxHP += 10;
                    player.HP = player.MaxHP; // Heal to full when leveling up
                    player.Attack += 2;
                    player.Defense += 1;
                    
                    Console.WriteLine($"\nLevel Up! You are now level {player.Level}!");
                    Console.WriteLine($"New Stats:");
                    Console.WriteLine($"HP: {player.HP}/{player.MaxHP}");
                    Console.WriteLine($"Attack: {player.Attack}");
                    Console.WriteLine($"Defense: {player.Defense}");
                }
                else
                {
                    Console.WriteLine("Not enough Tartarus Essence!");
                }
            }
        }

       private void ViewInventory()
        {
            while (true)
            {
                Console.WriteLine("\n=== Inventory ===");
                Console.WriteLine($"Current Stats:");
                Console.WriteLine($"HP: {player.HP}/{player.MaxHP}");
                Console.WriteLine($"Attack: {player.TotalAttack} ({player.Attack} + {player.TotalAttack - player.Attack} from weapon)");
                Console.WriteLine($"Defense: {player.TotalDefense} ({player.Defense} + {player.TotalDefense - player.Defense} from armor)");
                
                if (inventory.Count == 0)
                {
                    Console.WriteLine("\nYour inventory is empty.");
                    return;
                }

                // Group items by type
                var weapons = inventory.Where(i => i.Type == "Weapon").ToList();
                var armors = inventory.Where(i => i.Type == "Armor").ToList();
                var consumables = inventory.Where(i => i.Type == "Consumable").ToList();

                Console.WriteLine("\nWeapons:");
                for (int i = 0; i < weapons.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {weapons[i].GetDescription()}");
                }

                Console.WriteLine("\nArmor:");
                for (int i = 0; i < armors.Count; i++)
                {
                    Console.WriteLine($"{weapons.Count + i + 1}. {armors[i].GetDescription()}");
                }

                Console.WriteLine("\nConsumables:");
                for (int i = 0; i < consumables.Count; i++)
                {
                    Console.WriteLine($"{weapons.Count + armors.Count + i + 1}. {consumables[i].GetDescription()}");
                }

                Console.WriteLine("\nChoose an action:");
                Console.WriteLine("1. Equip/Use Item");
                Console.WriteLine("2. Unequip Weapon");
                Console.WriteLine("3. Unequip Armor");
                Console.WriteLine("0. Exit Inventory");

                string choice = Console.ReadLine();

                if (choice == "0") return;

                switch (choice)
                {
                    case "1":
                        ManageInventoryItem(weapons, armors, consumables);
                        break;
                    case "2":
                        UnequipWeapon();
                        break;
                    case "3":
                        UnequipArmor();
                        break;
                }
            }
        }
        private void ManageInventoryItem(List<Item> weapons, List<Item> armors, List<Item> consumables)
        {
            Console.WriteLine("\nEnter item number to equip/use:");
            if (int.TryParse(Console.ReadLine(), out int index))
            {
                index--; // Convert to 0-based index
                Item selectedItem = null;

                if (index < weapons.Count)
                {
                    selectedItem = weapons[index];
                }
                else if (index < weapons.Count + armors.Count)
                {
                    selectedItem = armors[index - weapons.Count];
                }
                else if (index < weapons.Count + armors.Count + consumables.Count)
                {
                    selectedItem = consumables[index - weapons.Count - armors.Count];
                }

                if (selectedItem != null)
                {
                    switch (selectedItem.Type)
                    {
                        case "Weapon":
                            EquipWeapon(selectedItem);
                            break;
                        case "Armor":
                            EquipArmor(selectedItem);
                            break;
                        case "Consumable":
                            UseConsumable(selectedItem);
                            break;
                    }
                }
            }
        }
        private void EquipWeapon(Item weapon)
        {
            // Unequip current weapon if any
            if (player.EquippedWeapon != null)
            {
                player.EquippedWeapon.IsEquipped = false;
            }

            // Equip new weapon
            weapon.IsEquipped = true;
            player.EquippedWeapon = weapon;
            Console.WriteLine($"Equipped {weapon.Name}! Attack increased by {weapon.Value}");
        }

        private void EquipArmor(Item armor)
        {
            // Unequip current armor if any
            if (player.EquippedArmor != null)
            {
                player.EquippedArmor.IsEquipped = false;
            }

            // Equip new armor
            armor.IsEquipped = true;
            player.EquippedArmor = armor;
            Console.WriteLine($"Equipped {armor.Name}! Defense increased by {armor.Value}");
        }

        private void UnequipWeapon()
        {
            if (player.EquippedWeapon != null)
            {
                player.EquippedWeapon.IsEquipped = false;
                Console.WriteLine($"Unequipped {player.EquippedWeapon.Name}");
                player.EquippedWeapon = null;
            }
            else
            {
                Console.WriteLine("No weapon equipped!");
            }
        }

        private void UnequipArmor()
        {
            if (player.EquippedArmor != null)
            {
                player.EquippedArmor.IsEquipped = false;
                Console.WriteLine($"Unequipped {player.EquippedArmor.Name}");
                player.EquippedArmor = null;
            }
            else
            {
                Console.WriteLine("No armor equipped!");
            }
        }

        private void UseConsumable(Item consumable)
        {
            if (player.HP == player.MaxHP)
            {
                Console.WriteLine("HP is already full!");
                return;
            }

            player.HP = Math.Min(player.MaxHP, player.HP + consumable.Value);
            inventory.Remove(consumable);
            Console.WriteLine($"Used {consumable.Name}! HP restored to {player.HP}");
        }

        private void TravelToHub()
        {
            Console.WriteLine("\nTraveling to FireLink Shrine...");
            isInHub = true;
            // Heal player when arriving at hub
            player.HP = player.MaxHP;
            Console.WriteLine("You feel refreshed as you arrive at the shrine.");
        }

private void ExploreLocation()
{
    Location currentLocation = currentLevel[currentLocationIndex];
    Console.WriteLine($"\nExploring {currentLocation.Name}...");
    Console.WriteLine(EXPLORE_SPRITE);

    while (true)
    {
        // Display unexplored points of interest
        Console.WriteLine("\nPoints of Interest:");
        var unexploredPoints = currentLocation.PointsOfInterest
            .Where(poi => !poi.IsExplored)
            .ToList();

        if (unexploredPoints.Count == 0 && !currentLocation.IsBossDefeated)
        {
            Console.WriteLine("All points of interest explored! Boss fight available.");
            Console.WriteLine("\n1. Challenge Boss");
            Console.WriteLine("2. Return to main menu");

            string bosschoice = Console.ReadLine();
            if (bosschoice == "1")
            {
                InitiateBossFight(currentLocation.LevelBoss);
                return;
            }
            else if (bosschoice == "2")
            {
                return;
            }
            continue;
        }
        else if (unexploredPoints.Count == 0)
        {
            Console.WriteLine("Area fully explored! [✔]");
            return;
        }

        for (int i = 0; i < unexploredPoints.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {unexploredPoints[i].Name} [?]");
        }
        Console.WriteLine("0. Return to main menu");

        Console.WriteLine("\nChoose a point to explore:");
        string choice = Console.ReadLine();

        if (choice == "0") return;

        if (int.TryParse(choice, out int index) && index <= unexploredPoints.Count)
        {
            ExplorePointOfInterest(unexploredPoints[index - 1]);
        }
    }
}

private void ExplorePointOfInterest(PointOfInterest point)
{
    Console.WriteLine($"\nExploring {point.Name}...");
    Console.WriteLine(point.Description);

    // Check for enemies
    if (point.Enemies.Any())
    {
        Console.WriteLine(@"⚠️ ENEMY SPOTTED! ⚠️");
        foreach (var enemy in point.Enemies)
        {
            InitiateCombat(enemy);
        }
    }

    // Check for items
    if (point.Items != null && point.Items.Any())
    {
        Console.WriteLine(@"
        📦 ITEMS FOUND! 
        ");
        foreach (var Item in point.Items)
        {
            inventory.Add(Item);
            Console.WriteLine($"Found {Item.Name}!");
        }
    }

    // Check for summons
    if (point.Summons.Any())
    {
        Console.WriteLine(@"
        ✨ NEW SUMMON AVAILABLE! 
        ");
        foreach (var summon in point.Summons)
        {
            availableSummons.Add(summon);
            Console.WriteLine($"New summon discovered: {summon.Name} ({summon.Type})!");
        }
    }

    point.IsExplored = true;
}

private void InitiateBossFight(Boss boss)
{
    Console.WriteLine($"\n=== BOSS BATTLE ===");
    Console.WriteLine($"Challenging {boss.Name}, {boss.Title}!");
    Console.WriteLine(@"
    👿 BOSS BATTLE INITIATED!
    ");

    bool playerTurn = true;

    while (boss.HP > 0 && player.HP > 0)
    {
        if (playerTurn)
        {
            Console.WriteLine($"\nYour turn! Boss HP: {boss.HP}");
            Console.WriteLine("1. Attack");
            Console.WriteLine("2. Defend");
            Console.WriteLine("3. Use Summon");
            Console.WriteLine("4. Use Item");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    PerformAttack(player, boss);
                    break;
                case "2":
                    player.Defense *= 2;
                    Console.WriteLine($"{player.Name} takes defensive stance!");
                    break;
                case "3":
                    if (activeParty.Any())
                    {
                        UseSummonInBattle(boss);
                    }
                    else
                    {
                        Console.WriteLine("No summons in active party!");
                        continue;
                    }
                    break;
                case "4":
                    UseItemInBattle();
                    continue;
            }
        }
        else
        {
            // Boss turn with special moves
            Console.WriteLine($"\n{boss.Name}'s turn!");
            if (new Random().Next(100) < 30)
            {
                // 30% chance for special attack
                int damage = boss.Attack * 2;
                player.HP -= damage;
                Console.WriteLine($"{boss.Name} uses special attack! Deals {damage} damage!");
            }
            else
            {
                PerformAttack(boss, player);
            }
        }
        playerTurn = !playerTurn;
    }

    if (boss.HP <= 0)
    {
        Console.WriteLine(@"
        🏆 BOSS DEFEATED!
        ");
        int essence = new Random().Next(50, 100);
        player.TartarusEssence += essence;
        Console.WriteLine($"Victory! Gained {essence} Tartarus Essence");
        currentLevel[currentLocationIndex].IsBossDefeated = true;
    }
    else
    {
        Console.WriteLine(@"
        💀 DEFEATED BY BOSS
        ");
        HandlePlayerDeath();
    }
}

private void UseSummonInBattle(Character enemy)
{
    Console.WriteLine("\nSelect summon to use:");
    for (int i = 0; i < activeParty.Count; i++)
    {
        Console.WriteLine($"{i + 1}. {activeParty[i].Name} (HP: {activeParty[i].HP}/{activeParty[i].MaxHP})");
    }

    if (int.TryParse(Console.ReadLine(), out int choice) && choice <= activeParty.Count)
    {
        PerformAttack(activeParty[choice - 1], enemy);
    }
}
private void UseItemInBattle()
{
    var consumables = inventory.Where(i => i.Type == "Consumable").ToList();
    if (!consumables.Any())
    {
        Console.WriteLine("No consumable items available!");
        return;
    }

    Console.WriteLine("\nSelect item to use:");
    for (int i = 0; i < consumables.Count; i++)
    {
        Console.WriteLine($"{i + 1}. {consumables[i].GetDescription()}");
    }

    if (int.TryParse(Console.ReadLine(), out int choice) && choice <= consumables.Count)
    {
        UseConsumable(consumables[choice - 1]);
    }
}
        private void InitiateCombat(Character enemy)
        {
            Console.WriteLine($"\nCombat started with {enemy.Name}!");
            Console.WriteLine(ENEMY_SPRITE);
            Console.WriteLine("\nVS\n");
            Console.WriteLine(PLAYER_SPRITE);
            bool playerTurn = true;

            while (enemy.HP > 0 && player.HP > 0)
            {
                if (playerTurn)
                {
                    Console.WriteLine("\nYour turn!");
                    Console.WriteLine("1. Attack");
                    Console.WriteLine("2. Defend");
                    Console.WriteLine("3. Use Summon");

                    string choice = Console.ReadLine();
                    switch (choice)
                    {
                        case "1":
                            Console.WriteLine(@"
                            ⚔️
                             /||\ 
                            ");
                            PerformAttack(player, enemy);
                            break;
                        case "2":
                            Console.WriteLine(@"
                            🛡️
                             /||\ 
                            ");
                            player.Defense *= 2; // Temporary defense boost
                            Console.WriteLine($"{player.Name} takes defensive stance!");
                            break;
                        case "3":
                            if (activeParty.Any())
                            {
                                Console.WriteLine(@"
                                ✨
                                \[T]/
                                ");
                                Summon summon = activeParty[0]; // Use first summon for simplicity
                                PerformAttack(summon, enemy);
                            }
                            else
                            {
                                Console.WriteLine("No summons in active party!");
                                continue;
                            }
                            break;
                    }
                }
                else
                {
                    // Enemy turn
                    Console.WriteLine(@"
                    😈 Enemy Turn!
                    ");
                    PerformAttack(enemy, player);
                }
                playerTurn = !playerTurn;
            }

            if (enemy.HP <= 0)
            {
                Console.WriteLine(@"
                🏆 VICTORY!
                ");
                int essence = new Random().Next(10, 30);
                player.TartarusEssence += essence;
                Console.WriteLine($"Victory! Gained {essence} Tartarus Essence");
                if(player.Chance == 1){
                    Console.WriteLine($"And You have reclaimed your Essence");
                    Console.WriteLine($"Don't let them take it away again.");
                }
            }
            else
            {
                Console.WriteLine(@"
                💀 DEFEATED
                ");
                Console.WriteLine("You have been defeated...");
                HandlePlayerDeath();
            }
        }

        private void PerformAttack(Character attacker, Character target)
        {
            // Calculate hit chance (70-90%)
            int hitChance = new Random().Next(70, 91);
            if (new Random().Next(1, 101) <= hitChance)
            {
                int damage = Math.Max(1, attacker.TotalAttack - target.Defense);
                target.HP -= damage;
                Console.WriteLine($"{attacker.Name} deals {damage} damage to {target.Name}!");
            }
            else
            {
                Console.WriteLine($"{attacker.Name}'s attack missed!");
            }
        }

        private void HandlePlayerDeath()
        {
            if(player.Chance == 0)
            {
                Console.WriteLine("You lost all of your Tartarus Essence...");
                Console.WriteLine("But the Essence still lingers, dont waste your chance");
                player.Chance = 1;
            }
            else{
                Console.WriteLine("You lost all of your Tartarus Essence...");
                Console.WriteLine("And you have lost the chance to gain your Essence back ...");
                player.Chance = 0;
            }
                player.TartarusEssenceOnDeath = player.TartarusEssence;
                player.TartarusEssence = 0;
                player.HP = player.MaxHP;
                // Reset to last safe location
                currentLocationIndex = Math.Max(0, currentLocationIndex - 1);
        }

        private void ViewParty()
        {
            Console.WriteLine("\n=== Active Party ===");
            Console.WriteLine($"Player: {player.Name} (HP: {player.HP}/{player.MaxHP})");
            foreach (var summon in activeParty)
            {
                Console.WriteLine($"Summon: {summon.Name} ({summon.Type}) - HP: {summon.HP}/{summon.MaxHP}");
            }
        }

        private void ManageSummons()
        {
            Console.WriteLine("\n=== Manage Summons ===");
            Console.WriteLine("Available Summons:");
            
            for (int i = 0; i < availableSummons.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {availableSummons[i].Name} ({availableSummons[i].Type})");
            }

            Console.WriteLine("\nEnter number to add to party (max 3), or 0 to return:");
            string choice = Console.ReadLine();

            if (choice != "0" && int.TryParse(choice, out int index) && index <= availableSummons.Count)
            {
                if (activeParty.Count < 3)
                {
                    activeParty.Add(availableSummons[index - 1]);
                    Console.WriteLine($"{availableSummons[index - 1].Name} added to party!");
                }
                else
                {
                    Console.WriteLine("Party is full! (Max 3 summons)");
                }
            }
        }

        private void CheckStatus()
        {
            Console.WriteLine($"\n=== Status ===");
            Console.WriteLine($"Name: {player.Name}");
            Console.WriteLine($"Level: {player.Level}");
            Console.WriteLine($"HP: {player.HP}/{player.MaxHP}");
            Console.WriteLine($"Attack: {player.Attack}");
            Console.WriteLine($"Defense: {player.Defense}");
            Console.WriteLine($"Tartarus Essence: {player.TartarusEssence}");
        }

private void MoveToNextArea()
{
    Location currentLocation = currentLevel[currentLocationIndex];
    
    if (!currentLocation.IsBossDefeated)
    {
        Console.WriteLine("You must defeat the boss before moving to the next area!");
        return;
    }

    currentLocationIndex++;
    if (currentLocationIndex >= currentLevel.Count)
    {
        Console.WriteLine("Level complete! Moving to next level...");
        InitializeLevel("Nyx Palace - Next Floor");
        currentLocationIndex = 0;
    }

    Console.WriteLine($"Moved to {currentLevel[currentLocationIndex].Name}");
}
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Game game = new Game();
            game.StartGame();
        }
    }
}