
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

    
    public static Character Create()
    {
        
     
        string race = AskChoice(
            "Choose your race:",
            "Human", "Elf", "Dwarf", "Halfling");

      
        string name;

        do
        {
            Console.Write("Enter your character's name: ");
            name = (Console.ReadLine() ?? "").Trim();

            if (name.Length == 0)
            {
                Console.WriteLine("Name cannot be empty.");
            }
        }
        while (name.Length == 0);

        // Step 3: Choose occupation
        string occupation = AskChoice(
            "Choose your occupation:",
            "Fighter", "Magician", "Thief", "Archer");

       
        string weapon;
        int maxWeaponDamage;

        switch (occupation)
        {
            case "Fighter":
                weapon = "Sword";
                maxWeaponDamage = 20;
                break;

            case "Magician":
                weapon = "Staff";
                maxWeaponDamage = 15;
                break;

            case "Thief":
                weapon = "Dagger";
                maxWeaponDamage = 12;
                break;

            default:
                weapon = "Bow";
                maxWeaponDamage = 18;
                break;
        }

       
        int strength = Dice.Roll(6);
        int agility = Dice.Roll(6);
        int health = 100;

        
        return new Character(
            name,
            race,
            occupation,
            strength,
            agility,
            health,
            weapon,
            maxWeaponDamage);
    }

    // Display a menu and require a valid selection
    private static string AskChoice(
        string question,
        params string[] choices)
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine(question);

            for (int i = 0; i < choices.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {choices[i]}");
            }

            Console.Write("Enter your choice: ");
            string input = Console.ReadLine() ?? "";

            if (int.TryParse(input, out int selection) &&
                selection >= 1 &&
                selection <= choices.Length)
            {
                return choices[selection - 1];
            }

            Console.WriteLine("Invalid choice. Please try again.");
        }
    }
}
