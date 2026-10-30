using CYOA;

namespace CYOA.Tests;

[TestClass]
public class GameTests
{
    // Character creation answers for menu option 1: race, name, occupation, then three rolls.
    private const string CreateElfThiefAnswers = "1\nElf\nEmber\nThief\nroll\nroll\nroll\n";

    // StartGame goes on into the adventure after creating the character; "e" exits at the path choice.
    private const string CreateElfThief = CreateElfThiefAnswers + "e\n";

    private const int WelcomeMessageId = 5;
    private const int MenuMessageId = 6;
    private const int PathPromptMessageId = 7;
    private const int SouthPathMessageId = 8;
    private const int NorthPathMessageId = 9;
    private const int DragonSaysMessageId = 10;
    private const int MagicRingMessageId = 11;
    private const int StrengthLabelId = 30;
    private const int AgilityLabelId = 31;
    private const int HealthLabelId = 32;
    private const int AttackIntroMessageId = 20;
    private const int AttackPromptMessageId = 25;
    private const int AttackRetreatMessageId = 26;
    private const int RetreatMessageId = 41;
    private const int DragonDefeatedMessageId = 42;
    private const int PlayerDefeatedMessageId = 43;
    private const int InvalidChoiceMessageId = 52;
    private const int ThiefArtMessageId = 55;

    // ---------- helpers ----------

    private static string? FindMessageFile(string fileName)
    {
        foreach (string start in new[] { AppContext.BaseDirectory, Directory.GetCurrentDirectory() })
        {
            for (DirectoryInfo? dir = new DirectoryInfo(start); dir != null; dir = dir.Parent)
            {
                string path = Path.Combine(dir.FullName, "MessageFiles", fileName);
                if (File.Exists(path))
                {
                    return path;
                }
            }
        }
        return null;
    }

    private static Messages CreateEnglishMessages()
    {
        string? path = FindMessageFile("English.json");
        Assert.IsNotNull(path, "MessageFiles/English.json was not found.");

        var messages = new Messages { CurrentLanguage = "English" };
        messages.ReadDictionary(path);
        return messages;
    }

    private sealed record Setup(Game Game, Messages Messages, StringWriter Output);

    // createPlayer defaults to doing nothing, so menu tests don't need character answers.
    private static Setup CreateGame(
        string input,
        Action<Die>? createPlayer = null,
        Die? die = null,
        Dragon? dragon = null)
    {
        Messages messages = CreateEnglishMessages();
        var output = new StringWriter();
        var game = new Game(messages, new StringReader(input), output, createPlayer ?? (_ => { }), die, dragon);
        return new Setup(game, messages, output);
    }

    // Uses the real character creation (Player.Create), so Game.Player is set after StartGame.
    private static Setup CreateGameWithPlayer(string input, Dragon? dragon = null)
    {
        Messages messages = CreateEnglishMessages();
        var output = new StringWriter();
        var game = new Game(messages, new StringReader(input), output, dragon: dragon);
        return new Setup(game, messages, output);
    }

    private static int CountText(string output, string text)
    {
        int count = 0;
        for (int i = output.IndexOf(text, StringComparison.Ordinal); i >= 0;
             i = output.IndexOf(text, i + text.Length, StringComparison.Ordinal))
        {
            count++;
        }
        return count;
    }

    private static void AssertOutputContains(StringWriter output, string text) =>
        Assert.IsTrue(output.ToString().Contains(text), $"Expected output to contain: {text}");

    // ---------- StartGame ----------

    [TestMethod]
    public void StartGame_ShowsWelcomeAndMenu()
    {
        Setup s = CreateGame("2\n");

        s.Game.StartGame();

        AssertOutputContains(s.Output, s.Messages.GetMessage(WelcomeMessageId));
        AssertOutputContains(s.Output, s.Messages.GetMessage(MenuMessageId));
    }

