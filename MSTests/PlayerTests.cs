
using CYOA;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
        string statistics = player.DisplayPlayerStatistics();

        // Assert
        StringAssert.Contains(statistics, "PLAYER STATISTICS");
        StringAssert.Contains(statistics, "Name: Ember");
        StringAssert.Contains(statistics, "Race: Elf");
        StringAssert.Contains(statistics, "Occupation: Fighter");
        StringAssert.Contains(statistics, "Strength: 5");
        StringAssert.Contains(statistics, "Agility: 4");
        StringAssert.Contains(statistics, "Health points: 100");
        StringAssert.Contains(statistics, "Weapon: Sword");
        StringAssert.Contains(statistics, "Maximum Weapon Damage: 20");
        StringAssert.Contains(statistics, "Sword art");
    }

    [TestMethod]
    public void DisplayPlayerStatistics_UsesFallbackLabelsWhenMessagesAreNull()
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
        string statistics = player.DisplayPlayerStatistics(null);

        // Assert
        StringAssert.Contains(statistics, "Name: Ember");
        StringAssert.Contains(statistics, "Race: Elf");
        StringAssert.Contains(statistics, "Occupation: Fighter");
        StringAssert.Contains(statistics, "Strength: 5");
        StringAssert.Contains(statistics, "Agility: 4");
        StringAssert.Contains(statistics, "Health points: 100");
        StringAssert.Contains(statistics, "Weapon: Sword");
        StringAssert.Contains(statistics, "Maximum Weapon Damage: 20");
    }
}
