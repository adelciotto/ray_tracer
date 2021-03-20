using System.Numerics;

namespace RayTracer
{
    // Ray class represents a ray that is cast into the scene by the RayCaster.
    // The parametric equation for a ray is P(t) = O + tR, where P is a point along the ray.
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
