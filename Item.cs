using System.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prototyping;

namespace Prototyping
{
    public abstract class Item
    {
        public virtual int GetBonusArmorClass()
        {
            return 0;
        }

        public virtual int GetBonusStrength()
        {
            return 0;
        }
    }

    public class RingOfProtection : Item
    {
        public override int GetBonusArmorClass()
        {
            return base.GetBonusArmorClass() + 2;
        }
    }

    public class BeltOfGiantStrength : Item
    {
        public BeltOfGiantStrength(ICharacter character)
        {
            character.Abilities.Strength.Score += 4;
        }
    }
}

[TestClass]
public class RingOfProtectionTests
{
    private ICharacter _character;

    [TestInitialize]
    public void Initialize()
    {
        _character = new BaseCharacter();
        _character.Items.Add(new RingOfProtection());
    }

    [TestMethod]
    public void GivesPlusTwoAc()
    {
        Assert.AreEqual(12, _character.GetArmorClass());
    }

    [TestMethod]
    public void GivesPlusTwoAcForEach()
    {
        _character.Items.Add(new RingOfProtection());
        Assert.AreEqual(14, _character.GetArmorClass());
    }
}

[TestClass]
public class BeltOfGiantStrengthTests
{
    private ICharacter _character;

    [TestInitialize]
    public void Initialize()
    {
        _character = new BaseCharacter();
        _character.Items.Add(new BeltOfGiantStrength(_character));
    }

    [TestMethod]
    public void GivesPlusFourStr()
    {
        Assert.AreEqual(14, _character.Abilities.Strength.Score);
    }

    [TestMethod]
    public void TakingTheBeltOffLowersYourStr()
    {
        _character.Items.Clear();
        Assert.AreEqual(10, _character.Abilities.Strength.Score);
    }
}