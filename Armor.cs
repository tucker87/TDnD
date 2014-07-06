using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prototyping;

namespace Prototyping
{
    public abstract class Armor
    {
        public virtual int GetBonusArmorClass()
        {
            return 0;
        }

        public virtual int GetBonusAttack()
        {
            return 0;
        }

        public virtual int GetBonusDamageReduction()
        {
            return 0;
        }
    }

    public class ClothArmor : Armor
    {
        public override int GetBonusArmorClass()
        {
            return 0;
        }
    }

    public class LeatherArmor : Armor
    {
        public override int GetBonusArmorClass()
        {
            return 2;
        }
    }

    public class ChainMail : Armor
    {
        public override int GetBonusArmorClass()
        {
            return 5;
        }
    }

    public class PlateArmor : Armor
    {
        private ICharacter _character;

        public PlateArmor(ICharacter character)
        {
            if (character.Races.First().RaceName != "Dwarf"
                && character.Classes.First().ClassName != "Fighter")
            {
                throw new ArgumentException("Only Fighters and Dwarves can equip Plate");
            }
        }

        public override int GetBonusArmorClass()
        {
            return 8;
        }
    }

    public class DamageReduction : Armor
    {
        private readonly int _amount;
        private readonly Armor _baseArmor;

        public DamageReduction(int amount, Armor baseArmor)
        {
            _amount = amount;
            _baseArmor = baseArmor;
        }

        public override int GetBonusArmorClass()
        {
            return _baseArmor.GetBonusArmorClass();
        }

        public override int GetBonusDamageReduction()
        {
            return _amount;
        }
    }

    public class ElvenArmor : Armor
    {
        private readonly Armor _baseArmor;
        private readonly ICharacter _character;

        public ElvenArmor(Armor baseArmor, ICharacter character)
        {
            _baseArmor = baseArmor;
            _character = character;
        }
        public override int GetBonusArmorClass()
        {
            var bonus = 0;
            if (_character.Races.First().RaceName == "Elf")
                bonus = 3;
            return _baseArmor.GetBonusArmorClass() + bonus;
        }

        public override int GetBonusAttack()
        {
            return _character.Races.First().RaceName == "Elf" ? 1 : 0;
        }
    }
}

[TestClass]
public class LeatherArmorTests
{
    private ICharacter _character;

    [TestInitialize]
    public void Initialize()
    {
        _character = new BaseCharacter();
    }

    [TestMethod]
    public void GivesPlusTwoArmorClass()
    {
        _character.Armor = new LeatherArmor();
        Assert.AreEqual(12, _character.GetArmorClass());
    }
}

[TestClass]
public class PlateArmorTests
{
    private ICharacter _character;

    [TestInitialize]
    public void Initialize()
    {
        _character = new BaseCharacter();
    }

    [TestMethod]
    public void GivesEightBonusArmor()
    {
        _character = new Dwarf(_character) { Armor = new PlateArmor(_character) };
        Assert.AreEqual(18, _character.GetArmorClass());
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException),
        "Only Fighters and Dwarves can equip Plate")]
    public void CannotBeWornByMany()
    {
        _character.Armor = new PlateArmor(_character);
    }

    [TestMethod]
    public void CanBeWornByFighters()
    {
        _character = new Fighter(_character) { Armor = new PlateArmor(_character) };
        Assert.AreEqual(18, _character.GetArmorClass());
    }
}

[TestClass]
public class DamageReductionLeatherArmorTests
{
    private ICharacter _character;
    private ICharacter _strongEnemy;

    [TestInitialize]
    public void Initialize()
    {
        _character = new Dwarf(new BaseCharacter()) { Armor = new DamageReduction(2, new LeatherArmor()) };
        var abilities = new AbilityScores(20);
        _strongEnemy = new BaseCharacter(abilities);
    }

    [TestMethod]
    public void StillGivesTwoAc()
    {

        Assert.AreEqual(12, _character.GetArmorClass());
    }

    [TestMethod]
    public void GivesTwoDamageReduction()
    {
        _strongEnemy.Attack(18, _character);
        Assert.AreEqual(4, _character.CurrentDamage);
    }
}

[TestClass]
public class ChainMailTests
{
    private ICharacter _character;

    [TestInitialize]
    public void Initialize()
    {
        _character = new BaseCharacter();
    }

    [TestMethod]
    public void GivesFiveToAc()
    {
        _character.Armor = new ChainMail();
        Assert.AreEqual(15, _character.GetArmorClass());
    }
}

[TestClass]
public class ElvenChainMailTests
{
    private ICharacter _character;
    private ICharacter _enemy;

    [TestInitialize]
    public void Initialize()
    {
        _character = new BaseCharacter();
        _character.Armor = new ElvenArmor(new ChainMail(), _character);
        _enemy = new BaseCharacter();
    }

    [TestMethod]
    public void NoExtraBonusToNonElves()
    {
        Assert.AreEqual(15, _character.GetArmorClass());
    }

    [TestMethod]
    public void ThreeExtraBonusToElves()
    {
        _character = new Elf(_character);
        Assert.AreEqual(18, _character.GetArmorClass());
    }

    [TestMethod]
    public void NoBonusAttackToNonElves()
    {
        var hit = _character.Attack(10, _enemy);
        Assert.IsFalse(hit);
    }

    [TestMethod]
    public void OneBonusAttackToElves()
    {
        _character = new Elf(_character);
        var hit = _character.Attack(10, _enemy);
        Assert.IsTrue(hit);
    }
}