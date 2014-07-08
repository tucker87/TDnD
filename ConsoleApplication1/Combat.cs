using System.Linq;

namespace TDnD
{
    public class Attack
    {
        public Attack(int roll, ICharacter attacker, ICharacter target)
        {
            var attackTotal = CalculateAttack(roll, attacker, target);

            var targetAc = attacker.AttacksFlatFootedAc
                ? target.FlatFootedArmorClass
                : target.GetArmorClass(attacker.Races.First());

            if (!CheckIfHit(attackTotal, targetAc)) return;
            IsHit = true;
            target.TakeDamage(CalculateDamage(roll, attacker, target));
            attacker.GainExperience(10);
        }


        public int CalculateAttack(int roll, ICharacter attacker, ICharacter target)
        {
            var attackerAbilityMod = attacker.AttackBonusMod;
            //Special Cases for Classes (Refactor when possible)
            var classBonus = CalculateClassBonus(attacker, target);
            //Special Cases for Races (Refactor when possible)
            var raceBonus = CalculateRaceBonus(attacker, target);
            var weaponBonus = attacker.Weapon.GetBonusAttack(target);
            var armorBonus = attacker.Armor.GetBonusAttack();
            return roll + attackerAbilityMod + classBonus + raceBonus + (attacker.Level / attacker.AttackPerLevelDivisor) + weaponBonus + armorBonus;
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

        public int CalculateDamage(int roll, ICharacter attacker, ICharacter target)
        {
            var attackerStrengthMod = attacker.Abilities.Strength.Modifier;
            var weaponBonus = attacker.Weapon.GetBonusDamage(target);
            var damage = attacker.BaseDamage + attackerStrengthMod + weaponBonus;
            if (roll == 20)
                damage = damage * attacker.CritMultiplier;
            if (damage < 1)
                damage = 1;
            return damage;
        }

        public bool IsHit { get; private set; }
    }
}