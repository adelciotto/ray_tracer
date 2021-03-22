namespace RayTracer
{
    struct Config
    {
        private const int _defaultImageWidth = 640;
        private const float _defaultImageAspectRatio = 16.0f / 9.0f;
        private const int _defaultSamplesPerPixel = 500;
        private const int _defaultMaxDepth = 50;

        public int ImageWidth { get; }
        public float ImageAspectRatio { get; }
        public int ImageHeight { get; }
        public int SamplesPerPixel { get; set; }
        public int MaxDepth { get; }

        public static Config NewDefault()
        {
            return new Config(_defaultImageWidth, _defaultImageAspectRatio, 
                _defaultSamplesPerPixel, _defaultMaxDepth);
        }

        public static Config NewDefaultForDev()
        {
            return new Config(_defaultImageWidth, _defaultImageAspectRatio, 
                100, _defaultMaxDepth);
        }

        public static Config New1080p()
        {
            return new Config(1920, _defaultImageAspectRatio, _defaultSamplesPerPixel,
                _defaultMaxDepth);
        }

        public Config(int imageW, float aspectRatio, int spp, int maxDepth)
        {
            ImageWidth = imageW;
            ImageAspectRatio = aspectRatio;
            ImageHeight = (int)(ImageWidth / aspectRatio);
            SamplesPerPixel = spp;
            MaxDepth = maxDepth;
        }

        public override string ToString()
        {
            return string.Format("image:{0}x{1}, samples:{2}, max_depth:{3}",
                ImageWidth, ImageHeight, SamplesPerPixel, MaxDepth);
        }
    }
}
