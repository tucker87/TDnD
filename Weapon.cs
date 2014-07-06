using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prototyping;

namespace Prototyping
{
    public abstract class Weapon
    {
        public virtual int GetBonusDamage(ICharacter enemy) { return 0; }
        public virtual int CriticalMultiplier { get { return 0; } }
        public virtual int GetBonusAttack(ICharacter enemy) { return 0; }
    }

    public class Fists : Weapon { }

    public class LongSword : Weapon
    {
        public override int GetBonusDamage(ICharacter enemy) { return 5; }
    }

    public class WarAxe : Weapon
    {
        private readonly ICharacter _character;
        public WarAxe(ICharacter character)
        {
            _character = character;
        }
        public override int GetBonusDamage(ICharacter enemy) { return 6; }
        public override int CriticalMultiplier
        {
            get
            {
                return _character.Classes.First().ClassName == "Rogue" ? 5 : 3;
            }
        }
    }

    public class NunChucks : Weapon
    {
        private ICharacter _character;

        public NunChucks(ICharacter character)
        {
            _character = character;
        }

        public override int GetBonusAttack(ICharacter enemy)
        {
            if (_character.Classes.First().ClassName != "Monk")
                return -4;
            return 0;
        }

        public override int GetBonusDamage(ICharacter enemy)
        {
            return 6;
        }
    }

    public class ElvenWeapon : Weapon
    {
        private readonly int _bonus;
        private readonly Weapon _weapon;
        private readonly ICharacter _character;
        public ElvenWeapon(int bonus, Weapon weapon, ICharacter character)
        {
            _bonus = bonus;
            _weapon = weapon;
            _character = character;
        }

        public override int GetBonusDamage(ICharacter enemy)
        {
            var racialBonus = 0;
            var characterRace = _character.Races.First().RaceName;
            var enemyRace = enemy.Races.First().RaceName;

            if (characterRace == "Elf" || enemyRace == "Orc")
                racialBonus = 1;

            if (characterRace == "Elf" && enemyRace == "Orc")
                racialBonus = 4;

            return _weapon.GetBonusDamage(enemy) + _bonus + racialBonus;
        }

        public override int GetBonusAttack(ICharacter enemy)
        {
            var characterRace = _character.Races.First().RaceName;
            var enemyRace = enemy.Races.First().RaceName;

            if (characterRace == "Elf" && enemyRace == "Orc")
                return _weapon.GetBonusAttack(enemy) + _bonus + 4;

            if (characterRace == "Elf" || enemyRace == "Orc")
                return _weapon.GetBonusAttack(enemy) + _bonus + 1;
            
            return _weapon.GetBonusAttack(enemy) + _bonus;
        }
    }


    public class MagicWeapon : Weapon
    {
        private readonly int _bonus;
        private readonly Weapon _weapon;
        public MagicWeapon(int bonus, Weapon weapon)
        {
            _bonus = bonus;
            _weapon = weapon;
        }
        public override int GetBonusAttack(ICharacter enemy) { return _weapon.GetBonusAttack(enemy) + _bonus; }
        public override int GetBonusDamage(ICharacter enemy) { return _weapon.GetBonusDamage(enemy) + _bonus; }
        public override int CriticalMultiplier { get { return _weapon.CriticalMultiplier; } }
    }

}

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

