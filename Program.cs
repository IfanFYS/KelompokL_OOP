using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace PersonaWrathOfNyx
{
    #region Enums and Data Classes
    public enum CharacterClass
    {
        Mage,
        Knight,
        Rogue,
        Summoner
    }

    public enum StatusEffect
    {
        Poisoned,
        Burned,
        Frozen,
        Weakened
    }

    public class Stats
    {
        public int HP { get; set; }
        public int MaxHP { get; set; }
        public int MP { get; set; }
        public int MaxMP { get; set; }
        public int Strength { get; set; }
        public int Intelligence { get; set; }
        public int Dexterity { get; set; }
        public int Defense { get; set; }
        public int Level { get; set; } = 1;
        public int Exp { get; set; } = 0;
        public int TartarusEssence { get; set; } = 0;
    }
    #endregion

    #region Core Game Classes
    public class Character
    {
        public string Name { get; private set; }
        public CharacterClass CharacterClass { get; private set; }
        public Stats Stats { get; private set; }
        public List<StatusEffect> StatusEffects { get; private set; }
        public List<Item> Inventory { get; private set; }
        public Dictionary<string, Equipment> EquippedItems { get; private set; }
        public List<Ability> Abilities { get; private set; }

        public Character(string name, CharacterClass characterClass, Stats stats)
        {
            Name = name;
            CharacterClass = characterClass;
            Stats = stats;
            StatusEffects = new List<StatusEffect>();
            Inventory = new List<Item>();
            EquippedItems = new Dictionary<string, Equipment>();
            Abilities = new List<Ability>();
            InitializeAbilities();
        }

        private void InitializeAbilities()
        {
            switch (CharacterClass)
            {
                case CharacterClass.Mage:
                    Abilities.Add(new Ability("Fireball", "Launches a ball of fire", 20, 0.9f, 25));
                    break;
                case CharacterClass.Knight:
                    Abilities.Add(new Ability("Slash", "A powerful sword attack", 10, 0.95f, 20));
                    break;
                // Add more abilities for other classes
            }
        }

        public void TakeDamage(int damage)
        {
            int actualDamage = Math.Max(1, damage - Stats.Defense);
            Stats.HP = Math.Max(0, Stats.HP - actualDamage);
            Console.WriteLine($"{Name} takes {actualDamage} damage!");
        }

        public bool IsAlive() => Stats.HP > 0;
    }

    public class Item
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Type { get; private set; }

        public Item(string name, string description, string type)
        {
            Name = name;
            Description = description;
            Type = type;
        }
    }

    public class Equipment : Item
    {
        public Dictionary<string, int> StatBonuses { get; private set; }
        public string Slot { get; private set; }

        public Equipment(string name, string description, string slot, Dictionary<string, int> statBonuses)
            : base(name, description, "Equipment")
        {
            Slot = slot;
            StatBonuses = statBonuses;
        }
    }

    public class Ability
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public int MPCost { get; private set; }
        public float SuccessRate { get; private set; }
        public int BaseDamage { get; private set; }

        public Ability(string name, string description, int mpCost, float successRate, int baseDamage)
        {
            Name = name;
            Description = description;
            MPCost = mpCost;
            SuccessRate = successRate;
            BaseDamage = baseDamage;
        }

        public bool Execute(Character user, Character target)
        {
            if (user.Stats.MP < MPCost)
            {
                Console.WriteLine("Not enough MP!");
                return false;
            }

            if (Random.Shared.NextDouble() <= SuccessRate)
            {
                user.Stats.MP -= MPCost;
                int damage = BaseDamage + user.Stats.Strength;
                target.TakeDamage(damage);
                return true;
            }

            Console.WriteLine($"{Name} missed!");
            return false;
        }
    }
    #endregion

    #region Game Systems
    public class BattleSystem
    {
        private Character player;
        private List<Character> enemies;
        private bool isBattleActive;

        public BattleSystem(Character player, List<Character> enemies)
        {
            this.player = player;
            this.enemies = enemies;
        }

        public void StartBattle()
        {
            isBattleActive = true;
            Console.WriteLine("\n=== Battle Start! ===\n");

            while (isBattleActive)
            {
                DisplayBattleStatus();
                PlayerTurn();
                
                if (!enemies.Any(e => e.IsAlive()))
                {
                    BattleVictory();
                    break;
                }

                EnemyTurn();
                
                if (!player.IsAlive())
                {
                    BattleDefeat();
                    break;
                }
            }
        }

        private void DisplayBattleStatus()
        {
            Console.WriteLine("\nCurrent Status:");
            Console.WriteLine($"{player.Name} - HP: {player.Stats.HP}/{player.Stats.MaxHP} MP: {player.Stats.MP}/{player.Stats.MaxMP}");
            foreach (var enemy in enemies.Where(e => e.IsAlive()))
            {
                Console.WriteLine($"{enemy.Name} - HP: {enemy.Stats.HP}/{enemy.Stats.MaxHP}");
            }
        }

        private void PlayerTurn()
        {
            Console.WriteLine("\nYour turn! Choose action:");
            Console.WriteLine("1. Attack");
            Console.WriteLine("2. Use Ability");
            Console.WriteLine("3. Use Item");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    HandlePlayerAttack();
                    break;
                case "2":
                    HandlePlayerAbility();
                    break;
                case "3":
                    HandlePlayerItem();
                    break;
                default:
                    Console.WriteLine("Invalid choice. Skipping turn...");
                    break;
            }
        }

        private void HandlePlayerAttack()
        {
            var aliveEnemies = enemies.Where(e => e.IsAlive()).ToList();
            Console.WriteLine("\nChoose target:");
            for (int i = 0; i < aliveEnemies.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {aliveEnemies[i].Name}");
            }

            if (int.TryParse(Console.ReadLine(), out int targetIndex) && targetIndex > 0 && targetIndex <= aliveEnemies.Count)
            {
                var target = aliveEnemies[targetIndex - 1];
                int damage = player.Stats.Strength * 2;
                target.TakeDamage(damage);
            }
            else
            {
                Console.WriteLine("Invalid target. Attack missed!");
            }
        }

        private void HandlePlayerAbility()
        {
            Console.WriteLine("\nChoose ability:");
            for (int i = 0; i < player.Abilities.Count; i++)
            {
                var ability = player.Abilities[i];
                Console.WriteLine($"{i + 1}. {ability.Name} (MP Cost: {ability.MPCost})");
            }

            if (int.TryParse(Console.ReadLine(), out int abilityIndex) && abilityIndex > 0 && abilityIndex <= player.Abilities.Count)
            {
                var ability = player.Abilities[abilityIndex - 1];
                var aliveEnemies = enemies.Where(e => e.IsAlive()).ToList();
                
                Console.WriteLine("\nChoose target:");
                for (int i = 0; i < aliveEnemies.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {aliveEnemies[i].Name}");
                }

                if (int.TryParse(Console.ReadLine(), out int targetIndex) && targetIndex > 0 && targetIndex <= aliveEnemies.Count)
                {
                    ability.Execute(player, aliveEnemies[targetIndex - 1]);
                }
            }
        }

        private void HandlePlayerItem()
        {
            if (player.Inventory.Count == 0)
            {
                Console.WriteLine("No items in inventory!");
                return;
            }

            Console.WriteLine("\nChoose item:");
            for (int i = 0; i < player.Inventory.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {player.Inventory[i].Name}");
            }
            // Implement item usage logic
        }

        private void EnemyTurn()
        {
            foreach (var enemy in enemies.Where(e => e.IsAlive()))
            {
                Console.WriteLine($"\n{enemy.Name}'s turn!");
                Thread.Sleep(1000); // Add some delay for better readability
                int damage = enemy.Stats.Strength;
                player.TakeDamage(damage);
            }
        }

        private void BattleVictory()
        {
            Console.WriteLine("\nVictory! You defeated all enemies!");
            // Add rewards, experience, etc.
        }

        private void BattleDefeat()
        {
            Console.WriteLine("\nDefeat... You have fallen in battle.");
            // Handle player defeat
        }
    }

    public class Game
    {
        private Character player;
        private List<Character> enemies;
        private BattleSystem currentBattle;

        public void Start()
        {
            Console.WriteLine("Welcome to Persona: Wrath of Nyx!\n");
            CreateCharacter();
            MainGameLoop();
        }

        private void CreateCharacter()
        {
            Console.WriteLine("Enter your character's name:");
            string name = Console.ReadLine();

            Console.WriteLine("\nChoose your class:");
            Console.WriteLine("1. Mage");
            Console.WriteLine("2. Knight");
            Console.WriteLine("3. Rogue");
            Console.WriteLine("4. Summoner");

            CharacterClass characterClass = CharacterClass.Knight; // Default
            if (int.TryParse(Console.ReadLine(), out int classChoice))
            {
                characterClass = (CharacterClass)(classChoice - 1);
            }

            Stats initialStats = GenerateInitialStats(characterClass);
            player = new Character(name, characterClass, initialStats);

            Console.WriteLine($"\nWelcome, {name} the {characterClass}!");
        }

        private Stats GenerateInitialStats(CharacterClass characterClass)
        {
            Stats stats = new Stats();
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    stats.MaxHP = stats.HP = 80;
                    stats.MaxMP = stats.MP = 120;
                    stats.Strength = 5;
                    stats.Intelligence = 15;
                    stats.Dexterity = 8;
                    stats.Defense = 6;
                    break;
                case CharacterClass.Knight:
                    stats.MaxHP = stats.HP = 120;
                    stats.MaxMP = stats.MP = 40;
                    stats.Strength = 15;
                    stats.Intelligence = 5;
                    stats.Dexterity = 8;
                    stats.Defense = 12;
                    break;
                // Add other classes
            }
            return stats;
        }

        private void MainGameLoop()
        {
            bool isRunning = true;
            while (isRunning && player.IsAlive())
            {
                Console.WriteLine("\nWhat would you like to do?");
                Console.WriteLine("1. Explore");
                Console.WriteLine("2. View Character");
                Console.WriteLine("3. View Inventory");
                Console.WriteLine("4. Exit Game");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        Explore();
                        break;
                    case "2":
                        ViewCharacter();
                        break;
                    case "3":
                        ViewInventory();
                        break;
                    case "4":
                        isRunning = false;
                        break;
                    default:
                        Console.WriteLine("Invalid choice!");
                        break;
                }
            }
        }

        private void Explore()
        {
            Console.WriteLine("\nExploring area...");
            if (Random.Shared.NextDouble() < 0.7) // 70% chance of encounter
            {
                InitiateBattle();
            }
            else
            {
                Console.WriteLine("You found nothing of interest.");
            }
        }

        private void InitiateBattle()
        {
            enemies = new List<Character>
            {
                new Character("Undead Soldier", CharacterClass.Knight, new Stats 
                { 
                    MaxHP = 50, 
                    HP = 50, 
                    Strength = 8, 
                    Defense = 5 
                })
            };

            currentBattle = new BattleSystem(player, enemies);
            currentBattle.StartBattle();
        }

        private void ViewCharacter()
        {
            Console.WriteLine($"\n=== {player.Name} the {player.CharacterClass} ===");
            Console.WriteLine($"Level: {player.Stats.Level}");
            Console.WriteLine($"HP: {player.Stats.HP}/{player.Stats.MaxHP}");
            Console.WriteLine($"MP: {player.Stats.MP}/{player.Stats.MaxMP}");
            Console.WriteLine($"Strength: {player.Stats.Strength}");
            Console.WriteLine($"Intelligence: {player.Stats.Intelligence}");
            Console.WriteLine($"Dexterity: {player.Stats.Dexterity}");
            Console.WriteLine($"Defense: {player.Stats.Defense}");
            Console.WriteLine($"Tartarus Essence: {player.Stats.TartarusEssence}");
        }

        private void ViewInventory()
        {
            Console.WriteLine("\n=== Inventory ===");
            if (player.Inventory.Count == 0)
            {
                Console.WriteLine("Inventory is empty!");
                return;
            }

            for (int i = 0; i < player.Inventory.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {player.Inventory[i].Name}");
            }
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Game game = new Game();
            game.Start();
        }
    }
}
#endregion