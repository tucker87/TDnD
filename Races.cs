using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prototyping;

namespace Prototyping
{
    public abstract class Race : ICharacter
    {
        private readonly ICharacter _character;

        protected Race(ICharacter character)
        {
            _character = character;
        }

        public string RaceName { get; set; }

        public virtual int ArmorClass { get { return _character.ArmorClass; } }
        public int FlatFootedArmorClass { get; set; }
        public int Level { get; set; }
        public string Name { get; set; }
        public List<Class> Classes { get { return _character.Classes; } }
        public List<Race> Races { get { return _character.Races; } }
        public bool IsDead { get; set; }
        public int BaseDamage { get; set; }
        public int AttackBonusMod { get; set; }
        public int CritMultiplier { get; set; }
        public bool AttacksFlatFootedAc { get; set; }

        public int AttackPerLevelDivisor
        {
            get { return _character.AttackPerLevelDivisor; }
            set { _character.AttackPerLevelDivisor = value; }
        }
        public int BaseHitPoints { get; set; }
        public Abilities Abilities
        {
            get { return _character.Abilities; }
            set { _character.Abilities = value; }
        }

        public Alignments Alignment { get; set; }
        public int CurrentDamage { get; set; }
        public int Experience { get; set; }
        public int HitPoints { get { return _character.HitPoints; } }

        public bool Attack(int roll, ICharacter target)
        {
            return _character.Attack(roll, target);
        }
        public void GainExperience(int xp)
        {
            _character.GainExperience(xp);
        }

        public void TakeDamage(int damage)
        {
            _character.TakeDamage(damage);
        }

    }

    public class Human : Race
    {
        public Human(ICharacter character)
            : base(character)
        {
            RaceName = "Human";
        }

    }

    public class Orc : Race
    {
        private readonly ICharacter _character;
        public Orc(ICharacter character)
            : base(character)
        {
            _character = character;
            character.Abilities.Strength.Score += 2;
            character.Abilities.Intelligence.Score += -1;
            character.Abilities.Wisdom.Score += -1;
            character.Abilities.Charisma.Score += -1;
            RaceName = "Orc";
            character.Races.Clear();
            character.Races.Add(this);
        }
        public override int ArmorClass
        {
            get { return _character.ArmorClass + 2; }
        }
    }

    public class Dwarf : Race
    {
        public Dwarf(ICharacter character)
            : base(character)
        {
            character.Abilities.Constitution.Score += 1;
            character.Abilities.Charisma.Score += -1;
            RaceName = "Dwarf";
            character.Races.Clear();
            character.Races.Add(this);
        }
    }
}

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
        Assert.AreEqual(12, _character.ArmorClass);
    }
}

[TestClass]
public class DwarfTests
{
    private ICharacter _character;

    [TestInitialize]
    public void Initialize()
    {
        _character = new BaseCharacter();
        _character = new Dwarf(_character);
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
        ICharacter enemy = new BaseCharacter();
        enemy = new Orc(enemy);
        const int justEnoughToMissByTwoTakingOrcAcBonusIntoAccount = 11;
        var attack = new Attack(justEnoughToMissByTwoTakingOrcAcBonusIntoAccount, _character, enemy);
        Assert.IsTrue(attack.IsHit);
    }
}
