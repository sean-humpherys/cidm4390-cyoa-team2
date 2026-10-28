namespace CYOA.Tests;
 
[TestClass]
public class GameTests
{
    // ---------- helpers ----------
 
    private static string? FindMessageFile(string fileName)
    {
        string[] startingDirectories =
        {
            AppContext.BaseDirectory,
            Directory.GetCurrentDirectory()
        };
 
        foreach (string start in startingDirectories)
        {
            DirectoryInfo? dir = new DirectoryInfo(start);
            while (dir != null)
            {
                string path = Path.Combine(dir.FullName, "MessageFiles", fileName);
                if (File.Exists(path))
                {
                    return path;
                }
                dir = dir.Parent;
            }
        }
        return null;
    }
 
    private static Messages CreateEnglishMessages()
    {
        string? path = FindMessageFile("English.json");
        Assert.IsNotNull(path, "MessageFiles/English.json was not found.");
 
        var messages = new Messages();
        messages.CurrentLanguage = "English";
        messages.ReadJsonDictionary(path);
        return messages;
    }
 
    private sealed record Setup(Game Game, Messages Messages, StringWriter Output);
 
    private static Setup CreateGame(
        string input,
        Action<Dice>? createPlayer = null,
        Dice? dice = null,
        Dragon? dragon = null)
    {
        Messages messages = CreateEnglishMessages();
        var output = new StringWriter();
        var game = new Game(
            messages: messages,
            input: new StringReader(input),
            output: output,
            createPlayer: createPlayer ?? (_ => { }),
            dice: dice,
            dragon: dragon);
        return new Setup(game, messages, output);
    }
 
    private static void AssertOutputContains(StringWriter output, string expected)
    {
        Assert.IsTrue(
            output.ToString().Contains(expected),
            $"Expected output to contain: {expected}");
    }
 
    private static void AssertOutputDoesNotContain(StringWriter output, string unexpected)
    {
        Assert.IsFalse(
            output.ToString().Contains(unexpected),
            $"Expected output NOT to contain: {unexpected}");
    }
 
    // ---------- StartGame ----------
 
    [TestMethod]
    public void StartGame_ChoiceOne_CallsCreatePlayerWithDice()
    {
        var dice = new Dice();
        Dice? received = null;
        Setup s = CreateGame("1\n", createPlayer: d => received = d, dice: dice);
 
        s.Game.StartGame();
 
        Assert.AreSame(dice, received);
    }
 
    [TestMethod]
    public void StartGame_ChoiceOne_ShowsWelcomeAndMenuMessages()
    {
        Setup s = CreateGame("1\n");
 
        s.Game.StartGame();
 
        AssertOutputContains(s.Output, s.Messages.GetMessage(5));
        AssertOutputContains(s.Output, s.Messages.GetMessage(6));
    }
 
    [TestMethod]
    public void StartGame_ChoiceTwo_EndsGameWithoutCreatingPlayer()
    {
        bool created = false;
        Setup s = CreateGame("2\n", createPlayer: _ => created = true);
 
        s.Game.StartGame();
 
        Assert.IsFalse(created);
        AssertOutputContains(s.Output, s.Messages.GetMessage(4));
    }
 
    [TestMethod]
    public void StartGame_InvalidChoiceThenValid_ShowsInvalidChoiceMessage()
    {
        Setup s = CreateGame("x\n\n2\n");
 
        s.Game.StartGame();
 
        AssertOutputContains(s.Output, Game.InvalidChoiceMessage);
        AssertOutputContains(s.Output, s.Messages.GetMessage(4));
    }
 
    [TestMethod]
    public void StartGame_InputEndsBeforeValidChoice_ThrowsEndOfStreamException()
    {
        Setup s = CreateGame("");
 
        Assert.ThrowsExactly<EndOfStreamException>(() => s.Game.StartGame());
    }
 
    [TestMethod]
    public void StartGame_NoDragonProvided_CreatesDefaultDragon()
    {
        Setup s = CreateGame("2\n");
 
        s.Game.StartGame();
 
        Assert.IsNotNull(s.Game.Dragon);
        Assert.AreEqual(Game.DefaultDragonName, s.Game.Dragon.DragonName);
    }
 
    [TestMethod]
    public void StartGame_DragonProvided_UsesProvidedDragon()
    {
        var dragon = new Dragon("Smaug");
        Setup s = CreateGame("2\n", dragon: dragon);
 
        s.Game.StartGame();
 
        Assert.AreSame(dragon, s.Game.Dragon);
    }
 
    [TestMethod]
    public void StartGame_NoDiceProvided_CreatesDice()
    {
        Setup s = CreateGame("2\n");
 
        s.Game.StartGame();
 
        Assert.IsNotNull(s.Game.Dice);
    }
 
    [TestMethod]
    public void StartGame_ResetsPlayerToNull()
    {
        Setup s = CreateGame("2\n");
 
        s.Game.StartGame();
 
        Assert.IsNull(s.Game.Player);
    }
 
