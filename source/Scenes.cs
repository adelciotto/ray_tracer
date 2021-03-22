using System;
using System.Numerics;

namespace RayTracer
{
    struct Scene
    {
        public string Name { get; }
        public Camera Camera { get; }
        public HitableList HitableList { get; }

        public Scene(string name, Camera camera)
        {
            Name = name;
            Camera = camera;
            HitableList = new HitableList();
        }
    }

    static class Scenes
    {
        // Create a version of the scene from the end of the first book in the 'Ray Tracing in One Weekend' series.
        public static Scene RandomSpheres(Config config)
        {
            var camera = new Camera(
                new Vector3(13.0f, 2.0f, 3.0f),
                new Vector3(0.0f, 0.0f, 0.0f),
                Vector3.UnitY, 
                20.0f, 
                config.ImageAspectRatio,
                0.1f, 
                10.0f,
                0.0f,
                1.0f
            );
            var scene = new Scene("random_spheres", camera);
            var objects = scene.HitableList;

            var groundMaterial = new Lambertian(new Vector3(0.5f, 0.5f, 0.5f));
            objects.Add(new Sphere(new Vector3(0.0f, -1000.0f, 0.0f), 1000.0f, groundMaterial));

            for (int a = -11; a < 11; a++)
            {
                for (int b = -11; b < 11; b++)
                {
                    var chooseMat = MathExt.RandomFloat();
                    var center = new Vector3(a + 0.9f * MathExt.RandomFloat(), 0.2f,
                        b + 0.9f * MathExt.RandomFloat());

                    IMaterial material;

                    if ((center - new Vector3(4.0f, 0.2f, 0.0f)).Length() > 0.9f)
                    {
                        if (chooseMat < 0.8f)
                        {
                            // Diffuse
                            var albedo = MathExt.RandomVector3() * MathExt.RandomVector3();
                            material = new Lambertian(albedo);
                            var center2 = center + new Vector3(0, MathExt.RandomFloat(0.0f, 0.5f), 0.0f);
                            objects.Add(new MovingSphere(center, center2, 0.0f, 1.0f, 0.2f, material));
                        }
                        else if (chooseMat < 0.95f)
                        {
                            // Metal
                            var albedo = MathExt.RandomVector3(0.5f, 1.0f);
                            float fuzz = MathExt.RandomFloat(0.0f, 0.5f);
                            material = new Metal(albedo, fuzz);
                            objects .Add(new Sphere(center, 0.2f, material));
                        } else
                        {
                            // Glass
                            material = new Dielectric(1.5f);
                            objects.Add(new Sphere(center, 0.2f, material));
                        }
                    }
                }
            }

            var material1 = new Dielectric(1.5f);
            objects.Add(new Sphere(new Vector3(0.0f, 1.0f, 0.0f), 1.0f, material1));

            var material2 = new Lambertian(new Vector3(0.4f, 0.2f, 0.1f));
            objects.Add(new Sphere(new Vector3(-4.0f, 1.0f, 0.0f), 1.0f, material2));

            var material3 = new Metal(new Vector3(0.4f, 0.2f, 0.1f), 0.0f  );
            objects.Add(new Sphere(new Vector3(4.0f, 1.0f, 0.0f), 1.0f, material3));

            return scene;
        }

        public static Scene SphereRing(Config config)
        {
            var center = new Vector3(0.0f, 2.0f, 0.0f);
            var camera = new Camera(
                new Vector3(-12.0f, 2.0f, 8.0f),
                center,
                Vector3.UnitY, 
                30.0f, 
                config.ImageAspectRatio,
                0.0f, 
                10.0f,
                0.0f,
                1.0f
            );
            var scene = new Scene("sphere_ring", camera);
            var objects = scene.HitableList;

            var groundMaterial = new Lambertian(new Vector3(0.4f, 0.4f, 0.4f));
            objects.Add(new Sphere(new Vector3(0.0f, -1000.0f, 0.0f), 1000.0f, groundMaterial));

            // Center metal sphere.
            var centerSphereMat = new Metal(new Vector3(0.5f, 0.4f, 0.4f), 0.1f);
            float centerSphereRad = 2.0f;
            objects.Add(new Sphere(center, centerSphereRad, centerSphereMat));

            // Ring of spheres.
            int numRingSpheres = 10;
            float ringSphereRad = 0.5f;
            float endPosOffset = 0.3f;
            float yPos = centerSphereRad * 0.5f;
            float theta = (MathF.PI*2.0f) / numRingSpheres;
            for (int i = 0; i < numRingSpheres; i++)
            {
                float angle = (theta * i);
                float cos = MathF.Cos(angle);
                float sin = MathF.Sin(angle);
                float radius = centerSphereRad + 0.1f;

                float xPrime = radius*cos - radius*sin;
                float zPrime = radius*sin + radius*cos;

                IMaterial mat;
                if (i % 3 == 0)
                {
                    var albedo = MathExt.RandomVector3(0.5f, 1.0f);
                    float fuzz = MathExt.RandomFloat(0.0f, 0.5f);
                    mat = new Metal(albedo, fuzz);
                } 
                else if (i % 4 == 0)
                {
                    mat = new Dielectric(1.5f);
                }
                else
                {
                    var albedo = MathExt.RandomVector3() * MathExt.RandomVector3();
                    mat = new Lambertian(albedo);
                }

                var startPos = new Vector3(xPrime, yPos + 1.0f, zPrime);
                var endPos = new Vector3(xPrime + endPosOffset, yPos + 1.0f,
                    zPrime + endPosOffset);
                objects.Add(new MovingSphere(startPos, endPos, 0.0f, 1.0f, ringSphereRad, mat));
            }

            // Background spheres.
            float bgSphereRad = 0.5f;
            objects.Add(
                new Sphere(new Vector3(center.X + 1.0f, bgSphereRad, center.Z - 8.5f), bgSphereRad,
                    new Lambertian(MathExt.RandomVector3() * MathExt.RandomVector3())),
                new Sphere(new Vector3(center.X - 3.0f, bgSphereRad, center.Z - 6.0f), bgSphereRad,
                    new Lambertian(MathExt.RandomVector3() * MathExt.RandomVector3()))
            );
  
            return scene;
        }
    }
}
