using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prototyping;

namespace Prototyping
{
    public interface ICharacter
    {
        Alignments Alignment { get; set; }
        Abilities Abilities { get; set; }
        int AttackBonusMod { get; set; }
        bool AttacksFlatFootedAc { get; set; }
        int AttackPerLevelDivisor { get; set; }
        int BaseDamage { get; set; }
        int BaseHitPoints { get; set; }
        List<Class> Classes { get; }
        int CritMultiplier { get; set; }
        int CurrentDamage { get; set; }
        int Experience { get; set; }
        int FlatFootedArmorClass { get; }
        int HitPoints { get; }
        bool IsDead { get; set; }
        int Level { get; }
        string Name { get; set; }
        List<Race> Races { get; }
        Weapon Weapon { get; set; }

        bool Attack(int roll, ICharacter target);
        void GainExperience(int xp);
        int GetArmorClass();
        int GetArmorClass(Race enemyRace);
        void TakeDamage(int damage);

        
    }

    public enum Alignments { Good, Neutral, Evil }

    public class BaseCharacter : ICharacter
    {
        public BaseCharacter(string name = "John Doe", Alignments alignment = Alignments.Neutral)
        {
            Abilities = new Abilities(new AbilityScores())
            {
                Strength = {Score = BaseAbilityScore},
                Dexterity = {Score = BaseAbilityScore},
                Constitution = {Score = BaseAbilityScore},
                Intelligence = {Score = BaseAbilityScore},
                Wisdom = {Score = BaseAbilityScore},
                Charisma = {Score = BaseAbilityScore}
            };

            Init(name, alignment);
        }

        public BaseCharacter(AbilityScores baseAbilityScores, string name = "John Doe", Alignments alignment = Alignments.Neutral)
        {
            Abilities = new Abilities(new AbilityScores())
            {
                Strength = {Score = baseAbilityScores.Strength},
                Dexterity = {Score = baseAbilityScores.Dexterity},
                Constitution = {Score = baseAbilityScores.Constitution},
                Intelligence = {Score = baseAbilityScores.Intelligence},
                Wisdom = {Score = baseAbilityScores.Wisdom},
                Charisma = {Score = baseAbilityScores.Charisma}
            };

            Init(name, alignment);
        }

        private void Init(string name = "John Doe", Alignments alignment = Alignments.Neutral)
        {
            Name = name;
            Alignment = alignment;
            AttackBonusMod = Abilities.Strength.Modifier;
            BaseHitPoints = 5;
            AttacksFlatFootedAc = false;
            AttackPerLevelDivisor = 2;
            CritMultiplier = 2;
            AttackBonusMod = Abilities.Strength.Modifier;
            BaseDamage = 1;
            Races = new List<Race> {new Human(this)};
            Classes = new List<Class>();
            Weapon = new Fists();
            Classes = new List<Class> {new Peasant(this)};
        }

        private const int BaseAbilityScore = 10;
        private const int BaseArmorClass = 10;

        private int BonusHpFromCon
        {
            get
            {
                var bonus = Abilities.Constitution.Modifier;
                if (Races.Single().RaceName == "Dwarf")
                    bonus = bonus * 2;
                return bonus;
            }
        }

        public int AttackBonusMod { get; set; }
        public Abilities Abilities { get; set; }
        public Alignments Alignment { get; set; }
        public bool AttacksFlatFootedAc { get; set; }
        public int AttackPerLevelDivisor { get; set; }
        public int BaseDamage { get; set; }
        public int BaseHitPoints { get; set; }
        private int _criticalMultiplier ;
        public int CritMultiplier
        {
            get { return Math.Max(_criticalMultiplier, Weapon.CriticalMultiplier); }
            set { _criticalMultiplier = value; }
        }

        public List<Class> Classes { get; set; }
        public int CurrentDamage { get; set; }
        public int Experience { get; set; }
        public int FlatFootedArmorClass { get { return BaseArmorClass; } }
        public int HitPoints { get { return (BaseHitPoints + BonusHpFromCon) * Level; } }
        public bool IsDead { get; set; }
        public int Level { get { return (Experience / 1000) + 1; } }
        public List<Race> Races { get; set; }
        public string Name { get; set; }
        public Weapon Weapon { get; set; }

