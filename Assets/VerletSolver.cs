using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public abstract class VerletSolver : MonoBehaviour
{
    // needs to be a struct because of compute shader limitations
    [Serializable]
    public struct Point
    {
        public Vector2 Position, PrevPosition;
        public int Locked;

        public Point(Vector2 initialPosition, int locked)
        {
            Position = initialPosition;
            PrevPosition = initialPosition;
            Locked = locked;
        }

        public Point(Vector2 position, Vector2 prevPosition, int locked)
        {
            Position = position;
            PrevPosition = prevPosition;
            Locked = locked;
        }
    }

    [Serializable]
    public struct Stick
    {
        public uint A { get; private set; }
        public uint B { get; private set; }
        public float Length { get; private set; }
        public Stick(uint a, uint b, float length)
        {
            A = a;
            B = b;
            Length = length;
        }
    }

    protected readonly Vector2 _kGravity = new Vector2(0, -9.81f);
    private const float _kDebugSmallRadius = 0.1f;
    private const float _kDebugLargeRadius = 0.2f;

    [SerializeField]
    protected Color _debugColor = default;
    [SerializeField]
    protected int _constraintReps = default;

    protected List<Point> _points = new List<Point>();
    protected List<Stick> _sticks = new List<Stick>();
    private Vector2 _knifeStart, _knifeEnd;
    private int selected = -1;

    public void PlaceNode(bool locked)
    {
        _points.Add(new Point(Camera.main.ScreenToWorldPoint(Input.mousePosition), Input.GetMouseButton(1) ? 1 : 0));
        uint newPoint = (uint)_points.Count - 1;
        if (selected != -1)
        {
            _sticks.Add(new Stick((uint)selected, newPoint, Vector2.Distance(_points[selected].Position, _points[(int)newPoint].Position)));
        }
        selected = (int)newPoint;
    }

    public void SetNodeLocked(bool locked)
    {
        var closestPoint = GetPointClosestToMouse(_points, point => point.Position);
        _points[closestPoint.Item3] = new Point(closestPoint.Item1.Position, locked ? 1 : 0);
    }

    public void SetKnife(Vector2 start, Vector2 end)
    {
        _knifeStart = start;
        _knifeEnd = end;
    }

    public void Cut() {
        for (int i = _sticks.Count - 1; i >= 0; i--)
        {
            Stick s = _sticks[i];
            Vector2 aPosition = _points[(int)s.A].Position;
            Vector2 bPosition = _points[(int)s.B].Position;
            if (Utils.Math.Intersects(aPosition, bPosition, _knifeStart, _knifeEnd))
            {
                _sticks.RemoveAt(i);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = _debugColor;
        foreach (var p in _points)
        {
            Gizmos.DrawSphere(p.Position, p.Locked.IsTrue() ? _kDebugLargeRadius : _kDebugSmallRadius);
        }

        foreach (var s in _sticks)
        {
            Vector2 aPosition = _points[(int)s.A].Position;
            Vector2 bPosition = _points[(int)s.B].Position;
            Gizmos.color = Utils.Math.Intersects(aPosition, bPosition, _knifeStart, _knifeEnd) ?  Color.black : _debugColor;
            Gizmos.DrawLine(_points[(int)s.A].Position, _points[(int)s.B].Position);
        }
    }

    // ############# UTILS ##############
    private static (T, float, int) GetPointClosestToMouse<T>(List<T> points, Func<T, Vector2> positionGetter)
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
