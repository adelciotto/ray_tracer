using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RayTracer
{
    static class MathExt
    {
        private const float _toRadians = MathF.PI / 180.0f;

        public static float ToRadians(float degrees)
        {
            return degrees * _toRadians;
        }

        public static float RandomFloat()
        {
            return (float)ThreadLocalRandom.NextDouble();
        }

        public static float RandomFloat(float min, float max)
        {
            return min + (max - min) * (float)ThreadLocalRandom.NextDouble();
        }

        public static Vector3 RandomVector3()
        {
            return new Vector3(MathExt.RandomFloat(), MathExt.RandomFloat(), MathExt.RandomFloat());
        }

        public static Vector3 RandomVector3(float min, float max)
        {
            return new Vector3(MathExt.RandomFloat(min, max), MathExt.RandomFloat(min, max), MathExt.RandomFloat(min, max));
        }

        public static Vector3 RandomInUnitSphere()
        {
            while (true)
            {
                Vector3 p = RandomVector3(-1.0f, 1.0f);
                if (p.LengthSquared() >= 1.0f) 
                    continue;

                return p;
            }
        }

        public static Vector3 RandomInUnitDisk()
        {
            while (true)
            {
                var p = new Vector3(MathExt.RandomFloat(-1.0f, 1.0f), MathExt.RandomFloat(-1.0f, 0.0f), 0.0f);
                if (p.LengthSquared() >= 1.0f) 
                    continue;

                return p;
            }
        }

        public static Vector3 RandomUnitVector()
        {
            return Vector3.Normalize(RandomInUnitSphere());
        }
        
        public static Vector3 RandomInHemisphere(Vector3 normal)
        {
            var inUnitSphere = RandomInUnitSphere();
            if (Vector3.Dot(inUnitSphere, normal) > 0.0f)
                // In the same hemisphere as the normal.
                return inUnitSphere;
            else
                return -inUnitSphere;
        }
    }
}
