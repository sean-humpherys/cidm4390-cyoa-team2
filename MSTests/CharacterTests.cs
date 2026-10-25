
using CYOA;


namespace CYOA.Tests;

[TestClass]
public class CharacterTests
{
    [TestMethod]
    public void Constructor_SetsAllCharacterProperties()
    {
        // Arrange & Act
        var character = new Character(
            "Ember",
            "Elf",
            "Fighter",
            5,
            4,
            100,
            "Sword",
            20);

        // Assert
        Assert.AreEqual("Ember", character.Name);
        Assert.AreEqual("Elf", character.Race);
        Assert.AreEqual("Fighter", character.Occupation);
        Assert.AreEqual(5, character.Strength);
        Assert.AreEqual(4, character.Agility);
        Assert.AreEqual(100, character.Health);
        Assert.AreEqual("Sword", character.Weapon);
        Assert.AreEqual(20, character.MaxWeaponDamage);
    }

    [TestMethod]
    [DataRow("1", "Human")]
    [DataRow("2", "Elf")]
    [DataRow("3", "Dwarf")]
    [DataRow("4", "Halfling")]
    public void Create_SetsSelectedRace(
        string raceChoice, string expectedRace)
    {
        Character character = CreateCharacter(
            raceChoice, "Ember", "1");

        Assert.AreEqual(expectedRace, character.Race);
    }

    [TestMethod]
    [DataRow("1", "Fighter", "Sword", 20)]
    [DataRow("2", "Magician", "Staff", 15)]
    [DataRow("3", "Thief", "Dagger", 12)]
    [DataRow("4", "Archer", "Bow", 18)]
    public void Create_SetsCorrectWeaponForOccupation(
        string occupationChoice,
        string expectedOccupation,
        string expectedWeapon,
        int expectedDamage)
    {
        Character character = CreateCharacter(
            "1", "Ember", occupationChoice);

        Assert.AreEqual(expectedOccupation, character.Occupation);
        Assert.AreEqual(expectedWeapon, character.Weapon);
        Assert.AreEqual(expectedDamage, character.MaxWeaponDamage);
    }

    [TestMethod]
    public void Create_SetsDefaultHealthAndValidDiceStats()
    {
        Character character = CreateCharacter(
            "1", "Ember", "1");

        Assert.AreEqual(100, character.Health);

        Assert.IsTrue(
            character.Strength >= 1 && character.Strength <= 6);

        Assert.IsTrue(
            character.Agility >= 1 && character.Agility <= 6);
    }

    [TestMethod]
    public void Create_RejectsEmptyName()
    {
        Character character = CreateCharacter(
            "1", "", "Ember", "1");

        Assert.AreEqual("Ember", character.Name);
    }

    private static Character CreateCharacter(
        params string[] inputs)
    {
        TextReader originalInput = Console.In;
        TextWriter originalOutput = Console.Out;

        try
        {
            Console.SetIn(new StringReader(
                string.Join(Environment.NewLine, inputs)
                + Environment.NewLine));

            Console.SetOut(new StringWriter());

            return Character.Create();
        }
        finally
        {
            Console.SetIn(originalInput);
            Console.SetOut(originalOutput);
        }
    }
}
