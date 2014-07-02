using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prototyping;

namespace Prototyping
{
    public interface ICharacter
    {
        int HitPoints { get; }
        int ArmorClass { get; }
        int FlatFootedArmorClass { get; set; }
        int Level { get; set; }
        string Name { get; set; }
        Class Class { get; set; }
        Race Race { get; set; }
        bool IsDead { get; set; }
        int BaseDamage { get; set; }
        int AttackBonusMod { get; set; }
        int CritMultiplier { get; set; }
        bool AttacksFlatFootedAc { get; set; }
        int AttackPerLevelDivisor { get; set; }
        int BaseHitPoints { get; set; }

        Abilities Abilities = new Abilities(new AbilityScores());
        int ArmorClassBonusFromClass { get; set; }

        Alignments Alignment { get; set; }
        int CurrentDamage { get; set; }
        int Experience { get; set; }
        int ArmorClassBonusFromRace { get; set; }

        void GainExperience(int xp);
        void TakeDamage(int damage);
        bool Attack(int roll, ICharacter target);
    }

    public enum Alignments { Good, Neutral, Evil }

    public class BaseCharacter : ICharacter
    {
        public BaseCharacter(string name = "John Doe", ICharacter.Alignments alignment = ICharacter.Alignments.Neutral)
        {
            Abilities.Strength.BaseScore = BaseAbilityScore;
            Abilities.Dexterity.BaseScore = BaseAbilityScore;
            Abilities.Constitution.BaseScore = BaseAbilityScore;
            Abilities.Intelligence.BaseScore = BaseAbilityScore;
            Abilities.Wisdom.BaseScore = BaseAbilityScore;
            Abilities.Charisma.BaseScore = BaseAbilityScore;

            Init(name, alignment);
        }

        public BaseCharacter(AbilityScores baseAbilityScores, string name = "John Doe", Alignments alignment = Alignments.Neutral)
        {
            Abilities.Strength.BaseScore = baseAbilityScores.Strength;
            Abilities.Dexterity.BaseScore = baseAbilityScores.Dexterity;
            Abilities.Constitution.BaseScore = baseAbilityScores.Constitution;
            Abilities.Intelligence.BaseScore = baseAbilityScores.Intelligence;
            Abilities.Wisdom.BaseScore = baseAbilityScores.Wisdom;
            Abilities.Charisma.BaseScore = baseAbilityScores.Charisma;

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
            Race = new Human(this);
        }

        private const int BaseAbilityScore = 10;
        private const int BaseArmorClass = 10;
        //private readonly Attack _attack = new Attack();
        private int BonusHpFromCon
        {
            get
            {
                return Race.RaceName == "Dwarf" ? (Abilities.Constitution.Modifier * 2) : Abilities.Constitution.Modifier;
            }
        }
        public int HitPoints { get { return (BaseHitPoints + BonusHpFromCon) * Level; } }
        public  int ArmorClass { get { return BaseArmorClass + Abilities.Dexterity.Modifier + ArmorClassBonusFromRace + ArmorClassBonusFromClass; } }
        int ICharacter.FlatFootedArmorClass { get; set; }
        int ICharacter.Level { get; set; }
        public string Name { get; set; }
        public Class Class { get; set; }
        public Race Race { get; set; }
        public bool IsDead { get; set; }
        public int BaseDamage { get; set; }
        public int AttackBonusMod { get; set; }
        public int CritMultiplier { get; set; }
        public bool AttacksFlatFootedAc { get; set; }
        public int AttackPerLevelDivisor { get; set; }
        public int BaseHitPoints { get; set; }
        public int ArmorClassBonusFromClass { get; set; }
        public Alignments Alignment { get; set; }
        public int CurrentDamage { get; set; }
        public int Experience { get; set; }
        public int ArmorClassBonusFromRace { get; set; }
        public  int FlatFootedArmorClass { get { return BaseArmorClass; } }
        public  int Level { get { return (Experience / 1000) + 1; } }

        public  bool Attack(int roll, ICharacter target)
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

        public  void TakeDamage(int damage)
        {
            CurrentDamage += damage;
            if (HitPoints - CurrentDamage <= 0)
                IsDead = true;
        }

        public  void GainExperience(int xp)
        {
            Experience += xp;
        }
    }

    public class AbilityScores
    {
        public AbilityScores() { }
        private const int BaseAbilityScore = 10;
        public AbilityScores(int strength = BaseAbilityScore, int dexterity = BaseAbilityScore, int constitution = BaseAbilityScore, int wisdom = BaseAbilityScore, int intelligence = BaseAbilityScore, int charisma = BaseAbilityScore)
        {
            Strength = strength;
            Dexterity = dexterity;
            Constitution = constitution;
            Wisdom = wisdom;
            Intelligence = intelligence;
            Charisma = charisma;
        }

