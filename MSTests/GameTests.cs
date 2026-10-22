namespace CYOA.Tests;
 
[TestClass]
public class GameMenuTests
{
    private const string Welcome = "Welcome test message";
    private const string MenuPrompt = "Menu prompt test message";
    private const string Goodbye = "Goodbye test message";
 
    private string _messageFile = "";
    private Messages _messages = new();
 
    [TestInitialize]
    public void Setup()
    {
        _messageFile = Path.GetTempFileName();
        File.WriteAllText(_messageFile,
            $"4={Goodbye}\n5={Welcome}\n6={MenuPrompt}\n");
        _messages = new Messages();
        _messages.ReadDictionary(_messageFile);
    }
 
    [TestCleanup]
    public void Cleanup() => File.Delete(_messageFile);
 
    private (string[] Lines, int CreateCalls) Run(string input) =>
        RunWithGame(input, out _);
 
    private (string[] Lines, int CreateCalls) RunWithGame(string input, out Game game)
    {
        int calls = 0;
        var output = new StringWriter();
        game = new Game(_messages, new StringReader(input), output, _ => calls++);
 
        game.StartGame();
 
        string[] lines = output.ToString()
            .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        return (lines, calls);
    }
 
    private static int Count(string[] lines, string value)
    {
        int n = 0;
        foreach (string line in lines)
        {
            if (line == value) n++;
        }
        return n;
    }
 
    [TestMethod]
    public void DiceAndDragonAreInstantiatedOnStart()
    {
        RunWithGame("2\n", out Game game);
        Assert.IsNotNull(game.Dice);
        Assert.IsNotNull(game.Dragon);
    }
 
    [TestMethod]
    public void InjectedDiceAndDragonAreKept()
    {
        var dice = new Dice();
        var dragon = new Dragon("Test");
        var game = new Game(_messages, new StringReader("2\n"), new StringWriter(),
            _ => { }, dice, dragon);
 
        game.StartGame();
 
        Assert.AreSame(dice, game.Dice);
        Assert.AreSame(dragon, game.Dragon);
    }
 
    [TestMethod]
    public void StartupDiceIsPassedToCreatePlayer()
    {
        Dice? received = null;
        var game = new Game(_messages, new StringReader("1\n"), new StringWriter(),
            d => received = d);
 
        game.StartGame();
 
        Assert.IsNotNull(received);
        Assert.AreSame(game.Dice, received);
    }
 
    [TestMethod]
    public void ShowsWelcomeThenMenuPrompt()
    {
        var (lines, _) = Run("2\n");
        Assert.AreEqual(Welcome, lines[0]);
        Assert.AreEqual(MenuPrompt, lines[1]);
    }
 
    [TestMethod]
    public void Option1StartsCreatePlayer()
    {
        var (_, calls) = Run("1\n");
        Assert.AreEqual(1, calls);
    }
 
    [TestMethod]
    public void Option2EndsGameWithoutCreatePlayer()
    {
        var (lines, calls) = Run("2\n");
        Assert.AreEqual(0, calls);
        Assert.AreEqual(1, Count(lines, Goodbye));
        Assert.AreEqual(0, Count(lines, Game.InvalidChoiceMessage));
    }
 
    [TestMethod]
    public void WhitespaceIsIgnored()
    {
        var (_, calls) = Run(" 1 \n");
        Assert.AreEqual(1, calls);
    }
 
    [TestMethod]
    [DataRow("3")]
    [DataRow("abc")]
    [DataRow("")]
    [DataRow("1.0")]
    public void InvalidInputRepromptsWithoutExitingOrCreatingPlayer(string bad)
    {
        var (lines, calls) = Run(bad + "\n2\n");
        Assert.AreEqual(1, Count(lines, Game.InvalidChoiceMessage));
        Assert.AreEqual(2, Count(lines, MenuPrompt));
        Assert.AreEqual(0, calls);
    }
 
    [TestMethod]
    public void InvalidThenOption1StartsCreatePlayer()
    {
        var (_, calls) = Run("x\n1\n");
        Assert.AreEqual(1, calls);
    }
 
    [TestMethod]
    public void EndOfInputThrowsInsteadOfLoopingForever()
    {
        var game = new Game(_messages, new StringReader(""), new StringWriter(), _ => { });
        try
        {
            game.StartGame();
        }
        catch (EndOfStreamException)
        {
            return;
        }
        Assert.Fail("Expected EndOfStreamException.");
    }
}
