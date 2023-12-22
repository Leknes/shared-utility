using System;

namespace Senkel.Utility.Random
{
    public static class RandomUtility
    {
#if !NET6_0_OR_GREATER
        private static System.Random _shared = new System.Random();
#endif

        public static System.Random Shared
        {
            get
            {  
#if !NET6_0_OR_GREATER
                return _shared;
#else
                return System.Random.Shared;
#endif
            }
        }
 
    }
}
