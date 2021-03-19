using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RayTracer
{
    public class Ray
    {
        public Ray()
        {
            Origin = Vector3.Zero;
            Direction = Vector3.Zero;
        }

        public Ray(Vector3 origin, Vector3 direction)
        {
            Origin = origin;
            Direction = direction;
        }

        public Vector3 Origin { get; set; }
        public Vector3 Direction { get; set; }

        public Vector3 At(float t)
        {
            return Origin + Direction * t;
        }
    }
}
