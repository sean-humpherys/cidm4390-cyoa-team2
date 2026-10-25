
using CYOA;

namespace CYOA.Tests;

[TestClass]
public class CharacterStatisticsTests
{
    [TestMethod]
    public void Display_ReturnsCorrectCharacterInformation()
    {
        // Arrange
        string name = "Aragorn";
        string race = "Human";
        string occupation = "Warrior";
        int strength = 20;
        int agility = 15;
        int health = 100;
        string weapon = "Sword";
        int maxWeaponDamage = 25;

        // Act
        string result = CharacterStatistics.Display(
            name,
            race,
            occupation,
            strength,
            agility,
            health,
            weapon,
            maxWeaponDamage);

        // Assert
        StringAssert.Contains(result, "Name: Aragorn");
        StringAssert.Contains(result, "Race: Human");
        StringAssert.Contains(result, "Occupation: Warrior");
        StringAssert.Contains(result, "Strength: 20");
        StringAssert.Contains(result, "Agility: 15");
        StringAssert.Contains(result, "Health: 100");
        StringAssert.Contains(result, "Weapon: Sword");
        StringAssert.Contains(result, "Maximum Weapon Damage: 25");
    }

    [TestMethod]
    public void Display_ContainsCorrectHeaderAndFooter()
    {
        // Arrange & Act
        string result = CharacterStatistics.Display(
            "Aragorn",
            "Human",
            "Warrior",
            20,
            15,
            100,
            "Sword",
            25);

        // Assert
        StringAssert.Contains(result, "===== CHARACTER STATISTICS =====");
        StringAssert.Contains(result, "================================");
    }

    [TestMethod]
    public void Display_ReturnsAllStatisticsInCorrectOrder()
    {
        // Arrange
        string expected =
            "===== CHARACTER STATISTICS =====\n" +
            "Name: Aragorn\n" +
            "Race: Human\n" +
            "Occupation: Warrior\n" +
            "Strength: 20\n" +
            "Agility: 15\n" +
            "Health: 100\n" +
            "Weapon: Sword\n" +
            "Maximum Weapon Damage: 25\n" +
            "================================";

        // Act
        string result = CharacterStatistics.Display(
            "Aragorn",
            "Human",
            "Warrior",
            20,
            15,
            100,
            "Sword",
            25);

        // Assert
        Assert.AreEqual(expected, result);
    }
}
