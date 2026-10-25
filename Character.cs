namespace CYOA;



public class Character

{

    public string Name { get; set; }

    public string Race { get; set; }

    public string Occupation { get; set; }

    public int Strength { get; set; }

    public int Agility { get; set; }

    public int Health { get; set; }

    public string Weapon { get; set; }

    public int MaxWeaponDamage { get; set; }



    public Character(

        string name,

        string race,

        string occupation,

        int strength,

        int agility,

        int health,

        string weapon,

        int maxWeaponDamage)

    {

        Name = name;

        Race = race;

        Occupation = occupation;

        Strength = strength;

        Agility = agility;

        Health = health;

        Weapon = weapon;

        MaxWeaponDamage = maxWeaponDamage;

    }



    public const string RacePrompt = "What race of character do you want to be? Human, Elf, Dwarf or Halfling?";

    public const string NamePrompt = "What is the name of your character?";

    public const string OccupationPrompt = "What is the occupation of your character? Fighter, magician, thief, or archer?";

    public const string StrengthPrompt = "Next, we are going to roll for strength. Type 'roll'";

    public const string AgilityPrompt = "Next, we will roll for agility. Type 'roll'";

    public const string HealthPrompt = "Next, we will roll for health points. Type 'roll'";

    public const string NextStepPrompt = "What would you like to do next? 1. List character stats. 2. Proceed on an adventure. Type 1 or 2";

    public const string InvalidRaceMessage = "Invalid race. Choose Human, Elf, Dwarf or Halfling.";

    public const string InvalidNameMessage = "Name cannot be empty.";

    public const string InvalidOccupationMessage = "Invalid occupation. Choose Fighter, Magician, Thief or Archer.";

    public const string InvalidRollMessage = "Type 'roll'.";

    public const string InvalidNextStepMessage = "Invalid choice. Type 1 or 2.";



    // Character stat labels in English.txt (27 to 34).

    public const int NameLabelId = 27;

    public const int RaceLabelId = 28;

    public const int OccupationLabelId = 29;

    public const int StrengthLabelId = 30;

    public const int AgilityLabelId = 31;

    public const int HealthLabelId = 32;

    public const int WeaponLabelId = 33;

    public const int MaxDamageLabelId = 34;



    private static string Label(Messages? messages, int id, string fallback) =>

        messages == null ? fallback : messages.GetMessage(id);



    public static string FormatSummary(Character c, Messages? messages = null) =>

        "===== CHARACTER STATISTICS =====\n" +

        $"{Label(messages, NameLabelId, "Name")}: {c.Name}\n" +

        $"{Label(messages, RaceLabelId, "Race")}: {c.Race}\n" +

        $"{Label(messages, OccupationLabelId, "Occupation")}: {c.Occupation}\n" +

        $"{Label(messages, StrengthLabelId, "Strength")}: {c.Strength}\n" +

        $"{Label(messages, AgilityLabelId, "Agility")}: {c.Agility}\n" +

        $"{Label(messages, HealthLabelId, "Health points")}: {c.Health}\n" +

        $"{Label(messages, WeaponLabelId, "Weapon")}: {c.Weapon}\n" +

        $"{Label(messages, MaxDamageLabelId, "Weapon's maximum damage points")}: {c.MaxWeaponDamage}\n" +

        "================================";



    public static Character Create(

        TextReader? input = null,

        TextWriter? output = null,

        Func<int, int>? roll = null,

        Messages? messages = null)

    {

        input ??= Console.In;

        output ??= Console.Out;

        roll ??= Dice.Roll;



        string race = Ask(input, output, RacePrompt, ParseRace, InvalidRaceMessage);

        string name = Ask(input, output, NamePrompt, v => v.Length == 0 ? null : v, InvalidNameMessage);

        string occupation = Ask(input, output, OccupationPrompt, ParseOccupation, InvalidOccupationMessage);



        (string weapon, int maxWeaponDamage) = occupation switch

        {

            "Fighter" => ("long sword", 12),

            "Magician" => ("lightning bolt spell", 12),

            "Thief" => ("dagger", 6),

            _ => ("long bow", 8)   // Archer

        };



        bool isHalfling = race == "Halfling";



        // Strength: 1-20, then +1-4 (Human, Elf, Dwarf) or -1-4 (Halfling).

        AskRoll(input, output, StrengthPrompt);

        int strength = roll(20);

        int strengthModifier = roll(4);

        strength += isHalfling ? -strengthModifier : strengthModifier;

        output.WriteLine($"{Label(messages, StrengthLabelId, "Strength")}: {strength}");



        // Agility: 1-20, then +1-4 (Halfling, Elf). Human and Dwarf have no modifier.

        AskRoll(input, output, AgilityPrompt);

        int agility = roll(20);

        if (isHalfling || race == "Elf")

        {

            agility += roll(4);

        }

        output.WriteLine($"{Label(messages, AgilityLabelId, "Agility")}: {agility}");



        // Health: 1-20, then add the strength modifier (Human, Elf, Dwarf). Halfling has none.

        AskRoll(input, output, HealthPrompt);

        int health = roll(20);

        if (!isHalfling)

        {

            health += strengthModifier;

        }

        output.WriteLine($"{Label(messages, HealthLabelId, "Health points")}: {health}");



        var character = new Character(

            name, race, occupation, strength, agility, health, weapon, maxWeaponDamage);



        // 1 shows the summary and asks again. 2 proceeds to the adventure.

        while (true)

        {

            string choice = Ask(input, output, NextStepPrompt,

                v => v == "1" || v == "2" ? v : null, InvalidNextStepMessage);



            if (choice == "2")

            {

                return character;

            }



            output.WriteLine(FormatSummary(character, messages));

        }

    }



    private static string? ParseRace(string value) => value.ToLowerInvariant() switch

    {

        "human" => "Human",

        "elf" => "Elf",

        "dwarf" => "Dwarf",

        "halfling" => "Halfling",

        _ => null

    };



    private static string? ParseOccupation(string value) => value.ToLowerInvariant() switch

    {

        "fighter" => "Fighter",

        "magician" => "Magician",

        "thief" => "Thief",

        "archer" => "Archer",

        _ => null

    };



    private static void AskRoll(TextReader input, TextWriter output, string prompt) =>

        Ask(input, output, prompt,

            v => v.Equals("roll", StringComparison.OrdinalIgnoreCase) ? v : null,

            InvalidRollMessage);



    private static string Ask(

        TextReader input,

        TextWriter output,

        string prompt,

        Func<string, string?> parse,

        string invalidMessage)

    {

        while (true)

        {

            output.WriteLine(prompt);



            string? line = input.ReadLine();

            if (line == null)

            {

                throw new EndOfStreamException(

                    "Input ended during character creation.");

            }



            string? result = parse(line.Trim());

            if (result != null)

            {

                return result;

            }



            output.WriteLine(invalidMessage);

        }

    }

}