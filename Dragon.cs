
namespace CYOA;

public class Dragon
{
    public string DragonName { get; set; }
    public int DragonStrength { get; set; }
    public int DragonHealth { get; set; }
    public int DragonAgility { get; set; }
    public Weapon Weapon { get; set; }

    public Dragon(string name)
    {
        DragonName = name;
        DragonStrength = 15;
        DragonHealth = 100;
        DragonAgility = 10;

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
               $"Name: {DragonName}\n" +
               $"Strength: {DragonStrength}\n" +
               $"Health: {DragonHealth}\n" +
               $"Agility: {DragonAgility}\n" +
               $"Weapon: {Weapon.type}\n" +
               $"Weapon Damage: {Weapon.maxDamage}\n" +
               $"{Weapon.asciiArt}";
    }
}
