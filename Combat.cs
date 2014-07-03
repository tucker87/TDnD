using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prototyping;

namespace Prototyping
{
    public class Attack
    {
        public Attack(int roll, ICharacter attacker, ICharacter target)
        {
            var attackTotal = CalculateAttack(roll, attacker, target);

            var targetAc = attacker.AttacksFlatFootedAc ? target.FlatFootedArmorClass : target.ArmorClass;

            if (!CheckIfHit(attackTotal, targetAc)) return;
            IsHit = true;
            target.TakeDamage(CalculateDamage(roll, attacker));
            attacker.GainExperience(10);
        }


        public int CalculateAttack(int roll, ICharacter attacker, ICharacter target)
        {
            var attackerAbilityMod = attacker.AttackBonusMod;
            //Special Cases for Classes (Refactor when possible)
            var classBonus = CalculateClassBonus(attacker, target);
            //Special Cases for Races (Refactor when possible)
            var raceBonus = CalculateRaceBonus(attacker, target);
            return roll + attackerAbilityMod + classBonus + raceBonus + (attacker.Level / attacker.AttackPerLevelDivisor);
        }

        private static int CalculateClassBonus(ICharacter attacker, ICharacter target)
        {
            var classBonus = 0;
            if (attacker.Classes == null) return classBonus;
            var firstOrDefault = attacker.Classes.FirstOrDefault();
            if (firstOrDefault != null && firstOrDefault.ClassName != "Paladin") return classBonus;
            if (target.Alignment == Alignments.Evil)
                classBonus = 2;
            
            return classBonus;
        }
        private static int CalculateRaceBonus(ICharacter attacker, ICharacter target)
        {
            var raceBonus = 0;
            if (attacker.Races == null) return raceBonus;
            if (attacker.Races.Single().RaceName != "Dwarf") return raceBonus;
            if (target.Races.Single().RaceName != "Orc") return raceBonus;
            raceBonus = 2;
            return raceBonus;
        }


        public bool CheckIfHit(int attackTotal, int targetAc)
        {
            return attackTotal > targetAc;
        }

        public int CalculateDamage(int roll, ICharacter attacker)
        {
            var attackerStrengthMod = attacker.Abilities.Strength.Modifier;
            var damage = attacker.BaseDamage + attackerStrengthMod;
            if (roll == 20)
                damage = damage * attacker.CritMultiplier;
            if (damage < 1)
                damage = 1;
            return damage;
        }

        public bool IsHit { get; private set; }
    }
}

[TestClass]
public class CombatTests
{
    private ICharacter _attacker;
    private ICharacter _strongMan;
    private ICharacter _target;
    private const int LowRoll = 8;
    private const int EqualRoll = 10;
    private const int HighRoll = 18;
    private const int CritRoll = 20;

    [TestInitialize]
    public void Initialize()
    {
        _attacker = new BaseCharacter();

        var abilities = new AbilityScores(12);
        _strongMan = new BaseCharacter(abilities);
        _target = new BaseCharacter();
    }

    [TestMethod]
    public void AnAttackLessThanArmorClassMisses()
    {
        var hit = _attacker.Attack(LowRoll ,_target);
        Assert.IsFalse(hit);
    }

    [TestMethod]
    public void AnAttackEqualToArmorClassMisses()
    {
        var hit =_attacker.Attack(EqualRoll, _target);
        Assert.IsFalse(hit);
    }

    [TestMethod]
    public void AnAttackGreaterThanArmorClassHits()
    {
        var hit = _attacker.Attack(HighRoll, _target);
        Assert.IsTrue(hit);
    }

    [TestMethod]
    public void AHitDealsOneDamage()
    {
        _attacker.Attack(HighRoll, _target);
        Assert.AreEqual(1, _target.CurrentDamage);
    }

    [TestMethod]
    public void AnAttackWithARollOfTwentyDealsTwoDamage()
    {
        _attacker.Attack(CritRoll, _target);
        Assert.AreEqual(2, _target.CurrentDamage);
    }

    [TestMethod]
    public void StrengthIsAddedToAttack()
    {
        var hit = _strongMan.Attack(EqualRoll, _target);
        Assert.IsTrue(hit);
    }

    [TestMethod]
    public void StrengthIsAddedToDamage()
    {
        _strongMan.Attack(HighRoll, _target);
        Assert.AreEqual(2, _target.CurrentDamage);
    }

    [TestMethod]
    public void StrengthDamageIsDoubledOnCrit()
    {
        _strongMan.Attack(CritRoll, _target);
        Assert.AreEqual(4, _target.CurrentDamage);
    }

    [TestMethod]
    public void MinimumDamageOnHitIsOne()
    {
        _attacker.Attack(HighRoll, _target);
        Assert.AreEqual(1, _target.CurrentDamage);
    }

    [TestMethod]
    public void CharactersAreDeadWhenTheirHitPointsAreZeroOrLess()
    {
        for (var i = 0; i < 4; i++)
        {
            _attacker.Attack(CritRoll, _target);
        }

        Assert.IsTrue(_target.IsDead);
    }

    [TestMethod]
    public void ExperienceIsGainedWhenAttacksLand()
    {
        _attacker.Attack(HighRoll, _target);
        Assert.AreEqual(10, _attacker.Experience);
    }

    [TestMethod]
    public void AttackIncreasesOneEveryOtherLevel()
    {
        for (var i = 0; i < 200; i++)
        {
            _attacker.Attack(HighRoll, _target);
        }
        var hit = _attacker.Attack(EqualRoll, _target);
        Assert.IsTrue(hit);
    }

    [TestMethod]
    public void RoguesDoTripleDamageOnCrit()
    {
        _attacker = new Rogue(_attacker);
        _attacker.Attack(CritRoll, _target);
        Assert.AreEqual(3, _target.CurrentDamage);
    }

    [TestMethod]
    public void RouguesIgnoreDexterityBonusToArmorClass()
    {
        var abilities = new AbilityScores(dexterity: 12);
        var dexterousEnemy = new BaseCharacter(abilities);
        _attacker = new Rogue(_attacker);
        var hit = _attacker.Attack(EqualRoll + 1, dexterousEnemy);
        Assert.IsTrue(hit);
    }

    [TestMethod]
    public void RoguesAddDexInsteadOfStrengthToAttacks()
    {
        var abilities = new AbilityScores(dexterity: 12);
        ICharacter attacker = new BaseCharacter(abilities);
        attacker = new Rogue(attacker);
        var hit = attacker.Attack(EqualRoll, _target);
        Assert.IsTrue(hit);
    }
}