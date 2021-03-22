using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace RayTracer
{
    // A Ray Tracer built while following along with Peter Shirley's excellent series of books
    // 'Ray Tracing in One Weekend'. So far I have worked through the first book and added some of my own
    // modifications, including basic parallelism.
    class Program
    {
        static void Main(string[] args)
        {
            var config = Config.NewDefaultForDev();
            var stopWatch = new Stopwatch();
            var framebuffer = new Image(config.ImageWidth, config.ImageHeight);

            var scene = Scenes.SphereRing(config);
            var rayTracer = new RayTracer(scene, config.SamplesPerPixel, config.MaxDepth);

            Console.WriteLine("Rendering {0} with config: {1}", scene.Name, config); 

            using (var progressBar = new ConsoleProgressBar("Progress"))
            {
                stopWatch.Start();

                int imageW = config.ImageWidth;
                int imageH = config.ImageHeight;
                int scanlinesComplete = 0;

                // For now I'm adding parallelism using the built in 'Parallel.For'.
                // I think more performance could be gained via parallelism using a custom solution.
                // For example, I could manually manage a set of worker threads. The image would then be divided
                // into a grid of small tiles which are then added onto a work queue. Each worker thread
                // would pick up tiles off the queue as they complete them until all the work is done.
                // Each worker thread could be assigned it's own instance of Random too so we could avoid
                // the ThreadLocalRandom. But, I would need to have a more sophisticated benchmarking utility
                // in place before I go and do this. Something that aggregates many samples, and has test scenes
                // for different scenarios. For now this simple parallelism works fine.
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