        public int Strength;
        public int Dexterity;
        public int Constitution;
        public int Wisdom;
        public int Intelligence;
        public int Charisma;
    }

    public class Abilities
    {
        public Abilities(AbilityScores scores)
        {
            Strength.BaseScore = scores.Strength;
            Dexterity.BaseScore = scores.Dexterity;
            Constitution.BaseScore = scores.Constitution;
            Wisdom.BaseScore = scores.Wisdom;
            Intelligence.BaseScore = scores.Intelligence;
            Charisma.BaseScore = scores.Charisma;
        }
        public StrengthAbility Strength = new StrengthAbility();
        public DexterityAbility Dexterity = new DexterityAbility();
        public ConstitutionAbility Constitution = new ConstitutionAbility();
        public WisdomAbility Wisdom = new WisdomAbility();
        public IntelligenceAbility Intelligence = new IntelligenceAbility();
        public CharismaAbility Charisma = new CharismaAbility();
    }

    public abstract class Ability
    {
        public int Score { get { return BaseScore + RaceBonus + ClassBonus; } }
        public int RaceBonus { get; set; }
        public int Modifier { get { return (Score / 2) - 5; } }
        private int _baseScore;
        public int BaseScore
        {
            get { return _baseScore; }
            set
            {
                if (value < 1)
                    value = 1;
                if (value > 20)
                    value = 20;
                _baseScore = value;
            }
        }
        public int ClassBonus { get; set; }
    }

    //This could be undone, I misread something....
    public sealed class StrengthAbility : Ability { }
    public sealed class DexterityAbility : Ability { }
    public sealed class ConstitutionAbility : Ability { }
    public sealed class WisdomAbility : Ability { }
    public sealed class IntelligenceAbility : Ability { }
    public sealed class CharismaAbility : Ability { }

    public abstract class Class : ICharacter
    {
        private readonly ICharacter _character;
        protected Class(ICharacter character)
        {
            _character = character;
        }


        public override int ArmorClass { get { return _character.ArmorClass; } }
        public override int HitPoints { get { return _character.HitPoints; } }

        public override bool Attack(int roll, ICharacter target)
        {
            return _character.Attack(roll, target);
        }
        public override void GainExperience(int xp)
        {
            _character.GainExperience(xp);
        }

        public override void TakeDamage(int damage)
        {
            _character.TakeDamage(damage);
        }

        public string ClassName = "";
    }

    public class Fighter : Class
    {
        public Fighter(ICharacter character)
            : base(character)
        {
            character.BaseHitPoints = 10;
            character.AttackPerLevelDivisor = 1;
        }
    }

    public class Rogue : Class
    {
        public Rogue(ICharacter character)
            : base(character)
        {
            character.CritMultiplier = 3;
            character.AttacksFlatFootedAc = true;
            character.AttackBonusMod = character.Abilities.Dexterity.Modifier;
        }
    }

    public class Monk : Class
    {
        public Monk(ICharacter character)
            : base(character)
        {
            character.BaseHitPoints = 6;
            character.BaseDamage = 3;
            if (character.Abilities.Wisdom.Modifier > 0)
                character.ArmorClassBonusFromClass = character.Abilities.Wisdom.Modifier;
        }

    }

    public class Paladin : Class
    {
        public Paladin(ICharacter character)
            : base(character)
        {
            ClassName = "Paladin";
            character.BaseHitPoints = 8;
        }
    }

    public abstract class Race : ICharacter
    {
        private readonly ICharacter _character;

        protected Race(ICharacter character)
        {
            _character = character;
        }

        public string RaceName { get; set; }

        public override int ArmorClass { get { return _character.ArmorClass; } }
        public override int HitPoints { get { return _character.HitPoints; } }

        public override bool Attack(int roll, ICharacter target)
        {
            return _character.Attack(roll, target);
        }
        public override void GainExperience(int xp)
        {
            _character.GainExperience(xp);
        }

        public override void TakeDamage(int damage)
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
        public Orc(ICharacter character)
            : base(character)
        {
            character.Abilities.Strength.RaceBonus = 2;
            character.Abilities.Intelligence.RaceBonus = -1;
            character.Abilities.Wisdom.RaceBonus = -1;
            character.Abilities.Charisma.RaceBonus = -1;

            character.ArmorClassBonusFromRace = 2;

            RaceName = "Orc";
        }
    }

