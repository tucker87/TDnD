namespace Prototyping
{
    public class AbilityScores
    {
        public int Strength;
        public int Dexterity;
        public int Constitution;
        public int Wisdom;
        public int Intelligence;
        public int Charisma;
        public const int BaseAbilityScore = 10;

        public AbilityScores()
        {
        }

        public AbilityScores(int strength, int dexterity, int constitution, int wisdom, int intelligence, int charisma)
        {
            Strength = strength;
            Dexterity = dexterity;
            Constitution = constitution;
            Wisdom = wisdom;
            Intelligence = intelligence;
            Charisma = charisma;
        }

        public int Strength1
        {
            set { Strength = value; }
            get { return Strength; }
        }

        public int Dexterity1
        {
            set { Dexterity = value; }
            get { return Dexterity; }
        }

        public int Constitution1
        {
            set { Constitution = value; }
            get { return Constitution; }
        }

        public int Wisdom1
        {
            set { Wisdom = value; }
            get { return Wisdom; }
        }

        public int Intelligence1
        {
            set { Intelligence = value; }
            get { return Intelligence; }
        }

        public int Charisma1
        {
            set { Charisma = value; }
            get { return Charisma; }
        }
    }
}