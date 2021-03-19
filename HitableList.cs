using System;
using System.Collections.Generic;
using System.Text;

namespace RayTracer
{
    class HitableList : IHitable
    {
        private List<IHitable> _hitables;

        public HitableList()
        {
            _hitables = new List<IHitable>();
        }

        public HitableList(params IHitable[] hitables) : this()
        {
            foreach (var hitable in hitables)
            {
                Add(hitable);
            }
        }

        public void Add(IHitable hitable)
        {
            _hitables.Add(hitable);
        }

        public void Clear()
        {
            _hitables.Clear();
        }

        public bool Hit(Ray ray, float tMin, float tMax, ref HitRecord hitRecord)
        {
            var tempHitRecord = new HitRecord();
            bool hitAnything = false;
            float closestSoFar = tMax;

            foreach (var hitable in _hitables)
            {
                if (hitable.Hit(ray, tMin, closestSoFar, ref tempHitRecord))
                {
                    hitAnything = true;
                    closestSoFar = tempHitRecord.T;
                    hitRecord = tempHitRecord;
                }
            }

            return hitAnything;
        }
    }
}
