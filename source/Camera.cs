using System;
using System.Numerics;

namespace RayTracer
{
    class Camera
    {
        private Vector3 _origin;
        private Vector3 _w, _u, _v;
        private Vector3 _horizontal;
        private Vector3 _vertical;
        private Vector3 _lowerLeftCorner;
        private float _lensRadius;
        private float _shutterOpenTime;
        private float _shutterCloseTime;

        public Camera(Vector3 lookFrom, Vector3 lookAt, Vector3 vUp, 
            float verticalFov, float aspectRatio, float aperture, float focusDist,
            float shutterOpenTime = 0.0f, float shutterCloseTime = 0.0f)
        {
            float theta = MathExt.ToRadians(verticalFov);
            float h = MathF.Tan(theta / 2.0f);
            float viewportHeight = 2.0f * h;
            float viewportWidth = aspectRatio * viewportHeight;

            _w = Vector3.Normalize(lookFrom - lookAt);
            _u = Vector3.Normalize(Vector3.Cross(vUp, _w));
            _v = Vector3.Cross(_w, _u);

            _origin = lookFrom;
            _horizontal = focusDist * viewportWidth * _u;
            _vertical = focusDist * viewportHeight * _v;
            _lowerLeftCorner = _origin - _horizontal / 2.0f - _vertical / 2.0f - focusDist*_w;

            _lensRadius = aperture / 2.0f;

            _shutterOpenTime = shutterOpenTime;
            _shutterCloseTime = shutterCloseTime;
        }

        public Ray GetRay(float s, float t)
        {
            var rd = _lensRadius * MathExt.RandomInUnitDisk();
            var offset = _u * rd.X + _v * rd.Y;
            return new Ray(
                _origin + offset,
                _lowerLeftCorner + s * _horizontal + t * _vertical - _origin - offset,
                MathExt.RandomFloat(_shutterOpenTime, _shutterCloseTime));
        }
    }
}
