
using System;
using CYOA;

namespace CYOA.Tests;

[TestClass]
public class LanguageTests
{
    [TestMethod]
    public void Language_ContainsExpectedValues()
    {
        Assert.AreEqual(3, Enum.GetValues<Language>().Length);

        Assert.IsTrue(Enum.IsDefined(Language.English));
        Assert.IsTrue(Enum.IsDefined(Language.Spanish));
        Assert.IsTrue(Enum.IsDefined(Language.French));
    }
}

[TestClass]
public class OccupationTests
{
    [TestMethod]
    public void Occupation_ContainsExpectedValues()
    {
        Assert.AreEqual(4, Enum.GetValues<Occupation>().Length);

        Assert.IsTrue(Enum.IsDefined(Occupation.Fighter));
        Assert.IsTrue(Enum.IsDefined(Occupation.Magician));
        Assert.IsTrue(Enum.IsDefined(Occupation.Thief));
        Assert.IsTrue(Enum.IsDefined(Occupation.Archer));
    }
}
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
