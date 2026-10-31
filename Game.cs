using System.Globalization;

namespace CYOA;
public class Game
{
    public const string DefaultDragonName = "Dagobah";

    private readonly Messages _messages;
    private readonly bool _loadMessagesFromFile;
    private readonly TextReader _input;
    private readonly TextWriter _output;
    private readonly Action<Die> _createPlayer;
    private readonly Dragon? _initialDragon;

    public Die? Die { get; private set; }
    public Dragon? Dragon { get; private set; }
    public Player? Player { get; private set; }

    public Game(
        Messages? messages = null,
        TextReader? input = null,
        TextWriter? output = null,
        Action<Die>? createPlayer = null,
        Die? die = null,
        Dragon? dragon = null)
    {
        Die = die;
        Dragon = dragon;
        _initialDragon = dragon;
        _loadMessagesFromFile = messages == null;
        _messages = messages ?? new Messages();
        _input = input ?? Console.In;
        _output = output ?? Console.Out;
        _createPlayer = createPlayer ?? CreatePlayer;
    }

    public void StartGame()
    {
        if (_loadMessagesFromFile)
        {
            SelectLanguage();
            LoadDefaultMessages();
        }

        Die die = Die ?? new Die();
        Die = die;
        Dragon = _initialDragon ?? new Dragon(DefaultDragonName);
        Player = null;

        _output.WriteLine(_messages.GetMessage(5));

        while (true)
        {
            _output.WriteLine(_messages.GetMessage(6));

            string? line = _input.ReadLine();
            if (line == null)
            {
                throw new EndOfStreamException(
                    "Input ended before a valid menu choice was entered.");
            }

            switch (line.Trim())
            {
                case "1":
                    _createPlayer(die);

                    if (Player != null)
                    {
                        StartAdventure(() => StartAttack());
                    }
                    return;

                case "2":
                    return;

                default:
                    _output.WriteLine(_messages.GetMessage(52));
                    break;
            }
        }
    }

    public void StartAdventure(Action startAttack)
    {
        Dragon dragon = Dragon ?? throw new InvalidOperationException(
            "The game has not been started, so there is no dragon.");

        string pathChoice;
        while (true)
        {
            pathChoice = ReadChoice(7, "n", "s", "e");
            if (pathChoice != "s")
            {
                break;
            }

            _output.WriteLine(_messages.GetMessage(8));
        }

        if (pathChoice == "e")
        {
            return;
        }

        _output.WriteLine(_messages.GetMessage(9));

        Console.ForegroundColor = ConsoleColor.Red;
        try
        {
            _output.WriteLine(Format(
                _messages.GetMessage(10),
                ("dragonName", dragon.DragonName)));
        }
        finally
        {
            Console.ResetColor();
        }

        _output.WriteLine(_messages.GetMessage(11));
        _output.WriteLine(dragon.DisplayStats(_messages));

        string fightChoice = ReadChoice(12, "r", "a");
        if (fightChoice == "r")
        {
            _output.WriteLine(_messages.GetMessage(41));
            return;
        }

        startAttack();
    }

    public bool StartAttack()
    {
        Player player = Player ?? throw new InvalidOperationException(
            "There is no character. Create one before attacking.");
        Dragon dragon = Dragon ?? throw new InvalidOperationException(
            "The game has not been started, so there is no dragon.");

        var combat = new Combat(_messages, _output);

        _output.WriteLine(Format(
            _messages.GetMessage(20),
            ("occupation", player.Occupation.ToString()),
            ("weapon", player.Weapon.type)));

        _output.WriteLine(_messages.GetMessage(ArtMessageIdFor(player.Occupation)));

        while (true)
        {
            combat.PlayerAttack(player, dragon);
            if (dragon.DragonHealth <= 0)
            {
                _output.WriteLine(_messages.GetMessage(42));
                return false;
            }

            combat.DragonAttack(dragon, player);
            if (player.Health <= 0)
            {
                _output.WriteLine(_messages.GetMessage(43));
                return false;
            }

            if (ReadChoice(25, "r", "a") == "r")
            {
                _output.WriteLine(_messages.GetMessage(26));
                return true;
            }
        }
    }

    private static int ArtMessageIdFor(Occupation occupation) => occupation switch
    {
        Occupation.Fighter => 53,
        Occupation.Magician => 54,
        Occupation.Thief => 55,
        _ => 56
    };

