using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TDnD;

    

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
