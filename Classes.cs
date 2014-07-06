using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prototyping;

namespace Prototyping
{
    public abstract class Class : ICharacter
    {
        private readonly ICharacter _character;
        protected Class(ICharacter character)
        {
            _character = character;
        }

        public Abilities Abilities { get; set; }
        public Alignments Alignment { get; set; }
        public Armor Armor { get { return _character.Armor; } set { _character.Armor = value; } }
        public int AttackBonusMod { get; set; }
        public int AttackPerLevelDivisor { get; set; }
        public bool AttacksFlatFootedAc { get; set; }
        public int BaseDamage { get; set; }
        public int BaseHitPoints { get; set; }
        public List<Class> Classes { get { return _character.Classes; } }
        public int CritMultiplier { get; set; }
        public int CurrentDamage { get; set; }
        public int DamageReduction { get { return _character.Armor.GetBonusDamageReduction(); } }
        public int Experience { get; set; }
        public int FlatFootedArmorClass { get; set; }
        public int HitPoints { get { return _character.HitPoints; } }
        public bool IsDead { get; set; }
        public List<Item> Items { get { return _character.Items; } set { _character.Items = value; } } 
        public int Level { get; set; }
        public string Name { get; set; }
        public List<Race> Races { get; set; }
        public abstract string ClassName { get; }
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
        
        protected void RemovePeasantClass(ICharacter character)
        {
            if (character.Classes.First().ClassName == "Peasant")
            character.Classes.Clear();
        }
    }

    public class Peasant : Class
    {
        public Peasant(ICharacter character) : base(character) {}
        public override string ClassName { get { return "Peasant"; } }
    }

    public class Fighter : Class
    {
        public Fighter(ICharacter character)
            : base(character)
        {
            character.BaseHitPoints = 10;
            character.AttackPerLevelDivisor = 1;
            RemovePeasantClass(character);
            character.Classes.Add(this);
        }
        public override string ClassName { get { return "Fighter"; } }
    }

    public class Rogue : Class
    {
        public Rogue(ICharacter character)
            : base(character)
        {
            character.CritMultiplier = 3;
            character.AttacksFlatFootedAc = true;
            character.AttackBonusMod = character.Abilities.Dexterity.Modifier;
            RemovePeasantClass(character);
            character.Classes.Add(this);
        }
        public override string ClassName { get { return "Rogue"; } }
    }

    public class Monk : Class
    {
        private readonly ICharacter _character;
        public Monk(ICharacter character)
            : base(character)
        {
            _character = character;
            character.BaseHitPoints = 6;
            character.BaseDamage = 3;
            RemovePeasantClass(character);
            character.Classes.Add(this);
        }
        public override string ClassName { get { return "Monk"; } }

        public override int GetArmorClass()
        {
            return base.GetArmorClass() + _character.Abilities.Wisdom.Modifier;
        }

        public override int GetArmorClass(Race enemyRace)
        {
            return base.GetArmorClass() + _character.Abilities.Wisdom.Modifier;
        }
    }

    public class Paladin : Class
    {
        public Paladin(ICharacter character)
            : base(character)
        {
            character.BaseHitPoints = 8;
            RemovePeasantClass(character);
            character.Classes.Add(this);
        }
        public override string ClassName { get { return "Paladin"; } }
    }
}

[TestClass]
public class FighterTests
{
    private ICharacter _character;
    private ICharacter _enemy;

    [TestInitialize]
    public void Initialize()
    {
        _character = new BaseCharacter();
        _character = new Fighter(_character);

        _enemy = new BaseCharacter();
    }

    [TestMethod]
    public void FightersAttackGoesUpEveryLevel()
    {

        for (var i = 0; i < 100; i++)
        {
            _character.Attack(18, _enemy);
        }
        var hit = _character.Attack(10, _enemy);
        Assert.IsTrue(hit);
    }

    [TestMethod]
    public void FightersGetTenHpPerLevel()
    {

        Assert.AreEqual(10, _character.HitPoints);
    }
}

[TestClass]
public class MonkTests
{
    private ICharacter _monk;
    private ICharacter _enemy;

    [TestInitialize]
    public void Initialize()
    {
        _monk = new BaseCharacter();
        _monk = new Monk(_monk);

        _enemy = new BaseCharacter();
    }

    [TestMethod]
    public void MonksGetSixHpPerLevel()
    {
        for (var i = 0; i < 100; i++)
        {
            _monk.Attack(18, _enemy);
        }
        Assert.AreEqual(12, _monk.HitPoints);
    }

    [TestMethod]
    public void MonksDoThreeDamageOnAttack()
    {
        _monk.Attack(18, _enemy);
        Assert.AreEqual(3, _enemy.CurrentDamage);
    }

    [TestMethod]
    public void MonksAddWisdomToArmorClass()
    {
        var abilites = new AbilityScores(wisdom: 12);
        ICharacter wiseCharacter = new BaseCharacter(abilites);
        wiseCharacter = new Monk(wiseCharacter);
        Assert.AreEqual(11, wiseCharacter.GetArmorClass());
    }

    [TestMethod]
    public void MonksDontAddNegativeWisdomToAc()
    {
        var abilites = new AbilityScores(wisdom: 8);
        var wiseCharacter = new BaseCharacter(abilites);
        _monk = new Monk(wiseCharacter);
        Assert.AreEqual(10, wiseCharacter.GetArmorClass());
    }
}

//Paladin is missing tests.
[TestClass]
public class PaladinTests
{
    private ICharacter _character;
    private ICharacter _enemy;

    [TestInitialize]
    public void Initialize()
    {
        _character = new BaseCharacter();
        _character = new Paladin(_character);

        _enemy = new BaseCharacter();
    }

    [TestMethod]
    public void PaladinsGetEightHpPerLevel()
    {
        for (var i = 0; i < 100; i++)
        {
            _character.Attack(18, _enemy);
        }
        Assert.AreEqual(16, _character.HitPoints);
    }

    [TestMethod]
    public void PaladinsGetTwoExtraAttackWhenAttackingEvilCharacters()
    {
        var evilEnemy = new BaseCharacter(alignment: Alignments.Evil);
        var hit = _character.Attack(9, evilEnemy);
        Assert.IsTrue(hit);
    }
}
