using System.Collections.Generic;
using PhotoVs.Utils.Extensions;

namespace PhotoVs.Utils.Collections
{
    public class LinearTweener<T>
    {
        private readonly List<(float, T)> _points;

        public LinearTweener()
        {
            _points = new List<(float, T)>();
        }

        public void AddPoint(float point, T item)
        {
            _points.Add((point, item));
            _points.Sort((a, b) => a.Item1.CompareTo(b.Item1));
        }

        public (float, T, T) GetPhase(float phase)
        {
            // first, find which points the phase is between
            var leftIndex = _points.Count - 1;
            var rightIndex = 0;
            for (var i = 0; i < _points.Count - 1; i++)
            {
                var point = _points[i];
                var nextPoint = _points[i + 1];
                if (phase >= point.Item1 && phase < nextPoint.Item1)
                {
                    leftIndex = i;
                    rightIndex = i + 1;
                    break;
                }
            }

            var left = _points[leftIndex];
            var right = _points[rightIndex];

            if (rightIndex == 0)
            {
                if (phase <= right.Item1) phase += 1f;

                var value = phase.Map(left.Item1, 1f + right.Item1, 0f, 1f);
                return (value, right.Item2, left.Item2);
            }
            else
            {
                var value = phase.Map(left.Item1, right.Item1, 0f, 1f);
                return (value, right.Item2, left.Item2);
            }
        }
    }
}