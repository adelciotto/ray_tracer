using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace RayTracer
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = Config.NewDefaultForDev();
            var stopWatch = new Stopwatch();
            var framebuffer = new Image(config.ImageWidth, config.ImageHeight);

            var scene = Scenes.RandomSpheres(config);
            var rayTracer = new RayTracer(scene, config.SamplesPerPixel, config.MaxDepth);

            Console.WriteLine("Rendering image with config: {0}", config); 

            using (var progressBar = new ConsoleProgressBar("Progress"))
            {
                stopWatch.Start();

                int imageW = config.ImageWidth;
                int imageH = config.ImageHeight;
                int scanlinesComplete = 0;

                Parallel.For(0, imageH, j =>
                {
                    for (int i = 0; i < imageW; i++)
                    {
                        var color = rayTracer.Trace(i, j, imageW, imageH);
                        framebuffer.SetPixel(new Pixel(color), i, j);
                    }

                    int progress = Interlocked.Increment(ref scanlinesComplete);
                    progressBar.Report((float)progress / (imageH - 1));
                });

                stopWatch.Stop();
            }

            Console.WriteLine("Render completed in {0}", stopWatch.Elapsed);

            string outputPath = string.Format("{0}_{1}x{2}_{3}.tga",
                scene.Name, config.ImageWidth, config.ImageHeight, config.SamplesPerPixel);
            framebuffer.WriteToTGA(outputPath);

            Console.WriteLine("Render saved to {0}", outputPath);
        }
    }
}
