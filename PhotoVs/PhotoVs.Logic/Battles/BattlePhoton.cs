namespace PhotoVs.Logic.Battles
{
    public class BattlePhoton
    {
        public int Speed { get; }

        public int Priority { get; }

        public bool HasMoved { get; }

        public bool CanAttack { get; }

        public BattlePhoton()
        {
            Speed = 0;
            Priority = 0;
            HasMoved = false;
            CanAttack = true;
        }
    }
}