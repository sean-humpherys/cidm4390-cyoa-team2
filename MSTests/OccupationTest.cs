
using CYOA;
using System;

namespace CYOA.Tests;

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
