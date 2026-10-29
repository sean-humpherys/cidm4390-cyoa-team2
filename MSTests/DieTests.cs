using CYOA;

namespace CYOA.Tests;

[TestClass]
public class DieTests
{
    // Rolls are random, so each test rolls many times to cover the possible results.
    private const int RollCount = 1000;

    [TestMethod]
    [DataRow(4)]
    [DataRow(6)]
    [DataRow(12)]
    [DataRow(20)]
    public void Roll_StaysBetweenOneAndSides(int sides)
    {
        for (int i = 0; i < RollCount; i++)
        {
            int roll = Die.Roll(sides);

            Assert.IsTrue(roll >= 1 && roll <= sides, $"Rolled {roll} on a d{sides}.");
        }
    }

    [TestMethod]
    public void Roll_OneSide_AlwaysReturnsOne()
    {
        for (int i = 0; i < RollCount; i++)
        {
            Assert.AreEqual(1, Die.Roll(1));
        }
    }

    [TestMethod]
    public void Roll_SixSides_CanReturnEveryFace()
    {
        var seen = new HashSet<int>();

        for (int i = 0; i < RollCount; i++)
        {
            seen.Add(Die.Roll(6));
        }

        CollectionAssert.AreEquivalent(new[] { 1, 2, 3, 4, 5, 6 }, seen.ToArray());
    }

    [TestMethod]
    public void Roll_NegativeSides_ThrowsArgumentOutOfRange()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Die.Roll(-1));
    }
}
