using System;
using System.Threading;

namespace RayTracer
{
    static class ThreadLocalRandom
    {
        private static readonly Random _globalRandom = new Random();
        private static readonly object _globalLock = new object();

        private static readonly ThreadLocal<Random> _threadRandom = new ThreadLocal<Random>(NewRandom);

        public static Random NewRandom()
        {
            lock (_globalLock)
            {
                return new Random(_globalRandom.Next());
            }
        }

        public static Random Instance 
        { 
            get { return _threadRandom.Value; } 
        }

        public static double NextDouble()
        {
            return Instance.NextDouble();
        }
    }
}
