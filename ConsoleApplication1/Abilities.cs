namespace TDnD
{
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
            Strength.Score = scores.Strength;
            Dexterity.Score = scores.Dexterity;
            Constitution.Score = scores.Constitution;
            Wisdom.Score = scores.Wisdom;
            Intelligence.Score = scores.Intelligence;
            Charisma.Score = scores.Charisma;
        }
        public Ability Strength = new Ability();
        public Ability Dexterity = new Ability();
        public Ability Constitution = new Ability();
        public Ability Wisdom = new Ability();
        public Ability Intelligence = new Ability();
        public Ability Charisma = new Ability();
    }

    public class Ability
    {
        public int Modifier { get { return (Score / 2) - 5; } }
        private int _baseScore;
        public int Score
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
    }
}
