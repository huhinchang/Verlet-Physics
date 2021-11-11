using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public static class Generic
    {
        public static Vector2 GetMousePosition()
        {
            return Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        // C style int to bool
        public static bool IsTrue(this int i)
        {
            return i != 0;
        }

        // C style int to bool
        public static bool IsFalse(this int i)
        {
            return i == 0;
        }
    }

    public static class Math
    {
        public enum Orientation { CW, CCW, Colinear }

        // checks if f is within a and b (inclusive, order of a and b doesn't matter)
        public static bool IsWithinInclusive(this float f, float a, float b)
        {
            return f >= Mathf.Min(a, b) && f <= Mathf.Max(a, b);
        }

        // checks if p is within the bounding box of a and b (inclusive, order of a and b doesn't matter)
        public static bool IsWithinBoundingBox(this Vector2 p, Vector2 a, Vector2 b)
        {
            return p.x.IsWithinInclusive(a.x, b.x) && p.y.IsWithinInclusive(a.y, b.y);
        }

        // determines if point p is on line segment p1 p2
        public static bool IsOnSegment(this Vector2 p, Vector2 a, Vector2 b)
        {
            return GetOrientation(p, a, b) == Orientation.Colinear && p.IsWithinBoundingBox(a, b);
        }

        // determines if p1, p2, and p3 are clockwise
        // p1 p2 are for line 1, p3 p4 are for line 2
        // implementation from https://stackoverflow.com/questions/3838329/how-can-i-check-if-two-segments-intersect
        public static Orientation GetOrientation(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            float orientation = (p3.y - p1.y) * (p2.x - p1.x) - (p2.y - p1.y) * (p3.x - p1.x);
            if (orientation < 0)
            {
                return Orientation.CCW;
            } else if (orientation > 0)
            {
                return Orientation.CW;
            } else
            {
                return Orientation.Colinear;
            }
        }

        // determines if two lines intersect
        // p1 p2 are for line 1, p3 p4 are for line 2
        // implementation from https://stackoverflow.com/questions/3838329/how-can-i-check-if-two-segments-intersect
        public static bool Intersects(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
        {
            Orientation o123 = GetOrientation(p1, p2, p3);
            Orientation o124 = GetOrientation(p1, p2, p4);
            Orientation o341 = GetOrientation(p3, p4, p1);
            Orientation o342 = GetOrientation(p3, p4, p2);

            if (o123 == Orientation.Colinear && o124 == Orientation.Colinear) // line segments are colinear
            {
                return p1.IsWithinBoundingBox(p3, p4) || p2.IsWithinBoundingBox(p3, p4); // they can be colinear but disconnected
            } else if (o123 != o124 && o341 != o342)
            {
                return true;// general case
            } else
            {
                return false; // Doesn't fall in any of the above cases
            }
        }

        public static bool IsParallel(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
        {
            return (p2.y - p1.y) * (p4.x - p3.x) == (p4.y - p3.y) * (p2.x - p1.x);
        }
    }
}
