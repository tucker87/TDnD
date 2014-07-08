using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TDnD;

namespace TDnDTests
{
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
}