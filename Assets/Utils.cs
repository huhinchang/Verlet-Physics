using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils {
    public static Vector2 GetMousePosition() {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    // checks if f is withing a and b (inclusive, order of a and b doesn't matter)
    public static bool IsWithinInclusive (this float f, float a, float b) {
        return (f >= Mathf.Min(a,b) && f <= Mathf.Max(a, b));
    }

    // p1 p2 are for line 1, p3 p4 are for line 2
    public static bool Intersects(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4) {
        Vector2? intersection = GetIntersection(p1, p2, p3, p4);
        return (intersection?.x.IsWithinInclusive(p1.x, p2.x) ?? false) &&
        (intersection?.y.IsWithinInclusive(p1.y, p2.y) ?? false) &&
        (intersection?.x.IsWithinInclusive(p3.x, p4.x) ?? false) &&
        (intersection?.y.IsWithinInclusive(p3.y, p4.y) ?? false);
    }

    // p1 p2 are for line 1, p3 p4 are for line 2
    public static Vector2? GetIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4) {
        float m1 = FindSlope(p1, p2);
        float b1 = FindYIntercept(p1, m1);

        float m2 = FindSlope(p3, p4);
        float b2 = FindYIntercept(p3, m2);

        if (m1 == m2) {
            return null;
        } else {
            float x = (b2 - b1) / (m1 - m2);
            float y = (m1 * x) + b1;
            return new Vector2(x, y);
        }
    }

    private static float FindSlope(Vector2 p1, Vector2 p2) {
        return (p2.y - p1.y) / (p2.x - p1.x); // rise over run
    }

    private static float FindYIntercept(Vector2 p1, float m) {
        // y = mx + b
        // b = y - mx
        return p1.y - (m * p1.x);
    }
}
