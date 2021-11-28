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
        public int A { get; private set; }
        public int B { get; private set; }
        public float Length { get; private set; }
        public Stick(int a, int b, float length)
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
    private GameObject _pointWidgetPrefab = default;
    [SerializeField]
    private GameObject _stickWidgetPrefab = default;
    [SerializeField]
    private Color _themeColor = default;
    [SerializeField]
    private Color _cutPreviewColor = default;
    [SerializeField]
    protected int _constraintReps = default;

    ObjectPooler<PointWidget> _pointWidgetPooler;
    ObjectPooler<StickWidget> _stickWidgetPooler;
    protected List<Point> _points = new List<Point>();
    protected List<Stick> _sticks = new List<Stick>();
    private Vector2 _knifeStart, _knifeEnd;
    private int _selected = -1;

    private void Awake()
    {
        _pointWidgetPooler = new ObjectPooler<PointWidget>(_pointWidgetPrefab, parent: transform);
        _stickWidgetPooler = new ObjectPooler<StickWidget>(_stickWidgetPrefab, parent: transform);
    }

    private void HandleMaxSizeReached()
    {
        throw new NotImplementedException("Max Size Reached");
    }

    public virtual void Solve()
    {
        UpdatePointWidgets();
        UpdateStickWidgets();
    }

    private void UpdatePointWidgets()
    {
        var widgets = _pointWidgetPooler.ActivePool;
        if (widgets.Count != _points.Count)
        {
            Debug.LogError("point widgets count was different from actual stick count!");
        }
        for (int i = 0; i < _points.Count; i++)
        {
            var point = _points[i];
            widgets[i].UpdateState(point.Position, point.Locked.IsTrue());
        }
    }

    private void UpdateStickWidgets()
    {
        var widgets = _stickWidgetPooler.ActivePool;
        if (widgets.Count != _sticks.Count)
        {
            Debug.LogError("stick widgets count was different from actual stick count!");
        }
        for (int i = 0; i < _sticks.Count; i++)
        {
            var stick = _sticks[i];
            widgets[i].UpdateState(_points[stick.A].Position,
            _points[stick.B].Position,
            Utils.Math.Intersects(_points[stick.A].Position, _points[stick.B].Position, _knifeStart, _knifeEnd));
        }
    }

    // Creates a point at the mouse position
    public void CreatePointAtMouse(bool locked)
    {
        _points.Add(new Point(Camera.main.ScreenToWorldPoint(Input.mousePosition), Input.GetMouseButton(1) ? 1 : 0));
        int newPointIndex = _points.Count - 1;
        LinkToSelected(newPointIndex, autoSelect: true);

        _pointWidgetPooler.SpawnFromPool().Initialize(_themeColor, gameObject.layer);
        UpdatePointWidgets();
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
        UpdatePointWidgets();
    }

    // sets the start and end points of the knife
    public void SetKnife(Vector2 start, Vector2 end)
    {
        _knifeStart = start;
        _knifeEnd = end;
        UpdateStickWidgets();
    }

    // Removes sticks that intersect the slice
    public void Cut()
    {
        for (int i = _sticks.Count - 1; i >= 0; i--)
        {
            Stick s = _sticks[i];
            Vector2 aPosition = _points[s.A].Position;
            Vector2 bPosition = _points[s.B].Position;
            if (Utils.Math.Intersects(aPosition, bPosition, _knifeStart, _knifeEnd))
            {
                _sticks.RemoveAt(i);
                _stickWidgetPooler.ReturnToPoolAt(0);
            }
        }
        UpdateStickWidgets();
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
            LinkPoints(_selected, index);
        }

        if (autoSelect)
        {
            SelectPoint(index);
        }
    }

    // Links 2 nodes
    private void LinkPoints(int a, int b)
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
        _sticks.Add(new Stick(a, b, Vector2.Distance(_points[a].Position, _points[b].Position)));

        _stickWidgetPooler.SpawnFromPool().Initialize(_themeColor, _cutPreviewColor, gameObject.layer);
        UpdateStickWidgets();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = _themeColor;
        foreach (var p in _points)
        {
            Gizmos.DrawSphere(p.Position, p.Locked.IsTrue() ? _kDebugLargeRadius : _kDebugSmallRadius);
        }

        foreach (var s in _sticks)
        {
            Vector2 aPosition = _points[s.A].Position;
            Vector2 bPosition = _points[s.B].Position;
            Gizmos.color = Utils.Math.Intersects(aPosition, bPosition, _knifeStart, _knifeEnd) ? Color.black : _themeColor;
            Gizmos.DrawLine(_points[s.A].Position, _points[s.B].Position);
        }
    }

    // ############# UTILS ##############
    private (T, float, int) GetPointClosestToMouse<T>(List<T> points, Func<T, Vector2> positionGetter)
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
