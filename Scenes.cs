using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

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
        public static Scene RandomSpheres(Config config)
        {
            var camera = new Camera(
                new Vector3(13.0f, 2.0f, 3.0f),
                new Vector3(0.0f, 0.0f, 0.0f),
                Vector3.UnitY, 
                20.0f, 
                config.ImageAspectRatio,
                0.1f, 
                10.0f
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
                            objects.Add(new Sphere(center, 0.2f, material));
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

            var material3 = new Metal(new Vector3(0.4f, 0.2f, 0.1f), 0.0f);
            objects.Add(new Sphere(new Vector3(4.0f, 1.0f, 0.0f), 1.0f, material3));

            return scene;
        }
    }
}
