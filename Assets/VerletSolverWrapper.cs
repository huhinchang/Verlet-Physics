using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VerletSolverWrapper : VerletSolver
{
    // Creates a point at the mouse position
    public void CreatePointAtMouse(bool locked)
    {
        if (!enabled) return;

        CreatePoint(Camera.main.ScreenToWorldPoint(Input.mousePosition), Input.GetMouseButton(1));

        int newPointIndex = _points.Count - 1;
        LinkToSelected(newPointIndex);
        SelectPoint(newPointIndex);

        UpdatePointWidgets();
    }

    // sets the point closest to the mouse as selected
    public void SelectClosest()
    {
        if (!enabled) return;
        SelectPoint(GetPointClosestToMouse(_points, point => point.Position).Item3);
    }

    // sets the lock status of the point closest to the mouse
    public void LockClosest(bool locked)
    {
        if (!enabled) return;
        var closestPoint = GetPointClosestToMouse(_points, point => point.Position);
        _points[closestPoint.Item3] = new Point(closestPoint.Item1.Position, locked ? 1 : 0);
        UpdatePointWidgets();
    }

    // Links the point closest to the mouse with the selected point
    public void LinkClosestToSelected()
    {
        if (!enabled) return;
        var closestPoint = GetPointClosestToMouse(_points, point => point.Position);
        LinkToSelected(closestPoint.Item3);
    }

    // ############# UTILS ##############
    protected (T, float, int) GetPointClosestToMouse<T>(List<T> points, Func<T, Vector2> positionGetter)
    {
        var mousePos = Utils.Generic.GetMousePosition();

        T closest = points[0];
        float closestDist = float.MaxValue;
        int closestIndex = 0;

        for (int i = 0; i < points.Count; i++)
        {
            float dist = Vector2.Distance(positionGetter(points[i]), mousePos);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = points[i];
                closestIndex = i;
            }
        }
        return (closest, closestDist, closestIndex);
    }
}
