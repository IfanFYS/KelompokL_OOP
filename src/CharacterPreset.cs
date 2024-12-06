public class CharacterPreset
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int BaseHP { get; set; }
    public int BaseMP { get; set; }
    public int BaseStrength { get; set; }
    public int BaseMagic { get; set; }

    public CharacterPreset(string name, string description, int baseHP, int baseMP, int baseStrength, int baseMagic)
    {
        Name = name;
        Description = description;
        BaseHP = baseHP;
        BaseMP = baseMP;
        BaseStrength = baseStrength;
        BaseMagic = baseMagic;
    }
}