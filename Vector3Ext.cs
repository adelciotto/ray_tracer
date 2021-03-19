using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RayTracer
{
    static class Vector3Ext
    {

        public static bool NearZero(Vector3 v)
        {
            const float s = 1e-8f;
            return (Math.Abs(v.X) < s) && (Math.Abs(v.Y) < s) && (Math.Abs(v.Z) < s);
        }

        public static Vector3 Refract(Vector3 uv, Vector3 n, float etaiOverEtat)
        {
            float cosTheta = Math.Min(Vector3.Dot(-uv, n), 1.0f);
            var perp = etaiOverEtat * (uv + cosTheta * n);
            var parallel = -MathF.Sqrt(Math.Abs(1.0f - perp.LengthSquared())) * n;

            return perp + parallel;
        }
    }
}
