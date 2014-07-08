namespace TDnD
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
