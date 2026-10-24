using System;
using System.IO;
using CYOA;

namespace CYOA;

public class Game
{
    public const string InvalidChoiceMessage = "Invalid choice. Type '1' or '2'.";

    // The design documents do not define the dragon's name. Placeholder.
    public const string DefaultDragonName = "Dragon";

    private readonly Messages _messages;
    private readonly bool _loadMessagesFromFile;
    private readonly TextReader _input;
    private readonly TextWriter _output;
    private readonly Action<Dice> _createPlayer;

    // Set when the game starts. Exposed so tests and later flows can reuse them.
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
            LoadDefaultMessages();
        }

        Dice dice = Dice ?? new Dice();
        Dice = dice;
        Dragon = Dragon ?? new Dragon(DefaultDragonName);

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

    public const int PathPromptMessageId = 30;
    public const int NorthPathMessageId = 31;
    public const int SouthPathMessageId = 32;
    public const int DragonSaysMessageId = 33;      // contains {0} for the dragon's name
    public const int MagicRingMessageId = 34;
    public const int RetreatOrAttackPromptMessageId = 35;
    public const int InvalidAdventureChoiceMessageId = 36;
    public void StartAdventure(Action startAttack)
    {
        Dragon dragon = Dragon ?? throw new InvalidOperationException(
            "The game has not been started, so there is no dragon.");

        string pathChoice = ReadChoice(PathPromptMessageId, "n", "s", "e");
        if (pathChoice == "e")
        {
            EndGame();
            return;
        }

        _output.WriteLine(_messages.GetMessage(
            pathChoice == "n" ? NorthPathMessageId : SouthPathMessageId));

        Console.ForegroundColor = ConsoleColor.Red;
        try
        {
            _output.WriteLine(string.Format(
                _messages.GetMessage(DragonSaysMessageId), dragon.DragonName));
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

    public const int RetreatMessageId = 20;
    public const int DragonDefeatedMessageId = 21;
    public const int PlayerDefeatedMessageId = 22;

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

    private void LoadDefaultMessages()
    {
        string? filePath = FindMessageFile("English.txt");

        if (filePath == null)
        {
            throw new FileNotFoundException(
                "Could not find MessageFiles/English.txt. " +
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
        Player = Character.Create();
    }
}
