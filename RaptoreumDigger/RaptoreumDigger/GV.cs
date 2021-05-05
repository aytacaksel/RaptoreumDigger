using System.Threading;

namespace RaptoreumDigger
{
    public static class GV
    {
        public static double CurrentDifficulty;
        public static double CurrentTarget;
        public static bool StopMining = false;
        public static bool ResetMining = false;

        public static bool Bench = false;

        public static bool DarkTheme = false;

        public static Thread[] threads;
        public static bool[] threadStateList;
        public static uint[] NonceList;
        public static uint[] MaxNonceList;
    }

    public class HT
    {
        public int? id;
        public string method;

        public HT(int id, string method)
        {
            this.id = id;
            this.method = method;
        }

        public HT(int? id, string method)
        {
            this.id = id;
            this.method = method;
        }
    }


}
