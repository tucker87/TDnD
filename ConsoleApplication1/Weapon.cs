using System.Linq;

namespace TDnD
{
    public abstract class Weapon
    {
        public virtual int GetBonusDamage(ICharacter enemy) { return 0; }
        public virtual int CriticalMultiplier { get { return 0; } }
        public virtual int GetBonusAttack(ICharacter enemy) { return 0; }
    }

    public class Fists : Weapon { }

    public class LongSword : Weapon
    {
        public override int GetBonusDamage(ICharacter enemy) { return 5; }
    }

    public class WarAxe : Weapon
    {
        private readonly ICharacter _character;
        public WarAxe(ICharacter character)
        {
            _character = character;
        }
        public override int GetBonusDamage(ICharacter enemy) { return 6; }
        public override int CriticalMultiplier
        {
            get
            {
                return _character.Classes.First().ClassName == "Rogue" ? 5 : 3;
            }
        }
    }

    public class NunChucks : Weapon
    {
        private ICharacter _character;

        public NunChucks(ICharacter character)
        {
            _character = character;
        }

        public override int GetBonusAttack(ICharacter enemy)
        {
            if (_character.Classes.First().ClassName != "Monk")
                return -4;
            return 0;
        }

        public override int GetBonusDamage(ICharacter enemy)
        {
            return 6;
        }
    }

    public class ElvenWeapon : Weapon
    {
        private readonly int _bonus;
        private readonly Weapon _weapon;
        private readonly ICharacter _character;
        public ElvenWeapon(int bonus, Weapon weapon, ICharacter character)
        {
            _bonus = bonus;
            _weapon = weapon;
            _character = character;
        }

        public override int GetBonusDamage(ICharacter enemy)
        {
            var racialBonus = 0;
            var characterRace = _character.Races.First().RaceName;
            var enemyRace = enemy.Races.First().RaceName;

            if (characterRace == "Elf" || enemyRace == "Orc")
                racialBonus = 1;

            if (characterRace == "Elf" && enemyRace == "Orc")
                racialBonus = 4;

            return _weapon.GetBonusDamage(enemy) + _bonus + racialBonus;
        }

        public override int GetBonusAttack(ICharacter enemy)
        {
            var characterRace = _character.Races.First().RaceName;
            var enemyRace = enemy.Races.First().RaceName;

            if (characterRace == "Elf" && enemyRace == "Orc")
                return _weapon.GetBonusAttack(enemy) + _bonus + 4;

            if (characterRace == "Elf" || enemyRace == "Orc")
                return _weapon.GetBonusAttack(enemy) + _bonus + 1;

            return _weapon.GetBonusAttack(enemy) + _bonus;
        }
    }


    public class MagicWeapon : Weapon
    {
        private readonly int _bonus;
        private readonly Weapon _weapon;
        public MagicWeapon(int bonus, Weapon weapon)
        {
            _bonus = bonus;
            _weapon = weapon;
        }
        public override int GetBonusAttack(ICharacter enemy) { return _weapon.GetBonusAttack(enemy) + _bonus; }
        public override int GetBonusDamage(ICharacter enemy) { return _weapon.GetBonusDamage(enemy) + _bonus; }
        public override int CriticalMultiplier { get { return _weapon.CriticalMultiplier; } }
    }

}