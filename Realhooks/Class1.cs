using System;
using System.IO;
using System.Reflection;
using human_fall_flat_hax;

namespace Realhooks
{
    public class loader
    {
        internal static object locker = new object();
        static hookies hookinstace = new hookies();

        public static void init()
        {
            lock (locker)
            {
                LoadMainDLL();
            }
        }

        public static void LoadMainDLL()
        {
            hookinstace.allhooks();
        }
    }
}