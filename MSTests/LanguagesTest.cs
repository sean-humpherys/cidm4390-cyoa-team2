
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
