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