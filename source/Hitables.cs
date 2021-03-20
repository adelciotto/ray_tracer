using System;
using System.Numerics;

namespace RayTracer
{
    struct HitRecord
    {
        public Vector3 Point { get; set; }
        public Vector3 Normal { get; set; }
        public float T { get; set; }
        public bool FrontFace { get; set; }
        public IMaterial Material { get; set; }

        public void SetFaceNormal(Ray ray, Vector3 outwardNormal)
        {
            FrontFace = Vector3.Dot(ray.Direction, outwardNormal) < 0.0f;
            Normal = FrontFace ? outwardNormal : -outwardNormal;
        }
    }

    interface IHitable
    {
        bool Hit(Ray ray, float tMin, float tMax, ref HitRecord hitRecord);
    }

    class Sphere : IHitable
    {
        public Vector3 Center { get; }
        public float Radius { get;  }

        private IMaterial _material;

        public Sphere(Vector3 center, float radius, IMaterial material)
        {
            Center = center;
            Radius = radius;
            _material = material;
        }

        public bool Hit(Ray ray, float tMin, float tMax, ref HitRecord hitRecord)
        {
            Vector3 oc = ray.Origin - Center;
            var a = ray.Direction.LengthSquared();
            var halfB = Vector3.Dot(oc, ray.Direction);
            var c = oc.LengthSquared() - Radius * Radius;

            float discriminant = halfB * halfB - a * c;
            if (discriminant < 0.0f)
                return false;

            // Find the nearest root that lies in the acceptable range.
            float sqrtD = MathF.Sqrt(discriminant);
            float root = (-halfB - sqrtD) / a;
            if (root < tMin || root > tMax)
            {
                root = (-halfB + sqrtD) / a;
                if (root < tMin || root > tMax)
                    return false;
            }

            hitRecord.T = root;
            hitRecord.Point = ray.At(hitRecord.T);
            var outwardNormal = (hitRecord.Point - Center) / Radius;
            hitRecord.SetFaceNormal(ray, outwardNormal);
            hitRecord.Material = _material;

            return true;
        }
    }
}
