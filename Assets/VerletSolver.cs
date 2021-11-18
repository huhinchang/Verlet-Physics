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
    private GameObject _pointPrefab = default;
    [SerializeField]
    private GameObject _stickPrefab = default;
    [SerializeField]
    private Color _debugColor = default;
    [SerializeField]
    protected int _constraintReps = default;

    ObjectPooler<SpriteRenderer> _pointPooler;
    ObjectPooler<LineRenderer> _stickPooler;
    protected List<Point> _points = new List<Point>();
    protected List<Stick> _sticks = new List<Stick>();
    private Vector2 _knifeStart, _knifeEnd;
    private int _selected = -1;

    private void Awake()
    {
        _pointPooler = new ObjectPooler<SpriteRenderer>(_pointPrefab, parent: null, initialSize: 0, maxSize: null, ObjectPoolerMaxSizeReachedBehavior.CancelSpawn);
        _stickPooler = new ObjectPooler<LineRenderer>(_stickPrefab, parent: null, initialSize: 0, maxSize: null, ObjectPoolerMaxSizeReachedBehavior.CancelSpawn);  
    }

    private void OnEnable()
    {
        _pointPooler.OnMaxSizeReached += HandleMaxSizeReached;
        _stickPooler.OnMaxSizeReached += HandleMaxSizeReached;
    }

    private void OnDisable()
    {
        _pointPooler.OnMaxSizeReached -= HandleMaxSizeReached;
        _stickPooler.OnMaxSizeReached -= HandleMaxSizeReached;
    }

    private void HandleMaxSizeReached() {
        throw new NotImplementedException("Max Size Reached");
    }

    // Creates a point at the mouse position
    public void CreatePointAtMouse(bool locked)
    {
        _points.Add(new Point(Camera.main.ScreenToWorldPoint(Input.mousePosition), Input.GetMouseButton(1) ? 1 : 0));
        uint newPoint = (uint)_points.Count - 1;
        LinkToSelected((int)newPoint, autoSelect: true);

        var pointInstance = _pointPooler.SpawnFromPool();
        pointInstance.transform.position = _points[(int) newPoint].Position;
    }

    // sets the point closest to the mouse as selected
    public void SelectClosest()
    {
        _selected = GetPointClosestToMouse(_points, point => point.Position).Item3;
    }

    // sets the lock status of the point closest to the mouse
    public void LockClosest(bool locked)
    {
        var closestPoint = GetPointClosestToMouse(_points, point => point.Position);
        _points[closestPoint.Item3] = new Point(closestPoint.Item1.Position, locked ? 1 : 0);
    }

    // sets the start and end points of the knife
    public void SetKnife(Vector2 start, Vector2 end)
    {
        _knifeStart = start;
        _knifeEnd = end;
    }

    // Removes sticks that intersect the slice
    public void Cut()
    {
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

    // Links the point closest to the mouse with the selected point
    public void LinkClosestToSelected()
    {
        var closestPoint = GetPointClosestToMouse(_points, point => point.Position);
        LinkToSelected(closestPoint.Item3, autoSelect: false);
    }

    // Selects the point at the index
    private void SelectPoint(int index)
    {
        if (index >= _points.Count)
        {
            Debug.LogError($"index was out of bounds");
            return;
        }
        _selected = index;
    }

    // Links the given point with the selected point
    private void LinkToSelected(int index, bool autoSelect)
    {
        if (index >= _points.Count)
        {
            Debug.LogError($"index was out of bounds");
            return;
        }

        if (_selected >= 0)
        {
            LinkNodes(_selected, index);
        }

        if (autoSelect)
        {
            SelectPoint(index);
        }
    }

    // Links 2 nodes
    private void LinkNodes(int a, int b)
    {
        if (a >= _points.Count)
        {
            Debug.LogError($"a was out of bounds");
            return;
        }

        if (b >= _points.Count)
        {
            Debug.LogError($"b was out of bounds");
            return;
        }

        foreach (var stick in _sticks)
        {
            if ((stick.A == a && stick.B == b) || (stick.B == a && stick.A == b))
            {
                return; // stick linking same points already exists
            }
        }
        _sticks.Add(new Stick((uint)a, (uint)b, Vector2.Distance(_points[a].Position, _points[b].Position)));
        var stickInstance = _stickPooler.SpawnFromPool();
        stickInstance.SetPosition(0, _points[a].Position);
        stickInstance.SetPosition(1, _points[b].Position);
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
            Gizmos.color = Utils.Math.Intersects(aPosition, bPosition, _knifeStart, _knifeEnd) ? Color.black : _debugColor;
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
