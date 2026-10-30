
using CYOA;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CYOA.Tests;

[TestClass]
public class DragonTests
{
    [TestMethod]
    public void Constructor_SetsDragonName()
    {
        // Arrange
        string name = "Ember";

        // Act
        var dragon = new Dragon(name);

        // Assert
        Assert.AreEqual("Ember", dragon.DragonName);
    }

    [TestMethod]
    public void Constructor_SetsDefaultStatistics()
    {
        // Arrange and Act
        var dragon = new Dragon("Ember");

        // Assert
        Assert.AreEqual(15, dragon.DragonStrength);
        Assert.AreEqual(100, dragon.DragonHealth);
        Assert.AreEqual(10, dragon.DragonAgility);
    }

    [TestMethod]
    public void Constructor_SetsDefaultWeapon()
    {
        // Arrange and Act
        var dragon = new Dragon("Ember");

        // Assert
        Assert.AreEqual("Fire Breath", dragon.Weapon.type);
        Assert.AreEqual(25, dragon.Weapon.maxDamage);
        Assert.AreEqual(
            "  (  )\n (    )\n  (  )",
            dragon.Weapon.asciiArt);
    }

    [TestMethod]
    public void MissedMeTaunt_ReturnsCorrectMessage()
    {
        // Arrange
        var dragon = new Dragon("Ember");

        // Act
        string taunt = dragon.MissedMeTaunt();

        // Assert
        Assert.AreEqual(
            "The dragon laughs as your attack misses!",
            taunt);
    }

    [TestMethod]
    public void WasHitTaunt_ReturnsCorrectMessage()
    {
        // Arrange
        var dragon = new Dragon("Ember");

        // Act
        string taunt = dragon.WasHitTaunt();

        // Assert
        Assert.AreEqual(
            "The dragon roars in anger after being hit!",
            taunt);
    }

    [TestMethod]
    public void DisplayStats_ContainsDragonInformation()
    {
        // Arrange
        var dragon = new Dragon("Ember");

        // Act
        string stats = dragon.DisplayStats();

        // Assert
        StringAssert.Contains(stats, "Dragon Statistics");
        StringAssert.Contains(stats, "Name: Ember");
        StringAssert.Contains(stats, "Strength: 15");
        StringAssert.Contains(stats, "Health: 100");
        StringAssert.Contains(stats, "Agility: 10");
        StringAssert.Contains(stats, "Weapon: Fire Breath");
        StringAssert.Contains(stats, "Weapon Damage: 25");
        StringAssert.Contains(stats, dragon.Weapon.asciiArt);
    }

    [TestMethod]
    public void DisplayStats_UsesFallbackLabelsWhenMessagesAreNull()
    {
        // Arrange
        var dragon = new Dragon("Ember");

        // Act
        string stats = dragon.DisplayStats(null);

        // Assert
        StringAssert.Contains(stats, "Name: Ember");
        StringAssert.Contains(stats, "Strength: 15");
        StringAssert.Contains(stats, "Health: 100");
        StringAssert.Contains(stats, "Agility: 10");
        StringAssert.Contains(stats, "Weapon: Fire Breath");
        StringAssert.Contains(stats, "Weapon Damage: 25");
    }
}
