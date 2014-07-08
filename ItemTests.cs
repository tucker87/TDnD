using Microsoft.VisualStudio.TestTools.UnitTesting;
using TDnD;

namespace TDnDTests
{
    [TestClass]
    public class RingOfProtectionTests
    {
        private ICharacter _character;

        [TestInitialize]
        public void Initialize()
        {
            _character = new BaseCharacter();
            _character.Items.Add(new RingOfProtection());
        }

        [TestMethod]
        public void GivesPlusTwoAc()
        {
            Assert.AreEqual(12, _character.GetArmorClass());
        }

        [TestMethod]
        public void GivesPlusTwoAcForEach()
        {
            _character.Items.Add(new RingOfProtection());
            Assert.AreEqual(14, _character.GetArmorClass());
        }
    }

    [TestClass]
    public class BeltOfGiantStrengthTests
    {
        private ICharacter _character;

        [TestInitialize]
        public void Initialize()
        {
            _character = new BaseCharacter();
            _character.Items.Add(new BeltOfGiantStrength(_character));
        }

        [TestMethod]
        public void GivesPlusFourStr()
        {
            Assert.AreEqual(14, _character.Abilities.Strength.Score);
        }

        [Ignore]
        [TestMethod]
        public void TakingTheBeltOffLowersYourStr()
        {
            _character.Items.Clear();
            Assert.AreEqual(10, _character.Abilities.Strength.Score);
        }
    }
}