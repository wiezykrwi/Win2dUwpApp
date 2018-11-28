using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Win2dUwpApp.Models
{
    public class Hex
    {
        public Hex(Vector2 center, int size)
        {
            Center = center;
            Corners = new Vector2[6];

            for (int i = 0; i < 6; i++)
            {
                Corners[i] = GetCorner(center, size, i);
            }
        }

        public Vector2 Center { get; }
        public Vector2[] Corners { get; }

        public string Tag { get; set; }
        public bool IsSelected { get; set; }

        private Vector2 GetCorner(Vector2 center, int size, int i)
        {
            var angleDeg = 60 * i - 30;
            var angleRad = Math.PI / 180 * angleDeg;

            return new Vector2(
                (float) (center.X + size * Math.Cos(angleRad)),
                (float) (center.Y + size * Math.Sin(angleRad)));
        }

        public bool Contains(Vector2 coordinates)
        {
            // the easy ones
            if (coordinates.X > Corners.Max(c => c.X) || coordinates.X < Corners.Min(c => c.X))
                return false;
            if (coordinates.Y < Corners.Min(c => c.Y) || coordinates.Y > Corners.Max(c => c.Y))
                return false;


            var triangles = GetIncludedTriangles();
            return triangles.Any(t => IsPointInTriagle(coordinates, t.V1, t.V2, t.V3));
        }

        private IEnumerable<Triangle> GetIncludedTriangles()
        {
            for (var i = 0; i < Corners.Length; i++) // don't include the last corner
            {
                var j = i + 1;
                if (j == Corners.Length)
                    j = 0;

                yield return new Triangle(Center, Corners[i], Corners[j]);
            }
        }

        private bool IsPointInTriagle(Vector2 point, Vector2 v1, Vector2 v2, Vector2 v3)
        {
            double s1 = v3.Y - v1.Y;
            double s2 = v3.X - v1.X;
            double s3 = v2.Y - v1.Y;
            double s4 = point.Y - v1.Y;

            var w1 = (v1.X * s1 + s4 * s2 - point.X * s1) / (s3 * s2 - (v2.X - v1.X) * s1);
            var w2 = (s4 - w1 * s3) / s1;

            var result = w1 >= 0 && w2 >= 0 && (w1 + w2) <= 1;
            return result;
        }

        private class Triangle
        {
            public Vector2 V1 { get; }
            public Vector2 V2 { get; }
            public Vector2 V3 { get; }

            public Triangle(Vector2 v1, Vector2 v2, Vector2 v3)
            {
                V1 = v1;
                V2 = v2;
                V3 = v3;
            }
        }
    }
}