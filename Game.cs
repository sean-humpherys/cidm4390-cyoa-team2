namespace CYOA;

public class Game
{
    public const string InvalidChoiceMessage = "Invalid choice. Type '1' or '2'.";

    public const string DefaultDragonName = "Dragon";

    private readonly Messages _messages;
    private readonly bool _loadMessagesFromFile;
    private readonly TextReader _input;
    private readonly TextWriter _output;
    private readonly Action<Dice> _createPlayer;
    private readonly Dragon? _initialDragon;

    public Dice? Dice { get; private set; }
    public Dragon? Dragon { get; private set; }
    public Character? Player { get; private set; }

    public Game(
        Messages? messages = null,
        TextReader? input = null,
        TextWriter? output = null,
        Action<Dice>? createPlayer = null,
        Dice? dice = null,
        Dragon? dragon = null)
    {
        Dice = dice;
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

        Dice dice = Dice ?? new Dice();
        Dice = dice;
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
                    _createPlayer(dice);
                    return;

                case "2":
                    EndGame();
                    return;

                default:
                    _output.WriteLine(InvalidChoiceMessage);
                    break;
            }
        }
    }

    public const int PathPromptMessageId = 7;
    public const int NorthPathMessageId = 9;
    public const int SouthPathMessageId = 8;
    public const int DragonSaysMessageId = 10;      
    public const int MagicRingMessageId = 11;
    public const int RetreatOrAttackPromptMessageId = 12;
    public const int InvalidAdventureChoiceMessageId = 52;

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
            EndGame();
            return;
        }

        _output.WriteLine(_messages.GetMessage(NorthPathMessageId));

        Console.ForegroundColor = ConsoleColor.Red;
        try
        {
            _output.WriteLine(Format(
                _messages.GetMessage(DragonSaysMessageId),
                ("dragonName", dragon.DragonName)));
        }
        finally
        {
            Console.ResetColor();
        }

        _output.WriteLine(_messages.GetMessage(MagicRingMessageId));
        _output.WriteLine(dragon.DisplayStats());

        string fightChoice = ReadChoice(RetreatOrAttackPromptMessageId, "r", "a");
        if (fightChoice == "r")
        {
            EndGame(EndCondition.CharacterRetreats);
            return;
        }

        startAttack();
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
            if (Array.IndexOf(validChoices, choice) >= 0)
            {
                return choice;
            }

            _output.WriteLine(_messages.GetMessage(InvalidAdventureChoiceMessageId));
        }
    }
    public const int RetreatMessageId = 41;
    public const int DragonDefeatedMessageId = 42;
    public const int PlayerDefeatedMessageId = 43;

    public void EndGame()
    {
        _output.WriteLine(_messages.GetMessage(4));
    }


    public void EndGame(EndCondition condition)
    {
        int messageId = condition switch
        {
            EndCondition.CharacterRetreats => RetreatMessageId,
            EndCondition.DragonDefeated => DragonDefeatedMessageId,
            EndCondition.PlayerDefeated => PlayerDefeatedMessageId,
            _ => throw new ArgumentOutOfRangeException(nameof(condition))
        };

        _output.WriteLine(_messages.GetMessage(messageId));
    }

    public const int AttackIntroMessageId = 20;        // {occupation}, {weapon}
    public const int AttackPromptMessageId = 25;
    public const int AttackRetreatMessageId = 26;
    public const int FighterArtMessageId = 53;
    public const int MagicianArtMessageId = 54;
    public const int ThiefArtMessageId = 55;
    public const int ArcherArtMessageId = 56;

    public bool StartAttack(Combat combat)
    {
        Character player = Player ?? throw new InvalidOperationException(
            "There is no character. Create one before attacking.");
        Dragon dragon = Dragon ?? throw new InvalidOperationException(
            "The game has not been started, so there is no dragon.");

        _output.WriteLine(Format(
            _messages.GetMessage(AttackIntroMessageId),
            ("occupation", player.Occupation),
            ("weapon", player.Weapon)));

        int? artMessageId = player.Occupation switch
        {
            "Fighter" => FighterArtMessageId,
            "Magician" => MagicianArtMessageId,
            "Thief" => ThiefArtMessageId,
            "Archer" => ArcherArtMessageId,
            _ => null
        };
        if (artMessageId != null)
        {
            _output.WriteLine(_messages.GetMessage(artMessageId.Value));
        }

        while (true)
        {
            combat.PlayerAttack(player, dragon);
            if (dragon.DragonHealth <= 0)
            {
                EndGame(EndCondition.DragonDefeated);
                return false;
            }

            combat.DragonAttack(dragon, player);
            if (player.Health <= 0)
            {
                EndGame(EndCondition.PlayerDefeated);
                return false;
            }

            if (ReadChoice(AttackPromptMessageId, "r", "a") == "r")
            {
                _output.WriteLine(_messages.GetMessage(AttackRetreatMessageId));
                return true;
            }
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
            DirectoryInfo? directory =
                new DirectoryInfo(startingDirectory);

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

    private void CreatePlayer(Dice dice)
    {
        Player = Character.Create(_input, _output, null, _messages);
    }
}