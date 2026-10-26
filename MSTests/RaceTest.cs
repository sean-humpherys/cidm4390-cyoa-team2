
using CYOA;

namespace CYOA.Tests;

[TestClass]
public class RaceTests
{
    [TestMethod]
    public void Race_ContainsExpectedValues()
    {
        Assert.AreEqual(4, Enum.GetValues<Race>().Length);

        Assert.IsTrue(Enum.IsDefined(Race.Human));
        Assert.IsTrue(Enum.IsDefined(Race.Elf));
        Assert.IsTrue(Enum.IsDefined(Race.Dwarf));
        Assert.IsTrue(Enum.IsDefined(Race.Halfling));
    }
}
