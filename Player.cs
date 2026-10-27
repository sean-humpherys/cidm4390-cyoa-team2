
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

    public const string RacePrompt =
        "What race of character do you want to be? Human, Elf, Dwarf or Halfling?";

    public const string NamePrompt =
        "What is the name of your character?";

    public const string OccupationPrompt =
        "What is the occupation of your character? Fighter, magician, thief, or archer?";

    public const string StrengthPrompt =
        "Next, we are going to roll for strength. Type 'roll'";

    public const string AgilityPrompt =
        "Next, we will roll for agility. Type 'roll'";

    public const string HealthPrompt =
        "Next, we will roll for health points. Type 'roll'";

    public const string NextStepPrompt =
        "What would you like to do next? 1. List character stats. 2. Proceed on an adventure. Type 1 or 2";

    public const string InvalidRaceMessage =
        "Invalid race. Choose Human, Elf, Dwarf or Halfling.";

    public const string InvalidNameMessage =
        "Name cannot be empty.";

    public const string InvalidOccupationMessage =
        "Invalid occupation. Choose Fighter, Magician, Thief or Archer.";

    public const string InvalidRollMessage =
        "Type 'roll'.";

    public const string InvalidNextStepMessage =
        "Invalid choice. Type 1 or 2.";

    public const int NameLabelId = 27;
    public const int RaceLabelId = 28;
    public const int OccupationLabelId = 29;
    public const int StrengthLabelId = 30;
    public const int AgilityLabelId = 31;
    public const int HealthLabelId = 32;
    public const int WeaponLabelId = 33;
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
