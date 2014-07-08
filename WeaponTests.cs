using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TDnD;

namespace TDnDTests
{
    [TestClass]
    public class LongSwordTests
    {
        private ICharacter _character;
        private ICharacter _enemy;

        [TestInitialize]
        public void Initialize()
        {
            _character = new BaseCharacter();
            _enemy = new BaseCharacter();
        }

        [TestMethod]
        public void LongSwordsGive5BonusDamage()
        {
            _character.Weapon = new LongSword();
            _character.Attack(18, _enemy);
            Assert.AreEqual(6, _enemy.CurrentDamage);
        }
    }

    [TestClass]
    public class WarAxeTests
    {
        private ICharacter _character;
        private ICharacter _enemy;

        [TestInitialize]
        public void Initialize()
        {
            _character = new BaseCharacter();
            _character.Weapon = new WarAxe(_character.Classes.First());
            _enemy = new BaseCharacter();
        }

        [TestMethod]
        public void DoesSixBonusDamage()
        {
            _character.Attack(18, _enemy);
            Assert.AreEqual(7, _enemy.CurrentDamage);
        }

        [TestMethod]
        public void DoesTripleDamageOnCrit()
        {
            _character.Attack(20, _enemy);
            Assert.AreEqual(21, _enemy.CurrentDamage);
        }

        [TestMethod]
        public void DoesFiveTimesDamageOnCritAsRogue()
        {
            _character = new Rogue(_character);
            _character.Attack(20, _enemy);
            Assert.AreEqual(35, _enemy.CurrentDamage);
        }
    }

    [TestClass]
    public class PlusTwoWarAxeTests
    {
        private ICharacter _character;
        private ICharacter _enemy;

        [TestInitialize]
        public void Initialize()
        {
            _character = new BaseCharacter();
            _character.Weapon = new MagicWeapon(2, new WarAxe(_character.Classes.First()));
            _enemy = new BaseCharacter();
        }

        [TestMethod]
        public void DoesEightBonusDamage()
        {
            _character.Attack(18, _enemy);
            Assert.AreEqual(9, _enemy.CurrentDamage);
        }

        [TestMethod]
        public void DoesTripleDamageOnCrit()
        {
            _character.Attack(20, _enemy);
            Assert.AreEqual(27, _enemy.CurrentDamage);
        }

        [TestMethod]
        public void DoesFiveTimesDamageOnCritAsRogue()
        {
            _character = new Rogue(_character);
            _character.Attack(20, _enemy);
            Assert.AreEqual(45, _enemy.CurrentDamage);
        }

        [TestMethod]
        public void GivesPlusTwoAttack()
        {
            var hit = _character.Attack(9, _enemy);
            Assert.IsTrue(hit);
        }
    }

    [TestClass]
    public class ElvenLongSwordTests
    {
        private ICharacter _character;
        private ICharacter _enemy;

        [TestInitialize]
        public void Initialize()
        {
            _character = new BaseCharacter();
            _character.Weapon = new ElvenWeapon(1, new LongSword(), _character.Races.First());
            _enemy = new BaseCharacter();
        }

        [TestMethod]
        public void DoesSixBonusDamage()
        {
            _character.Attack(18, _enemy);
            Assert.AreEqual(7, _enemy.CurrentDamage);
        }

        [TestMethod]
        public void GiveOneBonusAttack()
        {
            var hit = _character.Attack(10, _enemy);
            Assert.IsTrue(hit);
        }

        [TestMethod]
        public void GivesTwoBonusAttackWhenHeldByElf()
        {
            _character = new Elf(_character);
            var hit = _character.Attack(9, _enemy);
            Assert.IsTrue(hit);
        }

        [TestMethod]
        public void GivesTwoBonusAttackWhenUsedAgainstOrc()
        {
            _enemy = new Orc(_enemy);
            var hit = _character.Attack(11, _enemy);
            Assert.IsTrue(hit);
        }

        [TestMethod]
        public void GivesFiveBonusAttackWhenHeldByElfAgainstOrc()
        {
            _character = new Elf(_character);
            _enemy = new Orc(_enemy);
            var hit = _character.Attack(8, _enemy);
            Assert.IsTrue(hit);
        }

        [TestMethod]
        public void GivesTwoBonusDamageWhenHeldByElf()
        {
            _character = new Elf(_character);
            _character.Attack(9, _enemy);
            Assert.AreEqual(8, _enemy.CurrentDamage);
        }

        [TestMethod]
        public void GivesTwoBonusDamageWhenUsedOnOrc()
        {
            _enemy = new Orc(_enemy);
            _character.Attack(11, _enemy);
            Assert.AreEqual(8, _enemy.CurrentDamage);
        }

        [TestMethod]
        public void GivesFiveBonusDamageWhenUsedByElfOnOrc()
        {
            _character = new Elf(_character);
            _enemy = new Orc(_enemy);
            _character.Attack(11, _enemy);
            Assert.AreEqual(11, _enemy.CurrentDamage);
        }
    }

    [TestClass]
    public class NunChuckTests
    {
        private ICharacter _character;
        private ICharacter _enemy;

        [TestInitialize]
        public void Initialize()
        {
            _character = new BaseCharacter();
            _character.Weapon = new NunChucks(_character);
            _enemy = new BaseCharacter();
        }

        [TestMethod]
        public void DoesSixBonusDamage()
        {
            _character = new Monk(_character);
            _character.Attack(14, _enemy);
            Assert.AreEqual(9, _enemy.CurrentDamage);
        }

        [TestMethod]
        public void NonMonksTakeFourPenaltyToAttack()
        {
            var hit = _character.Attack(14, _enemy);
            Assert.IsFalse(hit);
        }
    }
}