using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prototyping;

namespace Prototyping
{
    public abstract class Character
    {
        public abstract int HitPoints { get; }
        public virtual int ArmorClass { get; set; }
        public Class Class { get; set; }
        public virtual int FlatFootedArmorClass { get; set; }
        public virtual int Level { get; set; }
        public string Name { get; set; }
        public Race Race { get; set; }

        public bool IsDead;
        public int BaseDamage;
        public int AttackBonusMod;
        public int CritMultiplier;
        public bool AttacksFlatFootedAc;
        public int AttackPerLevelDivisor;
        public int BaseHitPoints;
        
        public Abilities Abilities = new Abilities(new AbilityScores());
        public int ArmorClassBonusFromClass;
        public enum Alignments { Good, Neutral, Evil }
        public Alignments Alignment;
        public int CurrentDamage;
        public int Experience;

        public abstract void GainExperience(int xp);
        public abstract void TakeDamage(int damage);
        public abstract bool Attack(int roll, Character target);
    } 

    public class BaseCharacter : Character
    {
        public BaseCharacter(string name = "John Doe", Alignments alignment = Alignments.Neutral)
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
        }

        private const int BaseAbilityScore = 10;
        private const int BaseArmorClass = 10;
        private readonly Attack _attack = new Attack();

        public override int HitPoints { get { return (BaseHitPoints + Abilities.Constitution.Modifier) * Level; } }
        public override int ArmorClass { get { return BaseArmorClass + Abilities.Dexterity.Modifier + ArmorClassBonusFromClass; } }
        public override int FlatFootedArmorClass { get { return BaseArmorClass; } }
        public override int Level { get { return (Experience / 1000) + 1; } }

        public override bool Attack(int roll, Character target)
        {
            var attackTotal = _attack.CalculateAttack(roll, this);
            var targetAc = AttacksFlatFootedAc ? target.FlatFootedArmorClass : target.ArmorClass;
            var hit = _attack.CheckIfHit(attackTotal, targetAc);
            if (!hit) return false;
            
            var damage = _attack.CalculateDamage(roll, this);
            target.TakeDamage(damage);
            GainExperience(10);
            return true;
        }

        public override void TakeDamage(int damage)
        {
            CurrentDamage += damage;
            if (HitPoints - CurrentDamage <= 0)
                IsDead = true;
        }

        public override void GainExperience(int xp)
        {
            Experience += xp;
        }
    }

    public class AbilityScores
    {
        public AbilityScores() { }
        private const int BaseAbilityScore = 10;
        public AbilityScores(int strength = BaseAbilityScore, int dexterity = BaseAbilityScore, int constituion = BaseAbilityScore, int wisdom = BaseAbilityScore, int intelligence = BaseAbilityScore, int charisma = BaseAbilityScore)
        {
            Strength = strength;
            Dexterity = dexterity;
            Constitution = constituion;
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


    public abstract class Race { }

    public class Orc : Race
    {
        public Orc(Character character)
        {
            character.Abilities.Strength.RaceBonus = 2;
        }
    }

    public abstract class Class : Character
    {
        private readonly Character _character;
        protected Class(Character character)
        {
            _character = character;
        }

        public override int HitPoints { get { return _character.HitPoints; } }

        public override bool Attack(int roll, Character target)
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

    public class Fighter : Class
    {
        public Fighter(Character character) : base(character)
        {
            character.BaseHitPoints = 10;
            character.AttackPerLevelDivisor = 1;
        }
    }

    public class Rogue : Class
    {
        public Rogue(Character character) : base(character)
        {
            character.CritMultiplier = 3;
            character.AttacksFlatFootedAc = true;
            character.AttackBonusMod = character.Abilities.Dexterity.Modifier;
        }
    }

    public class Monk : Class
    {
        public Monk(Character character) : base(character)
        {
            character.BaseHitPoints = 6;
            character.BaseDamage = 3;
            if (character.Abilities.Wisdom.Modifier > 0)
                character.ArmorClassBonusFromClass = character.Abilities.Wisdom.Modifier;
        }
        
    }

    public class Paladin : Class
    {
        public Paladin(Character character) : base(character)
        {
            character.BaseHitPoints = 8;
        }

        public override bool Attack(int roll, Character target)
        {
            if (target.Alignment == Alignments.Evil)
            base.Attack(roll, target);
            return false;
        }
    }
}

[TestClass]
public class Testing
{
    private Character _character;
    private Character _enemy;

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
        Assert.AreEqual(Character.Alignments.Neutral, _character.Alignment);
    }

    [TestMethod]
    public void CharactersAlignmentCanBeSet()
    {
        var goodyTwoShoes = new BaseCharacter("Cleric", Character.Alignments.Good);
        Assert.AreEqual(Character.Alignments.Good, goodyTwoShoes.Alignment);
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
        var abilities = new AbilityScores(constituion: 12);
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
        var abilities = new AbilityScores(constituion: 12);
        var toughCharacter = new BaseCharacter(abilities);
        for (var i = 0; i < 100; i++)
        {
            //var attack = new Attack(18, toughCharacter, _enemy);
            toughCharacter.Attack(18, _enemy);
        }
        Assert.AreEqual(12, toughCharacter.HitPoints);
    }

    [TestMethod]
    public void OrcsAddTwoToStrength()
    {
        _character.Race = new Orc(_character);
        Assert.AreEqual(12, _character.Abilities.Strength.Score);
    }

    [TestMethod]
    public void FightersAttackGoesUpEveryLevel()
    {
        _character.Class = new Fighter(_character);
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
        _character.Class = new Fighter(_character);

        Assert.AreEqual(10, _character.HitPoints);
    }

    [TestMethod]
    public void MonksGetSixHpPerLevel()
    {
        _character.Class = new Monk(_character);
        for (var i = 0; i < 100; i++)
        {
            //var attack = new Attack(18, _character, _enemy);
            _character.Attack(18, _enemy);
        }
        Assert.AreEqual(12, _character.HitPoints);
    }

    [TestMethod]
    public void MonksDoThreeDamageOnAttack()
    {
        _character.Class = new Monk(_character);
        //var attack = new Attack(18, _character, _enemy);
        _character.Attack(18, _enemy);
        Assert.AreEqual(3, _enemy.CurrentDamage);
    }

    [TestMethod]
    public void MonksAddArmorClassToAc()
    {
        var abilites = new AbilityScores(wisdom: 12);
        var wiseCharacter = new BaseCharacter(abilites);
        _character.Class = new Monk(wiseCharacter);
        Assert.AreEqual(11, wiseCharacter.ArmorClass);
    }

    [TestMethod]
    public void MonksDontAddNegativeWisdomToAc()
    {
        var abilites = new AbilityScores(wisdom: 8);
        var wiseCharacter = new BaseCharacter(abilites);
        _character.Class = new Monk(wiseCharacter);
        Assert.AreEqual(10, wiseCharacter.ArmorClass);
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
        var evilEnemy = new BaseCharacter(alignment: Character.Alignments.Evil);
        _character.Class = new Paladin(_character);
        //var attack = new Attack(9, _character, evilEnemy);
        var hit = _character.Attack(9, evilEnemy);
        Assert.IsTrue(hit);
    }

}