    [TestMethod]
    public void StartGame_ChoiceOne_CallsCreatePlayerWithDie()
    {
        var die = new Die();
        Die? received = null;
        Setup s = CreateGame("1\n", createPlayer: d => received = d, die: die);

        s.Game.StartGame();

        Assert.AreSame(die, received);
    }

    [TestMethod]
    public void StartGame_ChoiceTwo_EndsGameWithoutCreatingPlayer()
    {
        bool created = false;
        Setup s = CreateGame("2\n", createPlayer: _ => created = true);

        s.Game.StartGame();

        Assert.IsFalse(created);
        Assert.IsNull(s.Game.Player);
    }

    [TestMethod]
    [DataRow("x")]
    [DataRow("")]
    [DataRow("3")]
    public void StartGame_InvalidChoice_ShowsInvalidMessageAndAsksAgain(string bad)
    {
        Setup s = CreateGame(bad + "\n2\n");

        s.Game.StartGame();

        AssertOutputContains(s.Output, s.Messages.GetMessage(InvalidChoiceMessageId));
        Assert.AreEqual(2, CountText(s.Output.ToString(), s.Messages.GetMessage(MenuMessageId)));
    }

    [TestMethod]
    public void StartGame_InputEndsBeforeValidChoice_ThrowsEndOfStreamException()
    {
        Setup s = CreateGame("");

        Assert.ThrowsExactly<EndOfStreamException>(() => s.Game.StartGame());
    }

