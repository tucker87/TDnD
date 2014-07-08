using Microsoft.VisualStudio.TestTools.UnitTesting;
using TDnD;

namespace TDnDTests
{
    [TestClass]
    public class OrcTests
    {
        private ICharacter _character;

        [TestInitialize]
        public void Initialize()
        {
            _character = new BaseCharacter();
            _character = new Orc(_character);
        }

        [TestMethod]
        public void OrcsGiveBonusTwoStrength()
        {
            Assert.AreEqual(12, _character.Abilities.Strength.Score);
        }

        [TestMethod]
        public void OrcsGetNegativeOneToInt()
        {
            Assert.AreEqual(9, _character.Abilities.Intelligence.Score);
        }

        [TestMethod]
        public void OrcsGetNegativeOneCha()
        {
            Assert.AreEqual(9, _character.Abilities.Charisma.Score);
        }

        [TestMethod]
        public void OrcsGetNegativeOneWis()
        {
            Assert.AreEqual(9, _character.Abilities.Wisdom.Score);
        }

        [TestMethod]
        public void OrcsGetTwoBonusAc()
        {
            Assert.AreEqual(12, _character.GetArmorClass());
        }
    }

    [TestClass]
    public class DwarfTests
    {
        private ICharacter _character;

        [TestInitialize]
        public void Initialize()
        {
            _character = new Dwarf(new BaseCharacter());
        }

        [TestMethod]
        public void DwarvesGetOneBonusCon()
        {
            Assert.AreEqual(11, _character.Abilities.Constitution.Score);
        }

        [TestMethod]
        public void DwarvesLoseOneCha()
        {
            Assert.AreEqual(9, _character.Abilities.Charisma.Score);
        }

        [TestMethod]
        public void DwarvesGetDoubleConBonusToHp()
        {
            var abilities = new AbilityScores(constitution: 14);
            ICharacter character = new BaseCharacter(abilities);
            character = new Dwarf(character);
            Assert.AreEqual(9, character.HitPoints);
        }

        [TestMethod]
        public void DwarvesGetTwoAttackWhenTargetIsOrc()
        {
            ICharacter enemy = new Orc(new BaseCharacter());
            var attack = new Attack(11, _character, enemy);
            Assert.IsTrue(attack.IsHit);
        }
    }

    [TestClass]
    public class ElfTests
    {
        private ICharacter _character;

        [TestInitialize]
        public void Initialize()
        {
            _character = new BaseCharacter();
            _character = new Elf(_character);
        }

        [TestMethod]
        public void ElvesGetOneBousDex()
        {
            Assert.AreEqual(11, _character.Abilities.Dexterity.Score);
        }

        [TestMethod]
        public void ElvesLoseOneConstitution()
        {
            Assert.AreEqual(9, _character.Abilities.Constitution.Score);
        }

        [TestMethod]
        public void ElvesGainTwoAcWhenAttackedByOrcs()
        {
            var orcEnemy = new Orc(new BaseCharacter());

            var attack = new Attack(12, orcEnemy, _character);

            Assert.IsFalse(attack.IsHit);
        }
    }

    [TestClass]
    public class HalflingTests
    {
        private ICharacter _character;

        [TestInitialize]
        public void Initialize()
        {
            _character = new Halfling(new BaseCharacter());
        }

        [TestMethod]
        public void HalflingsGetOneBousDex()
        {
            Assert.AreEqual(11, _character.Abilities.Dexterity.Score);
        }

        [TestMethod]
        public void HalflingsLoseOneStr()
        {
            Assert.AreEqual(9, _character.Abilities.Strength.Score);
        }

        [TestMethod]
        public void HalflingsGainsTwoAcWhenAttackedByNonHalfling()
        {
            var orc = new Orc(new BaseCharacter());
            var elf = new Elf(new BaseCharacter());
            var dwarf = new Dwarf(new BaseCharacter());
            var halfling = new Halfling(new BaseCharacter());

            var thePoorTarget = new Halfling(new BaseCharacter());

            var attack = orc.Attack(12, thePoorTarget);
            Assert.IsFalse(attack);
            attack = elf.Attack(12, thePoorTarget);
            Assert.IsFalse(attack);
            attack = dwarf.Attack(12, thePoorTarget);
            Assert.IsFalse(attack);
            attack = halfling.Attack(12, thePoorTarget);
            Assert.IsTrue(attack);
        }
    }
}