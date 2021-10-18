using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VerletSolver : MonoBehaviour {
    public abstract void PlaceNode(bool locked);
    public abstract void SetNodeLocked(bool locked);
    public abstract void SetKnife(Vector2 p1, Vector2 p2);

    protected static (T, float, int) GetPointClosestToMouse<T>(List<T> points, Func<T, Vector2> getPos) {
        var mousePos = Utils.GetMousePosition();

        T closest = points[0];
        float closestDist = float.MaxValue;
        int closestIndex = 0;

        for (int i = 0; i < points.Count; i++) {
            float dist = Vector2.Distance(getPos(points[i]), mousePos);
            if (dist < closestDist) {
                closestDist = dist;
                closest = points[i];
                closestIndex = i;
            }
        }
        return (closest, closestDist, closestIndex);
    }
}