    [TestMethod]
    public void StartGame_NoDieProvided_CreatesDie()
    {
        Setup s = CreateGame("2\n");

        s.Game.StartGame();

        Assert.IsNotNull(s.Game.Die);
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

    // ---------- character creation (menu option 1) ----------

    [TestMethod]
    public void StartGame_ChoiceOne_CreatesPlayerFromAnswers()
    {
        Setup s = CreateGameWithPlayer(CreateElfThief);

        s.Game.StartGame();

        Player? player = s.Game.Player;
        Assert.IsNotNull(player);
        Assert.AreEqual("Ember", player.Name);
        Assert.AreEqual(Race.Elf, player.Race);
        Assert.AreEqual(Occupation.Thief, player.Occupation);
        Assert.AreEqual("Dagger", player.Weapon.type);
        Assert.AreEqual(12, player.Weapon.maxDamage);
        Assert.AreEqual(s.Messages.GetMessage(ThiefArtMessageId), player.Weapon.asciiArt);
    }

    [TestMethod]
    public void StartGame_ChoiceOne_ChoicesIgnoreCase()
    {
        Setup s = CreateGameWithPlayer("1\nDWARF\nEmber\narcher\nROLL\nRoll\nroll\ne\n");

        s.Game.StartGame();

        Assert.IsNotNull(s.Game.Player);
        Assert.AreEqual(Race.Dwarf, s.Game.Player.Race);
        Assert.AreEqual(Occupation.Archer, s.Game.Player.Occupation);
    }

    [TestMethod]
    public void StartGame_ChoiceOne_RejectsInvalidRaceAndEmptyName()
    {
        Setup s = CreateGameWithPlayer("1\nOrc\nElf\n\nEmber\nThief\nroll\nroll\nroll\ne\n");

        s.Game.StartGame();

        Assert.IsNotNull(s.Game.Player);
        Assert.AreEqual(Race.Elf, s.Game.Player.Race);
        Assert.AreEqual("Ember", s.Game.Player.Name);
        Assert.AreEqual(2, CountText(s.Output.ToString(), s.Messages.GetMessage(InvalidChoiceMessageId)));
    }

    [TestMethod]
    public void StartGame_ChoiceOne_ShowsRolledStats()
    {
        Setup s = CreateGameWithPlayer(CreateElfThief);

        s.Game.StartGame();

        Player player = s.Game.Player!;
        AssertOutputContains(s.Output, $"{s.Messages.GetMessage(StrengthLabelId)}: {player.Strength}");
        AssertOutputContains(s.Output, $"{s.Messages.GetMessage(AgilityLabelId)}: {player.Agility}");
        AssertOutputContains(s.Output, $"{s.Messages.GetMessage(HealthLabelId)}: {player.Health}");
    }

    [TestMethod]
    public void StartGame_ChoiceOne_RunsAdventureAfterCreatingPlayer()
    {
        Setup s = CreateGameWithPlayer(CreateElfThiefAnswers + "n\nr\n");

        s.Game.StartGame();

        AssertOutputContains(s.Output, s.Messages.GetMessage(PathPromptMessageId));
        AssertOutputContains(s.Output, s.Messages.GetMessage(NorthPathMessageId));
        AssertOutputContains(s.Output, s.Messages.GetMessage(RetreatMessageId));
    }

    [TestMethod]
    public void StartGame_ChoiceTwo_DoesNotStartAdventure()
    {
        Setup s = CreateGameWithPlayer("2\n");

        s.Game.StartGame();

        Assert.IsFalse(s.Output.ToString().Contains(s.Messages.GetMessage(PathPromptMessageId)));
    }

    // ---------- StartAdventure ----------

    [TestMethod]
    public void StartAdventure_GameNotStarted_ThrowsInvalidOperation()
    {
        var game = new Game(CreateEnglishMessages(), new StringReader(""), new StringWriter());

        Assert.ThrowsExactly<InvalidOperationException>(() => game.StartAdventure(() => { }));
    }

    [TestMethod]
    public void StartAdventure_Exit_EndsGameWithoutAttack()
    {
        bool attacked = false;
        Setup s = CreateGame("1\ne\n");
        s.Game.StartGame();

        s.Game.StartAdventure(() => attacked = true);

        Assert.IsFalse(attacked);
        Assert.IsFalse(s.Output.ToString().Contains(s.Messages.GetMessage(NorthPathMessageId)));
    }

    [TestMethod]
    public void StartAdventure_SouthReturnsToPathChoice()
    {
        Setup s = CreateGame("1\ns\ne\n");
        s.Game.StartGame();

        s.Game.StartAdventure(() => { });

        string output = s.Output.ToString();
        Assert.AreEqual(1, CountText(output, s.Messages.GetMessage(SouthPathMessageId)));
        Assert.AreEqual(2, CountText(output, s.Messages.GetMessage(PathPromptMessageId)));
        Assert.IsFalse(output.Contains(s.Messages.GetMessage(NorthPathMessageId)));
    }

    [TestMethod]
    public void StartAdventure_North_ShowsDragonEncounter()
    {
        var dragon = new Dragon("Smaug");
        Setup s = CreateGame("1\nn\nr\n", dragon: dragon);
        s.Game.StartGame();

        s.Game.StartAdventure(() => { });

        AssertOutputContains(s.Output, s.Messages.GetMessage(NorthPathMessageId));
        AssertOutputContains(s.Output, s.Messages.GetMessage(DragonSaysMessageId).Replace("{dragonName}", "Smaug"));
        AssertOutputContains(s.Output, s.Messages.GetMessage(MagicRingMessageId));
        AssertOutputContains(s.Output, dragon.DisplayStats(s.Messages));
    }

    [TestMethod]
    public void StartAdventure_NorthThenRetreat_ShowsRetreatEndingWithoutAttack()
    {
        bool attacked = false;
        Setup s = CreateGame("1\nn\nr\n");
        s.Game.StartGame();

        s.Game.StartAdventure(() => attacked = true);

        Assert.IsFalse(attacked);
        AssertOutputContains(s.Output, s.Messages.GetMessage(RetreatMessageId));
    }

    [TestMethod]
    public void StartAdventure_NorthThenAttack_StartsAttack()
    {
        bool attacked = false;
        Setup s = CreateGame("1\nn\na\n");
        s.Game.StartGame();

        s.Game.StartAdventure(() => attacked = true);

        Assert.IsTrue(attacked);
    }

    [TestMethod]
    public void StartAdventure_InvalidChoice_ShowsInvalidMessageAndAsksAgain()
    {
        Setup s = CreateGame("1\nx\ne\n");
        s.Game.StartGame();

        s.Game.StartAdventure(() => { });

        AssertOutputContains(s.Output, s.Messages.GetMessage(InvalidChoiceMessageId));
        Assert.AreEqual(2, CountText(s.Output.ToString(), s.Messages.GetMessage(PathPromptMessageId)));
    }

    [TestMethod]
    public void StartAdventure_InputEnds_ThrowsEndOfStreamException()
    {
        Setup s = CreateGame("1\n");
        s.Game.StartGame();

        Assert.ThrowsExactly<EndOfStreamException>(() => s.Game.StartAdventure(() => { }));
    }

    // ---------- StartAttack ----------
    // Combat hits when a d20 roll <= strength and defends when a d20 roll <= agility,
    // so 20 always succeeds and 0 always fails.

    [TestMethod]
    public void StartAttack_NoPlayer_ThrowsInvalidOperation()
    {
        Setup s = CreateGame("2\n");
        s.Game.StartGame();

        Assert.ThrowsExactly<InvalidOperationException>(() => s.Game.StartAttack());
    }

    [TestMethod]
    public void StartAttack_ShowsAttackIntroAndWeaponArt()
    {
        var dragon = new Dragon("Smaug") { DragonHealth = 1, DragonAgility = 0 };
        Setup s = CreateGameWithPlayer(CreateElfThief, dragon);
        s.Game.StartGame();
        s.Game.Player!.Strength = 20;

        s.Game.StartAttack();

        string intro = s.Messages.GetMessage(AttackIntroMessageId)
            .Replace("{occupation}", "Thief")
            .Replace("{weapon}", "Dagger");
        AssertOutputContains(s.Output, intro);
        AssertOutputContains(s.Output, s.Messages.GetMessage(ThiefArtMessageId));
    }

    [TestMethod]
    public void StartAttack_PlayerDefeatsDragon_ShowsVictoryAndEnds()
    {
        var dragon = new Dragon("Smaug") { DragonHealth = 1, DragonAgility = 0 };
        Setup s = CreateGameWithPlayer(CreateElfThief, dragon);
        s.Game.StartGame();
        s.Game.Player!.Strength = 20;

        bool retreated = s.Game.StartAttack();

        Assert.IsFalse(retreated);
        Assert.IsTrue(dragon.DragonHealth <= 0);
        AssertOutputContains(s.Output, s.Messages.GetMessage(DragonDefeatedMessageId));
    }

    [TestMethod]
    public void StartAttack_DragonDefeatsPlayer_ShowsDefeatAndEnds()
    {
        var dragon = new Dragon("Smaug") { DragonStrength = 20 };
        Setup s = CreateGameWithPlayer(CreateElfThief, dragon);
        s.Game.StartGame();
        Player player = s.Game.Player!;
        player.Strength = 0;
        player.Agility = 0;
        player.Health = 1;

        bool retreated = s.Game.StartAttack();

        Assert.IsFalse(retreated);
        Assert.IsTrue(player.Health <= 0);
        AssertOutputContains(s.Output, s.Messages.GetMessage(PlayerDefeatedMessageId));
    }

    [TestMethod]
    public void StartAttack_RetreatAfterRound_ReturnsTrue()
    {
        var dragon = new Dragon("Smaug") { DragonStrength = 0 };
        Setup s = CreateGameWithPlayer(CreateElfThief + "r\n", dragon);
        s.Game.StartGame();
        s.Game.Player!.Strength = 0;

        bool retreated = s.Game.StartAttack();

        Assert.IsTrue(retreated);
        AssertOutputContains(s.Output, s.Messages.GetMessage(AttackPromptMessageId));
        AssertOutputContains(s.Output, s.Messages.GetMessage(AttackRetreatMessageId));
    }
}
