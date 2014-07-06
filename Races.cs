using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prototyping;

namespace Prototyping
{
    public abstract class Race : ICharacter
    {
        private readonly ICharacter _character;
        protected Race(ICharacter character) { _character = character; }

        public Abilities Abilities
        {
            get { return _character.Abilities; }
            set { _character.Abilities = value; }
        }
        public Alignments Alignment { get; set; }
        public Armor Armor { get { return _character.Armor; } set { _character.Armor = value; } }
        public int AttackBonusMod { get; set; }
        public int AttackPerLevelDivisor
        {
            get { return _character.AttackPerLevelDivisor; }
            set { _character.AttackPerLevelDivisor = value; }
        }
        public bool AttacksFlatFootedAc { get; set; }
        public int BaseDamage { get; set; }
        public int BaseHitPoints { get; set; }
        public List<Class> Classes { get { return _character.Classes; } }
        public int CritMultiplier { get; set; }
        public int CurrentDamage { get { return _character.CurrentDamage; } set { _character.CurrentDamage = value; } }
        public int DamageReduction { get { return _character.Armor.GetBonusDamageReduction(); } }
        public int Experience { get; set; }
        public int FlatFootedArmorClass { get; set; }
        public int HitPoints { get { return _character.HitPoints; } }
        public bool IsDead { get; set; }
        public List<Item> Items { get { return _character.Items; } set { _character.Items = value; } } 
        public int Level { get; set; }
        public string Name { get; set; }
        public string RaceName { get; set; }
        public List<Race> Races{ get { return _character.Races; } }
        
        public Weapon Weapon { get { return _character.Weapon; } set { _character.Weapon = value; } }

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
        
        public virtual int GetArmorClass()
        {
            return _character.GetArmorClass();
        }

        public virtual int GetArmorClass(Race enemyRace)
        {
            return _character.GetArmorClass(enemyRace);
        }

        public void RemoveHumanRace(ICharacter character)
        {
            if (character.Races.First().RaceName != "Human") return;
            character.Races.Clear();
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

        public override int GetArmorClass()
        {
            return base.GetArmorClass() + 2;
        }

        public override int GetArmorClass(Race enemyRace)
        {
            return base.GetArmorClass(enemyRace) + 2;
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
            RemoveHumanRace(character);
            character.Races.Add(this);
        }
    }

    public class Elf : Race
    {
        private readonly ICharacter _character;

        public Elf(ICharacter character)
            : base(character)
        {
            _character = character;
            character.Abilities.Dexterity.Score += 1;
            character.Abilities.Constitution.Score -= 1;
            RaceName = "Elf";
            RemoveHumanRace(character);
            character.Races.Add(this);
        }

        public override int GetArmorClass()
        {
            return _character.GetArmorClass();
        }

        public override int GetArmorClass(Race enemyRace)
        {

            if (enemyRace.RaceName == "Orc")
                return _character.GetArmorClass(enemyRace) + 2;
            return _character.GetArmorClass(enemyRace);
        }
    }

    public class Halfling : Race
    {
        private readonly ICharacter _character;
        public Halfling(ICharacter character)
            : base(character)
        {
            _character = character;
            character.Abilities.Dexterity.Score += 1;
            character.Abilities.Strength.Score -= 1;
            RaceName = "Halfling";
            RemoveHumanRace(character);
            character.Races.Add(this);
        }

        public override int GetArmorClass()
        {
            return _character.GetArmorClass();
        }

        public override int GetArmorClass(Race enemyRace)
        {

            if (enemyRace.RaceName != "Halfling")
                return _character.GetArmorClass(enemyRace) + 2;
            return _character.GetArmorClass(enemyRace);
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
