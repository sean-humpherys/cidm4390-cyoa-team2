
namespace CYOA;

public class Dragon
{
    public string Name { get; set; }
    public int Strength { get; set; }
    public int Health { get; set; }
    public int Agility { get; set; }
    public Weapon Weapon { get; set; }

    public Dragon(string name)
    {
        Name = name;
        Strength = 15;
        Health = 100;
        Agility = 10;

        Weapon = new Weapon(
            "Fire Breath",
            25,
            "  (  )\n (    )\n  (  )"
        );
    }

    public string MissedMeTaunt()
    {
        return "The dragon laughs as your attack misses!";
    }

    public string WasHitTaunt()
    {
        return "The dragon roars in anger after being hit!";
    }

    public string DisplayStats()
    {
        return "--- Dragon Statistics ---\n" +
               $"Name: {Name}\n" +
               $"Strength: {Strength}\n" +
               $"Health: {Health}\n" +
               $"Agility: {Agility}\n" +
               $"Weapon: {Weapon.type}\n" +
               $"Weapon Damage: {Weapon.maxDamage}\n" +
               $"{Weapon.asciiArt}";
    }
}
