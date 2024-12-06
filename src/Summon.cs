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