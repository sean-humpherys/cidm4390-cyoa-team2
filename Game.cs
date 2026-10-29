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
   
    public const int PathPromptMessageId = 7;
    public const int SouthPathMessageId = 8;
    public const int NorthPathMessageId = 9;
    public const int DragonSaysMessageId = 10;
    public const int MagicRingMessageId = 11;
    public const int RetreatOrAttackPromptMessageId = 12;
    public const int RacePromptMessageId = 13;
    public const int NamePromptMessageId = 14;
    public const int OccupationPromptMessageId = 15;
    public const int StrengthPromptMessageId = 16;
    public const int AgilityPromptMessageId = 17;
    public const int HealthPromptMessageId = 18;
    public const int AttackIntroMessageId = 20;        
    public const int AttackPromptMessageId = 25;
    public const int AttackRetreatMessageId = 26;
    public const int RetreatMessageId = 41;
    public const int DragonDefeatedMessageId = 42;
    public const int PlayerDefeatedMessageId = 43;
    public const int InvalidAdventureChoiceMessageId = 52;
    public const int FighterArtMessageId = 53;
    public const int MagicianArtMessageId = 54;
    public const int ThiefArtMessageId = 55;
    public const int ArcherArtMessageId = 56;
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
                    return;

                case "2":
                    return;

                default:
                    _output.WriteLine(_messages.GetMessage(InvalidAdventureChoiceMessageId));
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
            pathChoice = ReadChoice(PathPromptMessageId, "n", "s", "e");
            if (pathChoice != "s")
            {
                break;
            }

            _output.WriteLine(_messages.GetMessage(SouthPathMessageId));
        }

        if (pathChoice == "e")
        {
            return;
        }

        _output.WriteLine(_messages.GetMessage(NorthPathMessageId));

        Console.ForegroundColor = ConsoleColor.Red;
        try
        {
            _output.WriteLine(Format(
                _messages.GetMessage(DragonSaysMessageId),
                ("dragonName", dragon.Name)));
        }
        finally
        {
            Console.ResetColor();
        }

        _output.WriteLine(_messages.GetMessage(MagicRingMessageId));
        _output.WriteLine(dragon.DisplayStats(_messages));

        string fightChoice = ReadChoice(RetreatOrAttackPromptMessageId, "r", "a");
        if (fightChoice == "r")
        {
            _output.WriteLine(_messages.GetMessage(RetreatMessageId));
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

        var combat = new Combat(player, dragon, _messages);

        _output.WriteLine(Format(
            _messages.GetMessage(AttackIntroMessageId),
            ("occupation", player.Occupation.ToString()),
            ("weapon", player.Weapon.type)));

        _output.WriteLine(_messages.GetMessage(ArtMessageIdFor(player.Occupation)));

        while (true)
        {
            combat.PlayerAttacksDragonSequence();
            if (dragon.Health <= 0)
            {
                _output.WriteLine(_messages.GetMessage(DragonDefeatedMessageId));
                return false;
            }

            combat.DragonAttacksPlayerSequence();
            if (player.Health <= 0)
            {
                _output.WriteLine(_messages.GetMessage(PlayerDefeatedMessageId));
                return false;
            }

            if (ReadChoice(AttackPromptMessageId, "r", "a") == "r")
            {
                _output.WriteLine(_messages.GetMessage(AttackRetreatMessageId));
                return true;
            }
        }
    }

    private static int ArtMessageIdFor(Occupation occupation) => occupation switch
    {
        Occupation.Fighter => FighterArtMessageId,
        Occupation.Magician => MagicianArtMessageId,
        Occupation.Thief => ThiefArtMessageId,
        _ => ArcherArtMessageId
    };

    private void CreatePlayer(Die die)
    {
        Race race = Enum.Parse<Race>(
            ReadChoice(RacePromptMessageId, Enum.GetNames<Race>()), true);
        string name = ReadName();
        Occupation occupation = Enum.Parse<Occupation>(
            ReadChoice(OccupationPromptMessageId, Enum.GetNames<Occupation>()), true);

        var player = new Player(name, race, occupation);
        player.Weapon.asciiArt = _messages.GetMessage(ArtMessageIdFor(occupation));

        bool isHalfling = race == Race.Halfling;

        // Strength: 1-20, then +1-4 (Human, Elf, Dwarf) or -1-4 (Halfling).
        ReadChoice(StrengthPromptMessageId, "roll");
        int strengthModifier = Die.Roll(4);
        player.Strength = Die.Roll(20) + (isHalfling ? -strengthModifier : strengthModifier);
        _output.WriteLine($"{_messages.GetMessage(Messages.StrengthLabelId)}: {player.Strength}");

        // Agility: 1-20, then +1-4 (Halfling, Elf). Human and Dwarf have no modifier.
        ReadChoice(AgilityPromptMessageId, "roll");
        player.Agility = Die.Roll(20);
        if (isHalfling || race == Race.Elf)
        {
            player.Agility += Die.Roll(4);
        }
        _output.WriteLine($"{_messages.GetMessage(Messages.AgilityLabelId)}: {player.Agility}");

        // Health: 1-20, then add the strength modifier (Human, Elf, Dwarf). Halfling has none.
        ReadChoice(HealthPromptMessageId, "roll");
        player.Health = Die.Roll(20) + (isHalfling ? 0 : strengthModifier);
        _output.WriteLine($"{_messages.GetMessage(Messages.HealthLabelId)}: {player.Health}");

        Player = player;
    }

    private string ReadName()
    {
        while (true)
        {
            _output.WriteLine(_messages.GetMessage(NamePromptMessageId));

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

            _output.WriteLine(_messages.GetMessage(InvalidAdventureChoiceMessageId));
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

    // Matching ignores case ("Elf", "elf"); the canonical spelling from validChoices is returned.
    private string ReadChoice(int promptMessageId, params string[] validChoices)
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

            string choice = line.Trim();
            int index = Array.FindIndex(validChoices,
                c => string.Equals(c, choice, StringComparison.OrdinalIgnoreCase));
            if (index >= 0)
            {
                return validChoices[index];
            }

            _output.WriteLine(_messages.GetMessage(InvalidAdventureChoiceMessageId));
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

        _messages.ReadJsonDictionary(filePath);
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
                string path = Path.Combine(directory.FullName, "MessageFiles", fileName);

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