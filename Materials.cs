using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RayTracer
{
    interface IMaterial
    {
        bool Scatter(Ray ray, HitRecord hitRecord, ref Vector3 attentuation, ref Ray scattered);
    }

    class Lambertian : IMaterial
    {
        public Vector3 Albedo { get; }

        public Lambertian(Vector3 albedo)
        {
            Albedo = albedo;
        }

        public bool Scatter(Ray ray, HitRecord hitRecord, ref Vector3 attentuation, ref Ray scattered)
        {
            var scatterDir = hitRecord.Normal + MathExt.RandomUnitVector();
            if (Vector3Ext.NearZero(scatterDir))
                scatterDir = hitRecord.Normal;

            scattered = new Ray(hitRecord.Point, scatterDir);
            attentuation = Albedo;
            return true;
        }
    }

    class Metal : IMaterial
    {
        public Vector3 Albedo { get; }
        public float Fuzz { get; }

        public Metal(Vector3 albedo, float fuzz)
        {
            Albedo = albedo;
            Fuzz = fuzz;
        }

        public bool Scatter(Ray ray, HitRecord hitRecord, ref Vector3 attentuation, ref Ray scattered)
        {
            var reflected = Vector3.Reflect(Vector3.Normalize(ray.Direction), hitRecord.Normal);
            scattered = new Ray(hitRecord.Point, reflected + MathExt.RandomInUnitSphere()*Fuzz);
            attentuation = Albedo;
            return Vector3.Dot(scattered.Direction, hitRecord.Normal) > 0.0f;
        }
    }

    class Dielectric : IMaterial
    {
        public float RefactionIndex { get; }

        public Dielectric(float refractionIndex)
        {
            RefactionIndex = refractionIndex;
        }

        public bool Scatter(Ray ray, HitRecord hitRecord, ref Vector3 attentuation, ref Ray scattered)
        {
            attentuation = Vector3.One;
            float refractionRatio = hitRecord.FrontFace ? (1.0f / RefactionIndex) : RefactionIndex;

            var unitDir = Vector3.Normalize(ray.Direction);
            float cosTheta = Math.Min(Vector3.Dot(-unitDir, hitRecord.Normal), 1.0f);
            float sinTheta = MathF.Sqrt(1.0f - cosTheta * cosTheta);

            bool cannotRefract = refractionRatio * sinTheta > 1.0f;
            Vector3 direction;
            if (cannotRefract || Reflectance(cosTheta, refractionRatio) > MathExt.RandomFloat())
                direction = Vector3.Reflect(unitDir, hitRecord.Normal);
            else
                direction = Vector3Ext.Refract(unitDir, hitRecord.Normal, refractionRatio);

            scattered = new Ray(hitRecord.Point, direction);
            return true;
        }

        private static float Reflectance(float cosine, float refractionIndex)
        {
            // Use Schlick's approximation for reflectance.
            float r0 = (1.0f - refractionIndex) / (1.0f + refractionIndex);
            r0 = r0 * r0;
            return r0 + (1.0f - r0) * MathF.Pow((1.0f - cosine), 5.0f);
        }
    }
}
