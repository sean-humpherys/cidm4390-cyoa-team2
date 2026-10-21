
using CYOA;

namespace CYOA.Tests;

[TestClass]
public class DragonTests
{
    [TestMethod]
    public void Constructor_SetsDragonName()
    {
        var dragon = new Dragon("Ember");

        Assert.AreEqual("Ember", dragon.Name);
    }

    [TestMethod]
    public void Constructor_SetsDefaultStatistics()
    {
        var dragon = new Dragon("Ember");

        Assert.AreEqual(15, dragon.Strength);
        Assert.AreEqual(100, dragon.Health);
        Assert.AreEqual(10, dragon.Agility);
        Assert.AreEqual("Fire Breath", dragon.Weapon);
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
