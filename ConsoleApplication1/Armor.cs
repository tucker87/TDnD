using System;
using System.Linq;

namespace TDnD
{
    public abstract class Armor
    {
        public virtual int GetBonusArmorClass()
        {
            return 0;
        }

        public virtual int GetBonusAttack()
        {
            return 0;
        }

        public virtual int GetBonusDamageReduction()
        {
            return 0;
        }
    }

    public class ClothArmor : Armor
    {
        public override int GetBonusArmorClass()
        {
            return 0;
        }
    }

    public class LeatherArmor : Armor
    {
        public override int GetBonusArmorClass()
        {
            return 2;
        }
    }

    public class ChainMail : Armor
    {
        public override int GetBonusArmorClass()
        {
            return 5;
        }
    }

    public class PlateArmor : Armor
    {
        private ICharacter _character;

        public PlateArmor(ICharacter character)
        {
            if (character.Races.First().RaceName != "Dwarf"
                && character.Classes.First().ClassName != "Fighter")
            {
                throw new ArgumentException("Only Fighters and Dwarves can equip Plate");
            }
        }

        public override int GetBonusArmorClass()
        {
            return 8;
        }
    }

    public class DamageReduction : Armor
    {
        private readonly int _amount;
        private readonly Armor _baseArmor;

        public DamageReduction(int amount, Armor baseArmor)
        {
            _amount = amount;
            _baseArmor = baseArmor;
        }

        public override int GetBonusArmorClass()
        {
            return _baseArmor.GetBonusArmorClass();
        }

        public override int GetBonusDamageReduction()
        {
            return _amount;
        }
    }

    public class ElvenArmor : Armor
    {
        private readonly Armor _baseArmor;
        private readonly ICharacter _character;

        public ElvenArmor(Armor baseArmor, ICharacter character)
        {
            _baseArmor = baseArmor;
            _character = character;
        }
        public override int GetBonusArmorClass()
        {
            var bonus = 0;
            if (_character.Races.First().RaceName == "Elf")
                bonus = 3;
            return _baseArmor.GetBonusArmorClass() + bonus;
        }

        public override int GetBonusAttack()
        {
            return _character.Races.First().RaceName == "Elf" ? 1 : 0;
        }
    }
}
