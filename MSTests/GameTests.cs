using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CYOA.Tests;

[TestClass]
public class GameTests
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
            $"4={Goodbye}\n" +
            $"5={Welcome}\n" +
            $"6={MenuPrompt}\n" +
            $"{Game.RetreatMessageId}=Retreat\n" +
            $"{Game.DragonDefeatedMessageId}=Victory\n" +
            $"{Game.PlayerDefeatedMessageId}=Defeat\n" +
            $"{Game.PathPromptMessageId}=Path prompt\n" +
            $"{Game.NorthPathMessageId}=North path\n" +
            $"{Game.SouthPathMessageId}=South path\n" +
            $"{Game.DragonSaysMessageId}=I am {{dragonName}}\n" +
            $"{Game.MagicRingMessageId}=Magic ring\n" +
            $"{Game.RetreatOrAttackPromptMessageId}=Fight prompt\n" +
            $"{Game.InvalidAdventureChoiceMessageId}=Invalid adventure choice\n");
        _messages = new Messages();
        _messages.ReadDictionary(_messageFile);
    }

    [TestCleanup]
    public void Cleanup() => File.Delete(_messageFile);

    // ---------------------------------------------------------------
    // Helpers
    // ---------------------------------------------------------------

    private static string[] SplitLines(string text) =>
        text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

    private static int CountLines(string[] lines, string value)
    {
        int n = 0;
        foreach (string line in lines)
        {
            if (line == value) n++;
        }
        return n;
    }

    private static int CountText(string text, string value)
    {
        int n = 0, i = 0;
        while ((i = text.IndexOf(value, i, StringComparison.Ordinal)) >= 0)
        {
            n++;
            i += value.Length;
        }
        return n;
    }

    private (string[] Lines, int CreateCalls) RunMenu(string input) =>
        RunMenuWithGame(input, out _);

    private (string[] Lines, int CreateCalls) RunMenuWithGame(string input, out Game game)
    {
        int calls = 0;
        var output = new StringWriter();
        game = new Game(_messages, new StringReader(input), output, _ => calls++);

        game.StartGame();

        return (SplitLines(output.ToString()), calls);
    }

    private (string Output, int AttackCalls) RunAdventure(string input)
    {
        int attackCalls = 0;
        var output = new StringWriter();
        var game = new Game(_messages, new StringReader(input), output,
            _ => { }, new Dice(), new Dragon("Smaug"));

        game.StartAdventure(() => attackCalls++);

        return (output.ToString(), attackCalls);
    }

    // ---------------------------------------------------------------
    // Game initialization and main menu
    // ---------------------------------------------------------------

    [TestMethod]
    public void DiceAndDragonAreInstantiatedOnStart()
    {
        RunMenuWithGame("2\n", out Game game);
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
        var (lines, _) = RunMenu("2\n");
        Assert.AreEqual(Welcome, lines[0]);
        Assert.AreEqual(MenuPrompt, lines[1]);
    }

    [TestMethod]
    public void Option1StartsCreatePlayer()
    {
        var (_, calls) = RunMenu("1\n");
        Assert.AreEqual(1, calls);
    }

    [TestMethod]
    public void Option2EndsGameWithoutCreatePlayer()
    {
        var (lines, calls) = RunMenu("2\n");
        Assert.AreEqual(0, calls);
        Assert.AreEqual(1, CountLines(lines, Goodbye));
        Assert.AreEqual(0, CountLines(lines, Game.InvalidChoiceMessage));
    }

    [TestMethod]
    public void WhitespaceIsIgnored()
    {
        var (_, calls) = RunMenu(" 1 \n");
        Assert.AreEqual(1, calls);
    }

    [TestMethod]
    [DataRow("3")]
    [DataRow("abc")]
    [DataRow("")]
    [DataRow("1.0")]
    public void InvalidInputRepromptsWithoutExitingOrCreatingPlayer(string bad)
    {
        var (lines, calls) = RunMenu(bad + "\n2\n");
        Assert.AreEqual(1, CountLines(lines, Game.InvalidChoiceMessage));
        Assert.AreEqual(2, CountLines(lines, MenuPrompt));
        Assert.AreEqual(0, calls);
    }

    [TestMethod]
    public void InvalidThenOption1StartsCreatePlayer()
    {
        var (_, calls) = RunMenu("x\n1\n");
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

    // ---------------------------------------------------------------
    // End Game
    // ---------------------------------------------------------------

    [TestMethod]
    [DataRow(EndCondition.CharacterRetreats, "Retreat")]
    [DataRow(EndCondition.DragonDefeated, "Victory")]
    [DataRow(EndCondition.PlayerDefeated, "Defeat")]
    public void EndGamePrintsOnlyTheMessageForTheCondition(EndCondition condition, string expected)
    {
        var output = new StringWriter();
        var game = new Game(_messages, new StringReader(""), output, _ => { });

        game.EndGame(condition);

        string[] lines = SplitLines(output.ToString());
        Assert.AreEqual(1, lines.Length);
        Assert.AreEqual(expected, lines[0]);
    }

    // ---------------------------------------------------------------
    // Start Adventure (path choice, dragon encounter, retreat or attack)
    // ---------------------------------------------------------------

    [TestMethod]
    public void NorthPathShowsNorthMessageThenDragonEncounter()
    {
        var (output, _) = RunAdventure("n\nr\n");
        StringAssert.Contains(output, "North path");
        Assert.IsFalse(output.Contains("South path"));
        StringAssert.Contains(output, "I am Smaug");
        StringAssert.Contains(output, "Magic ring");
        StringAssert.Contains(output, "--- Dragon Statistics ---");
        StringAssert.Contains(output, "Fight prompt");
    }

    [TestMethod]
    public void SouthPathShowsSouthMessageThenReturnsToPathPrompt()
    {
        var (output, attackCalls) = RunAdventure("s\ne\n");
        Assert.AreEqual(1, CountText(output, "South path"));
        Assert.AreEqual(2, CountText(output, "Path prompt"));
        Assert.IsFalse(output.Contains("North path"));
        Assert.IsFalse(output.Contains("I am Smaug"));
        Assert.AreEqual(1, CountText(output, Goodbye));
        Assert.AreEqual(0, attackCalls);
    }

    [TestMethod]
    public void SouthThenNorthContinuesToDragonEncounter()
    {
        var (output, _) = RunAdventure("s\nn\nr\n");
        Assert.AreEqual(1, CountText(output, "South path"));
        Assert.AreEqual(2, CountText(output, "Path prompt"));
        StringAssert.Contains(output, "North path");
        StringAssert.Contains(output, "I am Smaug");
    }

    [TestMethod]
    public void ExitChoiceShowsEndGameMessageAndStopsBeforeDragon()
    {
        var (output, attackCalls) = RunAdventure("e\n");
        Assert.AreEqual(1, CountText(output, Goodbye));
        Assert.IsFalse(output.Contains("I am Smaug"));
        Assert.IsFalse(output.Contains("Fight prompt"));
        Assert.AreEqual(0, attackCalls);
    }

    [TestMethod]
    public void InvalidPathChoiceRepromptsThenContinues()
    {
        var (output, _) = RunAdventure("x\nn\nr\n");
        Assert.AreEqual(1, CountText(output, "Invalid adventure choice"));
        Assert.AreEqual(2, CountText(output, "Path prompt"));
        StringAssert.Contains(output, "North path");
    }

    [TestMethod]
    public void RetreatShowsRetreatEndingAndDoesNotAttack()
    {
        var (output, attackCalls) = RunAdventure("n\nr\n");
        Assert.AreEqual(1, CountText(output, "Retreat"));
        Assert.AreEqual(0, attackCalls);
    }

    [TestMethod]
    public void AttackStartsAttackFlowWithoutEnding()
    {
        var (output, attackCalls) = RunAdventure("n\na\n");
        Assert.AreEqual(1, attackCalls);
        Assert.IsFalse(output.Contains("Retreat"));
        Assert.IsFalse(output.Contains(Goodbye));
    }

    [TestMethod]
    public void InvalidFightChoiceReprompts()
    {
        var (output, attackCalls) = RunAdventure("n\nx\na\n");
        Assert.AreEqual(1, CountText(output, "Invalid adventure choice"));
        Assert.AreEqual(2, CountText(output, "Fight prompt"));
        Assert.AreEqual(1, attackCalls);
    }
}