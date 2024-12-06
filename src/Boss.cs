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
