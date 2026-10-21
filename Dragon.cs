public class Dragon
{
    public string Name { get; set; }
    public int Strength { get; set; }
    public int Health { get; set; }
    public int Agility { get; set; }
    public string Weapon { get; set; }

    public Dragon(string name)
    {
        Name = name;
        Strength = 15;
        Health = 100;
        Agility = 10;
        Weapon = "Fire Breath";
    }

    public string DisplayStats()
    {
        return "--- Dragon Statistics ---\n" +
               $"Name: {Name}\n" +
               $"Strength: {Strength}\n" +
               $"Health: {Health}\n" +
               $"Agility: {Agility}\n" +
               $"Weapon: {Weapon}";
    }
}