        public bool Attack(int roll, ICharacter target)
        {
            var attack = new Attack(roll, this, target);
            return attack.IsHit;
            //var attackTotal = _attack.CalculateAttack(roll, this);
            //var targetAc = AttacksFlatFootedAc ? target.FlatFootedArmorClass : target.ArmorClass;
            //var hit = _attack.CheckIfHit(attackTotal, targetAc);
            //if (!hit) return false;

            //var damage = _attack.CalculateDamage(roll, this);
            //target.TakeDamage(damage);
            //GainExperience(10);
            //return true;
        }

        public void TakeDamage(int damage)
        {
            CurrentDamage += damage;
            if (HitPoints - CurrentDamage <= 0)
                IsDead = true;
        }

        public void GainExperience(int xp)
        {
            Experience += xp;
        }

        public int GetArmorClass()
        {
            return BaseArmorClass + Abilities.Dexterity.Modifier;
        }

        public int GetArmorClass(Race enemyRace)
        {
            return BaseArmorClass + Abilities.Dexterity.Modifier;
        }
    }
}

[TestClass]
public class CharacterTesting
{
    [TestClass]
    public class BaseCharacterTests
    {
        private ICharacter _character;
        private ICharacter _enemy;

        [TestInitialize]
        public void Initialize()
        {
            _character = new BaseCharacter("John");
            _enemy = new BaseCharacter("Not John");
        }

        [TestMethod]
        public void CharactersCanHaveNames()
        {
            Assert.AreEqual("John", _character.Name);
        }

        [TestMethod]
        public void CharactersAlignmentIsNeutralByDefault()
        {
            Assert.AreEqual(Alignments.Neutral, _character.Alignment);
        }

        [TestMethod]
        public void CharactersAlignmentCanBeSet()
        {
            var goodyTwoShoes = new BaseCharacter("Cleric", Alignments.Good);
            Assert.AreEqual(Alignments.Good, goodyTwoShoes.Alignment);
        }

        [TestMethod]
        public void AbilityScoresAre10ByDefault()
        {
            Assert.AreEqual(10, _character.Abilities.Strength.Score);
        }

        [TestMethod]
        public void AbilityModifiersRangeFromOneToTwenty()
        {
            var abilities = new AbilityScores();
            var cheater = new BaseCharacter(abilities);
            Assert.AreEqual(1, cheater.Abilities.Strength.Score);
        }

        [TestMethod]
        public void AbilityModifiersAreTwoWhenScoreIs14()
        {
            var abilities = new AbilityScores(14);
            var strongAttacker = new BaseCharacter(abilities);
            Assert.AreEqual(2, strongAttacker.Abilities.Strength.Modifier);
        }

        [TestMethod]
        public void DexterityIsAddedToArmorClass()
        {
            var abilities = new AbilityScores(dexterity: 14);
            var dexterousAttacker = new BaseCharacter(abilities);
            Assert.AreEqual(12, dexterousAttacker.GetArmorClass());
        }

        [TestMethod]
        public void ConstitutionIsAddedToHitPoints()
        {
            var abilities = new AbilityScores(constitution: 12);
            var toughAttacker = new BaseCharacter(abilities);
            Assert.AreEqual(6, toughAttacker.HitPoints);
        }

        [TestMethod]
        public void CharactersStartLevelOne()
        {
            Assert.AreEqual(1, _character.Level);
        }

        [TestMethod]
        public void CharactersLevelUpWhenExperienceIsOneThousand()
        {
            for (var i = 0; i < 100; i++)
            {
                _character.Attack(18, _enemy);
            }
            Assert.AreEqual(2, _character.Level);
        }

        [TestMethod]
        public void CharactersHaveFivePlusConModHpPerLevel()
        {
            var abilities = new AbilityScores(constitution: 12);
            var toughCharacter = new BaseCharacter(abilities);
            for (var i = 0; i < 100; i++)
            {
                toughCharacter.Attack(18, _enemy);
            }
            Assert.AreEqual(12, toughCharacter.HitPoints);
        }

        [TestMethod]
        public void AllCharactersRaceDefaultsToHuman()
        {
            Assert.AreEqual("Human", _character.Races.Single().RaceName);
        }
    }
}