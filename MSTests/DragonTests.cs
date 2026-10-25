
using CYOA;

namespace CYOA.Tests;

[TestClass]
public class DragonTests
{
    [TestMethod]
    public void Constructor_SetsDragonName()
    {
        var dragon = new Dragon("Ember");

        Assert.AreEqual("Ember", dragon.DragonName);
    }

    [TestMethod]
    public void Constructor_SetsDefaultStatistics()
    {
        var dragon = new Dragon("Ember");

        Assert.AreEqual(15, dragon.DragonStrength);
        Assert.AreEqual(100, dragon.DragonHealth);
        Assert.AreEqual(10, dragon.DragonAgility);
        Assert.AreEqual("Fire Breath", dragon.DragonWeapon);
    }

    [TestMethod]
    public void DisplayStats_ContainsDragonInformation()
    {
        var dragon = new Dragon("Ember");

        string stats = dragon.DisplayStats();

        StringAssert.Contains(stats, "Ember");
        StringAssert.Contains(stats, "Strength: 15");
        StringAssert.Contains(stats, "Health: 100");
        StringAssert.Contains(stats, "Agility: 10");
        StringAssert.Contains(stats, "Weapon: Fire Breath");
    }
}