    [TestMethod]
    public void StartGame_NoMessagesProvided_PromptsForLanguage()
    {
        // Depends on MessageFiles/English.json being findable from the test output folder.
        var output = new StringWriter();
        var game = new Game(
            input: new StringReader("x\n1\n2\n"),
            output: output,
            createPlayer: _ => { });
 
        game.StartGame();
 
        Assert.IsTrue(output.ToString().Contains("1. English"));
        Assert.IsTrue(output.ToString().Contains("Invalid choice. Try again."));
    }
 
    // ---------- EndGame ----------
 
    [TestMethod]
    public void EndGame_NoCondition_WritesGoodbyeMessage()
    {
        Setup s = CreateGame("");
 
        s.Game.EndGame();
 
        AssertOutputContains(s.Output, s.Messages.GetMessage(4));
    }
 
    [TestMethod]
    [DataRow(Game.EndCondition.CharacterRetreats, Game.RetreatMessageId)]
    [DataRow(Game.EndCondition.DragonDefeated, Game.DragonDefeatedMessageId)]
    [DataRow(Game.EndCondition.PlayerDefeated, Game.PlayerDefeatedMessageId)]
    public void EndGame_WithCondition_WritesMatchingMessage(
        Game.EndCondition condition, int messageId)
    {
        Setup s = CreateGame("");
 
        s.Game.EndGame(condition);
 
        AssertOutputContains(s.Output, s.Messages.GetMessage(messageId));
    }
 
    [TestMethod]
    public void EndGame_UndefinedCondition_ThrowsArgumentOutOfRange()
    {
        Setup s = CreateGame("");
 
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => s.Game.EndGame((Game.EndCondition)99));
    }
 
    // ---------- StartAdventure ----------
 
    [TestMethod]
    public void StartAdventure_GameNotStarted_ThrowsInvalidOperation()
    {
        Setup s = CreateGame("n\na\n");
 
        Assert.ThrowsExactly<InvalidOperationException>(
            () => s.Game.StartAdventure(() => { }));
    }
 
    [TestMethod]
    public void StartAdventure_ExitChoice_EndsGameWithoutAttack()
    {
        bool attacked = false;
        Setup s = CreateGame("e\n", dragon: new Dragon("Smaug"));
 
        s.Game.StartAdventure(() => attacked = true);
 
        Assert.IsFalse(attacked);
        AssertOutputContains(s.Output, s.Messages.GetMessage(Game.PathPromptMessageId));
        AssertOutputContains(s.Output, s.Messages.GetMessage(4));
    }
 
    [TestMethod]
    public void StartAdventure_SouthThenNorthThenAttack_ShowsSouthMessageAndStartsAttack()
    {
        bool attacked = false;
        var dragon = new Dragon("Smaug");
        Setup s = CreateGame("s\nn\na\n", dragon: dragon);
 
        s.Game.StartAdventure(() => attacked = true);
 
        Assert.IsTrue(attacked);
        AssertOutputContains(s.Output, s.Messages.GetMessage(Game.SouthPathMessageId));
        AssertOutputContains(s.Output, s.Messages.GetMessage(Game.NorthPathMessageId));
        AssertOutputContains(s.Output, s.Messages.GetMessage(Game.MagicRingMessageId));
        AssertOutputContains(s.Output, dragon.DisplayStats());
    }
 
    [TestMethod]
    public void StartAdventure_NorthThenRetreat_EndsWithRetreatMessageAndNoAttack()
    {
        bool attacked = false;
        Setup s = CreateGame("n\nr\n", dragon: new Dragon("Smaug"));
 
        s.Game.StartAdventure(() => attacked = true);
 
        Assert.IsFalse(attacked);
        AssertOutputContains(s.Output, s.Messages.GetMessage(Game.RetreatMessageId));
        AssertOutputDoesNotContain(s.Output, s.Messages.GetMessage(Game.SouthPathMessageId));
    }
 
    [TestMethod]
    public void StartAdventure_InvalidChoice_ShowsInvalidAdventureChoiceMessage()
    {
        Setup s = CreateGame("q\nn\nr\n", dragon: new Dragon("Smaug"));
 
        s.Game.StartAdventure(() => { });
 
        AssertOutputContains(
            s.Output, s.Messages.GetMessage(Game.InvalidAdventureChoiceMessageId));
    }
 
    [TestMethod]
    public void StartAdventure_InputEnds_ThrowsEndOfStreamException()
    {
        Setup s = CreateGame("", dragon: new Dragon("Smaug"));
 
        Assert.ThrowsExactly<EndOfStreamException>(
            () => s.Game.StartAdventure(() => { }));
    }
 
    // ---------- StartAttack ----------
 
    [TestMethod]
    public void StartAttack_NoPlayer_ThrowsInvalidOperation()
    {
        Setup s = CreateGame("", dragon: new Dragon("Smaug"));
 
        // Throws before combat is used, so null is safe here.
        Assert.ThrowsExactly<InvalidOperationException>(
            () => s.Game.StartAttack(null!));
    }
}