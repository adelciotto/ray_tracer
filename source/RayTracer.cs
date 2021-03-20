using System.Numerics;

namespace RayTracer
{
    class RayTracer
    {
        private Camera _camera;
        private Scene _scene;
        private readonly int _samplesPerPixel;
        private readonly int _maxDepth;
        private readonly float _invSamplesPerPixel;

        public RayTracer(Scene scene, int samplesPerPixel, int maxDepth)
        {
            _scene = scene;
            _camera = _scene.Camera;
            _samplesPerPixel = samplesPerPixel;
            _maxDepth = maxDepth;

            _invSamplesPerPixel = 1.0f / _samplesPerPixel;
        }

        public Vector3 Trace(int imageX, int imageY, int imageW, int imageH)
        {
            var color = Vector3.Zero;

            for (int s = 0; s < _samplesPerPixel; s++)
            {
                float u = (imageX + MathExt.RandomFloat()) / (imageW - 1);
                float v = (imageY + MathExt.RandomFloat()) / (imageH - 1);

                var ray = _camera.GetRay(u, v);
                color += RayColor(ray, _maxDepth);
            }

            color *= _invSamplesPerPixel;
            return Vector3.SquareRoot(color);
        }

        private Vector3 RayColor(Ray ray, int depth)
        {
            var hitRecord = new HitRecord();

            if (depth <= 0)
                return Vector3.Zero;

            if (_scene.HitableList.Hit(ray, 0.001f, float.PositiveInfinity, ref hitRecord))
            {
                var scattered = new Ray();
                var attentuation = Vector3.Zero;
                if (hitRecord.Material.Scatter(ray, hitRecord, ref attentuation, ref scattered))
                {
                    return attentuation * RayColor(scattered, depth - 1);
                }

                return Vector3.Zero;
            }

            var unitDirection = Vector3.Normalize(ray.Direction);
            float t = 0.5f * (unitDirection.Y + 1.0f);
            return Vector3.Lerp(Vector3.One, new Vector3(0.05f, 0.13f, 1.0f), t);
        }
    }
}
