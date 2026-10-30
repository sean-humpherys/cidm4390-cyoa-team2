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

        public const int MaxDamageLabelId = 34;

    private static string Label(
        Messages? messages,
        int id,
        string fallback) =>
        messages == null ? fallback : messages.GetMessage(id);

    public string DisplayPlayertatistics(Messages? messages = null)
    {
        return
            "===== PLAYER STATISTICS =====\n" +
            $"{Label(messages, NameLabelId, "Name")}: {Name}\n" +
            $"{Label(messages, RaceLabelId, "Race")}: {Race}\n" +
            $"{Label(messages, OccupationLabelId, "Occupation")}: {Occupation}\n" +
            $"{Label(messages, StrengthLabelId, "Strength")}: {Strength}\n" +
            $"{Label(messages, AgilityLabelId, "Agility")}: {Agility}\n" +
            $"{Label(messages, HealthLabelId, "Health points")}: {Health}\n" +
            $"{Label(messages, WeaponLabelId, "Weapon")}: {Weapon.type}\n" +
            $"{Label(messages, MaxDamageLabelId, "Maximum Weapon Damage")}: {Weapon.maxDamage}\n" +
            $"{Weapon.asciiArt}\n" +
            "================================";
    }
}
