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
    public EnemyType EnemyType { get; set; }

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

    public Character(string name, int hp, int attack, int defense, EnemyType enemyType)
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
        EnemyType = enemyType;
    }
}