    private void CreatePlayer(Die die)
    {
        Race race = Enum.Parse<Race>(ReadChoice(13, Enum.GetNames<Race>()), true);
        string name = ReadName();
        Occupation occupation = Enum.Parse<Occupation>(ReadChoice(15, Enum.GetNames<Occupation>()), true);

        Weapon weapon = WeaponFor(occupation);
        weapon.asciiArt = _messages.GetMessage(ArtMessageIdFor(occupation));

        bool isHalfling = race == Race.Halfling;

        // Strength: 1-20, then +1-4 (Human, Elf, Dwarf) or -1-4 (Halfling).
        ReadChoice(16, "roll");
        int strengthModifier = Die.Roll(4);
        int strength = Die.Roll(20) + (isHalfling ? -strengthModifier : strengthModifier);
        _output.WriteLine($"{_messages.GetMessage(30)}: {strength}");

        // Agility: 1-20, then +1-4 (Halfling, Elf). Human and Dwarf have no modifier.
        ReadChoice(17, "roll");
        int agility = Die.Roll(20);
        if (isHalfling || race == Race.Elf)
        {
            agility += Die.Roll(4);
        }
        _output.WriteLine($"{_messages.GetMessage(31)}: {agility}");

        // Health: 1-20, then add the strength modifier (Human, Elf, Dwarf). Halfling has none.
        ReadChoice(18, "roll");
        int health = Die.Roll(20) + (isHalfling ? 0 : strengthModifier);
        _output.WriteLine($"{_messages.GetMessage(32)}: {health}");

        Player = new Player(name, race, occupation, strength, agility, health, weapon);
    }

    private static Weapon WeaponFor(Occupation occupation) => occupation switch
    {
        Occupation.Fighter => new Weapon("Sword", 20, ""),
        Occupation.Magician => new Weapon("Staff", 15, ""),
        Occupation.Thief => new Weapon("Dagger", 12, ""),
        _ => new Weapon("Bow", 18, "")
    };

    private string ReadName()
    {
        while (true)
        {
            _output.WriteLine(_messages.GetMessage(14));

            string? line = _input.ReadLine();
            if (line == null)
            {
                throw new EndOfStreamException(
                    "Input ended before a name was entered.");
            }

            string name = line.Trim();
            if (name.Length > 0)
            {
                return name;
            }

            _output.WriteLine(_messages.GetMessage(52));
        }
    }

    private static string Format(string template, params (string Key, string Value)[] values)
    {
        foreach (var (key, value) in values)
        {
            template = template.Replace("{" + key + "}", value);
        }
        return template;
    }

    private string ReadChoice(int promptMessageId, params string[] validChoices)
    {
        int index = ReadChoiceIndex(promptMessageId, validChoices.Select(c => new[] { c }).ToArray());
        return validChoices[index];
    }

    private int ReadChoiceIndex(int promptMessageId, params string[][] choices)
    {
        while (true)
        {
            _output.WriteLine(_messages.GetMessage(promptMessageId));

            string? line = _input.ReadLine();
            if (line == null)
            {
                throw new EndOfStreamException(
                    "Input ended before a valid choice was entered.");
            }

            string answer = line.Trim();
            int index = Array.FindIndex(choices,
                words => words.Any(w => string.Compare(w, answer, CultureInfo.InvariantCulture,
                    CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace) == 0));
            if (index >= 0)
            {
                return index;
            }

            _output.WriteLine(_messages.GetMessage(52));
        }
    }

    private void SelectLanguage()
    {
        while (true)
        {
            _output.WriteLine("Choose your language / Choisissez votre langue / Elige tu idioma:");
            _output.WriteLine("1. English");
            _output.WriteLine("2. Français");
            _output.WriteLine("3. Español");

            string? choice = _input.ReadLine()?.Trim();

            switch (choice)
            {
                case "1":
                    _messages.CurrentLanguage = "English";
                    return;

                case "2":
                    _messages.CurrentLanguage = "French";
                    return;

                case "3":
                    _messages.CurrentLanguage = "Spanish";
                    return;

                default:
                    _output.WriteLine("Invalid choice. Try again.");
                    break;
            }
        }
    }

    private void LoadDefaultMessages()
    {
        string fileName = _messages.CurrentLanguage switch
        {
            "English" => "English.json",
            "French" => "French.json",
            "Spanish" => "Spanish.json",
            _ => "English.json"
        };

        string? filePath = FindMessageFile(fileName);

        if (filePath == null)
        {
            throw new FileNotFoundException(
                $"Could not find MessageFiles/{fileName}. " +
                "Check that the MessageFiles folder is available.");
        }

_messages.ReadDictionary(filePath);
    }

    private static string? FindMessageFile(string fileName)
    {
        string[] startingDirectories =
        {
            AppContext.BaseDirectory,
            Directory.GetCurrentDirectory()
        };

        foreach (string startingDirectory in startingDirectories)
        {
            DirectoryInfo? directory = new DirectoryInfo(startingDirectory);

            while (directory != null)
            {
                string path = Path.Combine(
                    directory.FullName,
                    "MessageFiles",
                    fileName);

                if (File.Exists(path))
                {
                    return path;
                }

                directory = directory.Parent;
            }
        }

        return null;
    }
}