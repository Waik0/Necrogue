namespace CafeAssets.Script.System.GameTimeSystem
{
    public class GameTimeModel
    {
        public GameTimeModel(long first)
        {
            TotalMinutes = first;
        }

        public long TotalMinutes; //最小単位は分
        public long Minutes => TotalMinutes % 60;
        public long Hour => (TotalMinutes / 60) % 24;
        public long Day => (TotalMinutes / 1440) % 7 + 1;
        public long Week => (TotalMinutes / 10080) % 4 + 1;
        public long Month => (TotalMinutes / 40320) % 12 + 1;
        public long Year => (TotalMinutes / 483840) + 1;
    }
}

