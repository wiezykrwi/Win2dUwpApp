using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Win2dUwpApp.Models
{
    public class Hex
    {
        private readonly int _size;

        private const int _topCorner = 1;
        private const int _toprightCorner = 2;
        private const int _bottomRightCorner = 3;
        private const int _bottomCorner = 4;
        private const int _bottomrightCorner = 5;
        private const int _topLeftCorner = 6;

        public Hex(Vector2 center, int size)
        {
            _size = size;
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
            if (coordinates.X < Corners[_toprightCorner].X || coordinates.X > Corners[_topLeftCorner].X)
                return false;
            if (coordinates.Y > Corners[_bottomCorner].Y || coordinates.Y < Corners[_topCorner].Y)
                return false;


            var triangles = GetIncludedTriangles();
            return triangles.Any(t => IsPointInTriagle(coordinates, t.V1, t.V2, t.V3));
        }

        private IEnumerable<Triangle> GetIncludedTriangles()
        {
            for (var i = 0; i < Corners.Length - 1; i++) // don't include the last corner
            {
                yield return new Triangle(Center, Corners[i], Corners[i+1]);
            }
        }

        private bool IsPointInTriagle(Vector2 point, Vector2 v1, Vector2 v2, Vector2 v3)
        {
            var totalArea = CalculateTriangleArea(v1, v2, v3);
            var area1 = CalculateTriangleArea(point, v2, v3);
            var area2 = CalculateTriangleArea(point, v1, v3);
            var area3 = CalculateTriangleArea(point, v1, v2);

            return !((area1 + area2 + area3) > totalArea);
        }

        private float CalculateTriangleArea(Vector2 v1, Vector2 v2, Vector2 v3)
        {
            var det = ((v1.X - v3.X) * (v2.Y - v3.Y)) - ((v2.X - v3.X) * (v1.Y - v3.Y));
            return (det / 2.0f);
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