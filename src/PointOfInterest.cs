public class PointOfInterest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsExplored { get; set; }
    public List<Character> Enemies { get; set; }
    public List<Item> Items { get; set; }
    public List<Summon> Summons { get; set; }
    public Dictionary<EnemyType, int> EnemySpawnRate { get; set; }

    public PointOfInterest(string name, string description)
    {
        Name = name;
        Description = description;
        IsExplored = false;
        Enemies = new List<Character>();
        Items = new List<Item>();
        Summons = new List<Summon>();
        EnemySpawnRate = new Dictionary<EnemyType, int>();

        foreach (EnemyType enemyType in Enum.GetValues(typeof(EnemyType)))
        {
            EnemySpawnRate[enemyType] = 1; // Set default spawn rate to 1
        }
    }
}