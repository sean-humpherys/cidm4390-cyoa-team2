
using CYOA;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace CYOA.Tests;

[TestClass]
public class PlayerTests
{
    [TestMethod]
    public void Constructor_SetsAllPlayerProperties()
    {
        // Arrange
        var weapon = new Weapon("Sword", 20, "Sword art");

        // Act
        var player = new Player(
            "Ember",
            Race.Elf,
            Occupation.Fighter,
            5,
            4,
            100,
            weapon);

        // Assert
        Assert.AreEqual("Ember", player.Name);
        Assert.AreEqual(Race.Elf, player.Race);
        Assert.AreEqual(Occupation.Fighter, player.Occupation);
        Assert.AreEqual(5, player.Strength);
        Assert.AreEqual(4, player.Agility);
        Assert.AreEqual(100, player.Health);
        Assert.AreEqual("Sword", player.Weapon.type);
        Assert.AreEqual(20, player.Weapon.maxDamage);
        Assert.AreEqual("Sword art", player.Weapon.asciiArt);
    }

    [TestMethod]
    [DataRow("1", Race.Human)]
    [DataRow("2", Race.Elf)]
    [DataRow("3", Race.Dwarf)]
    [DataRow("4", Race.Halfling)]
    public void Create_SetsSelectedRace(
        string raceChoice,
        Race expectedRace)
    {
        Player player = CreatePlayer(
            raceChoice, "Ember", "1", "roll", "roll", "roll", "1");

        Assert.AreEqual(expectedRace, player.Race);
    }

    [TestMethod]
    [DataRow("1", Occupation.Fighter, "Sword", 20)]
    [DataRow("2", Occupation.Magician, "Staff", 15)]
    [DataRow("3", Occupation.Thief, "Dagger", 12)]
    [DataRow("4", Occupation.Archer, "Bow", 18)]
    public void Create_SetsCorrectWeaponForOccupation(
        string occupationChoice,
        Occupation expectedOccupation,
        string expectedWeapon,
        int expectedDamage)
    {
        Player player = CreatePlayer(
            "1", "Ember", occupationChoice, "roll", "roll", "roll", "1");

        Assert.AreEqual(expectedOccupation, player.Occupation);
        Assert.AreEqual(expectedWeapon, player.Weapon.type);
        Assert.AreEqual(expectedDamage, player.Weapon.maxDamage);
    }

    [TestMethod]
    public void Create_SetsDefaultHealthAndValidDiceStats()
    {
        Player player = CreatePlayer(
            "1", "Ember", "1", "roll", "roll", "roll", "1");

        Assert.AreEqual(100, player.Health);

        Assert.IsTrue(
            player.Strength >= 1 && player.Strength <= 6);

        Assert.IsTrue(
            player.Agility >= 1 && player.Agility <= 6);
    }

    [TestMethod]
    public void Create_RejectsEmptyName()
    {
        Player player = CreatePlayer(
            "1", "", "Ember", "1", "roll", "roll", "roll", "1");

        Assert.AreEqual("Ember", player.Name);
    }

    [TestMethod]
    public void DisplayPlayerStatistics_ContainsPlayerInformation()
    {
        // Arrange
        var weapon = new Weapon("Sword", 20, "Sword art");
        var player = new Player(
            "Ember",
            Race.Elf,
            Occupation.Fighter,
            5,
            4,
            100,
            weapon);

        // Act
        string statistics = player.DisplayPlayertatistics();

        // Assert
        StringAssert.Contains(statistics, "Ember");
        StringAssert.Contains(statistics, "Elf");
        StringAssert.Contains(statistics, "Fighter");
        StringAssert.Contains(statistics, "Strength: 5");
        StringAssert.Contains(statistics, "Agility: 4");
        StringAssert.Contains(statistics, "Health points: 100");
        StringAssert.Contains(statistics, "Sword");
        StringAssert.Contains(statistics, "20");
    }

    private static Player CreatePlayer(params string[] inputs)
    {
        TextReader originalInput = Console.In;
        TextWriter originalOutput = Console.Out;

        try
        {
            Console.SetIn(new StringReader(
                string.Join(Environment.NewLine, inputs)
                + Environment.NewLine));

            Console.SetOut(new StringWriter());

            return Player.Create();
        }
        finally
        {
            Console.SetIn(originalInput);
            Console.SetOut(originalOutput);
        }
    }
}
