using CYOA;
namespace CYOA.Tests
{
    [TestClass]
    public class CombatTests
    {
        private Combat combat = null!;
        [TestInitialize]
        public void Setup()
        {
            combat = new Combat();
        }
        [TestMethod]
        public void CheckHit_ZeroStrength_ReturnsFalse()
        {
            bool result = combat.CheckHit(0);
            Assert.IsFalse(result);
        }
        [TestMethod]
        public void CheckHit_TwentyStrength_ReturnsTrue()
        {
            bool result = combat.CheckHit(20);
            Assert.IsTrue(result);
        }
        [TestMethod]
        public void CheckDefense_ZeroAgility_ReturnsFalse()
        {
            bool result = combat.CheckDefense(0);
            Assert.IsFalse(result);
        }
        [TestMethod]
        public void CheckDefense_TwentyAgility_ReturnsTrue()
        {
            bool result = combat.CheckDefense(20);
            Assert.IsTrue(result);
        }
        [TestMethod]
        public void CalculateDamage_ReturnsValidDamage()
        {
            int damage = combat.CalculateDamage(10);
            Assert.IsTrue(damage >= 1 && damage <= 10);
        }
        [TestMethod]
        public void CalculateDamage_InvalidValue_ThrowsException()
        {
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => combat.CalculateDamage(0));
        }
    }
}