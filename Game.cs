using CYOA;

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
 
    public Dice? Dice { get; private set; }
    public Dragon? Dragon { get; private set; }
 
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
 
    public void EndGame()
    {
        _output.WriteLine(_messages.GetMessage(4));
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
        Console.WriteLine(_messages.GetMessage(13));
        string? raceInput = Console.ReadLine()?.Trim();
 
        Console.WriteLine(_messages.GetMessage(14));
        string? name = Console.ReadLine()?.Trim();
 
        Console.WriteLine(_messages.GetMessage(15));
        string? occupationInput = Console.ReadLine()?.Trim();
 
        if (string.IsNullOrWhiteSpace(raceInput) ||
            string.IsNullOrWhiteSpace(name) ||
            string.IsNullOrWhiteSpace(occupationInput))
        {
            Console.WriteLine(
                "Character information cannot be empty.");
            return;
        }
 
        if (!Enum.TryParse<Race>(
                raceInput, true, out Race race) ||
            !Enum.TryParse<Occupation>(
                occupationInput, true, out Occupation occupation))
        {
            Console.WriteLine(
                "Invalid race or occupation. Please try again.");
            return;
        }
 
        Player player = new Player(name, race, occupation);
 
        Console.WriteLine(player.DisplayStats());
    }

    private class Player
    {
        public Player(string name, Race race, Occupation occupation)
        {
        }

        internal bool DisplayStats()
        {
            throw new NotImplementedException();
        }
    }
}

internal struct Occupation
{
}

internal struct Race
{
}