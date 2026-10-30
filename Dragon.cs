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
    private static string Label(
        Messages? messages,
        int id,
        string fallback) =>
        messages == null ? fallback : messages.GetMessage(id);
    public string DisplayStats(Messages? messages = null)
    {
        return $"{Label(messages, 66, "--- Dragon Statistics ---")}\n" +
               $"{Label(messages, 27, "Name")}: {DragonName}\n" +
               $"{Label(messages, 30, "Strength")}: {DragonStrength}\n" +
               $"{Label(messages, 32, "Health")}: {DragonHealth}\n" +
               $"{Label(messages, 31, "Agility")}: {DragonAgility}\n" +
               $"{Label(messages, 33, "Weapon")}: {Weapon.type}\n" +
               $"{Label(messages, 34, "Weapon Damage")}: {Weapon.maxDamage}\n" +
               $"{Weapon.asciiArt}";
    }
}
