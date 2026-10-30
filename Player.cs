namespace CYOA;
public class Player
{
    public string Name { get; set; }
    public Race Race { get; set; }
    public Occupation Occupation { get; set; }
    public int Strength { get; set; }
    public int Agility { get; set; }
    public int Health { get; set; }
    public Weapon Weapon { get; set; }
    public Player(
        string name,
        Race race,
        Occupation occupation,
        int strength,
        int agility,
        int health,
        Weapon weapon)
    {
        Name = name;
        Race = race;
        Occupation = occupation;
        Strength = strength;
        Agility = agility;
        Health = health;
        Weapon = weapon;
    }
    private static string Label(
        Messages? messages,
        int id,
        string fallback) =>
        messages == null ? fallback : messages.GetMessage(id);
    public string DisplayPlayertatistics(Messages? messages = null)
    {
        return
            "===== PLAYER STATISTICS =====\n" +
            $"{Label(messages, 27, "Name")}: {Name}\n" +
            $"{Label(messages, 28, "Race")}: {Race}\n" +
            $"{Label(messages, 29, "Occupation")}: {Occupation}\n" +
            $"{Label(messages, 30, "Strength")}: {Strength}\n" +
            $"{Label(messages, 31, "Agility")}: {Agility}\n" +
            $"{Label(messages, 32, "Health points")}: {Health}\n" +
            $"{Label(messages, 33, "Weapon")}: {Weapon.type}\n" +
            $"{Label(messages, 34, "Maximum Weapon Damage")}: {Weapon.maxDamage}\n" +
            $"{Weapon.asciiArt}\n" +
            "================================";
    }
}