    public class Dwarf : Race
    {
        public Dwarf(ICharacter character)
            : base(character)
        {
            character.Abilities.Constitution.RaceBonus = 1;
            character.Abilities.Charisma.RaceBonus = -1;
            RaceName = "Dwarf";
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
            Assert.AreEqual(ICharacter.Alignments.Neutral, _character.Alignment);
        }

        [TestMethod]
        public void CharactersAlignmentCanBeSet()
        {
            var goodyTwoShoes = new BaseCharacter("Cleric", ICharacter.Alignments.Good);
            Assert.AreEqual(ICharacter.Alignments.Good, goodyTwoShoes.Alignment);
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
            Assert.AreEqual(12, dexterousAttacker.ArmorClass);
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
                //var attack = new Attack(18, _character, _enemy);
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
                //var attack = new Attack(18, toughCharacter, _enemy);
                toughCharacter.Attack(18, _enemy);
            }
            Assert.AreEqual(12, toughCharacter.HitPoints);
        }

        [TestMethod]
        public void AllCharactersRaceDefaultsToHuman()
        {
            Assert.AreEqual("Human", _character.Race.RaceName);
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
            _character.Class = new Fighter(_character);

            _enemy = new BaseCharacter();
        }

        [TestMethod]
        public void FightersAttackGoesUpEveryLevel()
        {

            for (var i = 0; i < 100; i++)
            {
                //attack = new Attack(18, _character, _enemy);
                _character.Attack(18, _enemy);
            }
            //attack = new Attack(10, _character, _enemy);
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
            _monk.Class = new Monk(_monk);

            _enemy = new BaseCharacter();
        }

        [TestMethod]
        public void MonksGetSixHpPerLevel()
        {
            _monk.Class = new Monk(_monk);
            for (var i = 0; i < 100; i++)
            {
                //var attack = new Attack(18, _character, _enemy);
                _monk.Attack(18, _enemy);
            }
            Assert.AreEqual(12, _monk.HitPoints);
        }

        [TestMethod]
        public void MonksDoThreeDamageOnAttack()
        {
            _monk.Class = new Monk(_monk);
            //var attack = new Attack(18, _character, _enemy);
            _monk.Attack(18, _enemy);
            Assert.AreEqual(3, _enemy.CurrentDamage);
        }

        [TestMethod]
        public void MonksAddArmorClassToAc()
        {
            var abilites = new AbilityScores(wisdom: 12);
            var wiseCharacter = new BaseCharacter(abilites);
            _monk.Class = new Monk(wiseCharacter);
            Assert.AreEqual(11, wiseCharacter.ArmorClass);
        }

        [TestMethod]
        public void MonksDontAddNegativeWisdomToAc()
        {
            var abilites = new AbilityScores(wisdom: 8);
            var wiseCharacter = new BaseCharacter(abilites);
            _monk.Class = new Monk(wiseCharacter);
            Assert.AreEqual(10, wiseCharacter.ArmorClass);
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
            _character.Class = new Paladin(_character);

            _enemy = new BaseCharacter();
        }

        [TestMethod]
        public void PaladinsGetEightHpPerLevel()
        {
            _character.Class = new Paladin(_character);
            for (var i = 0; i < 100; i++)
            {
                //var attack = new Attack(18, _character, _enemy);
                _character.Attack(18, _enemy);
            }
            Assert.AreEqual(16, _character.HitPoints);
        }

        [TestMethod]
        public void PaladinsGetTwoExtraAttackWhenAttackingEvilCharacters()
        {
            var evilEnemy = new BaseCharacter(alignment: ICharacter.Alignments.Evil);
            _character.Class = new Paladin(_character);
            //var attack = new Attack(9, _character, evilEnemy);
            var hit = _character.Attack(9, evilEnemy);
            Assert.IsTrue(hit);
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
            _character.Race = new Orc(_character);
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
            _character.Race = new Dwarf(_character);
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
            var character = new BaseCharacter(abilities);
            character.Race = new Dwarf(character);
            Assert.AreEqual(9, character.HitPoints);
        }

        [TestMethod]
        public void DwarvesGetTwoAttackWhenTargetIsOrc()
        {
            var enemy = new BaseCharacter();
            enemy.Race = new Orc(enemy);
            const int justEnoughToMissByTwoTakingOrcAcBonusIntoAccount = 11;
            var attack = new Attack(justEnoughToMissByTwoTakingOrcAcBonusIntoAccount, _character, enemy);
            Assert.IsTrue(attack.IsHit);
        }
    }
}