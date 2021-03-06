using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
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
    private HeldButton _solveButton = default;
    [SerializeField]
    private GameObject _pointWidgetPrefab = default;
    [SerializeField]
    private GameObject _stickWidgetPrefab = default;
    [SerializeField]
    private Color _themeColor = default;
    [SerializeField]
    private Color _cutPreviewColor = default;
    [SerializeField]
    private Slider _constraintRepsSlider = default;
    [SerializeField]
    protected int _constraintReps = default;

    private ObjectPooler<PointWidget> _pointWidgetPooler;
    private ObjectPooler<StickWidget> _stickWidgetPooler;
    private Vector2 _knifeStart, _knifeEnd;
    private bool _knifeActive = false;
    private bool _solveButtonPressed = false;

    protected List<Point> _points = new List<Point>();
    protected List<Stick> _sticks = new List<Stick>();
    protected int _selected = -1;

    private void OnValidate()
    {
        Assert.IsNotNull(_solveButton);
        Assert.IsNotNull(_pointWidgetPrefab);
        Assert.IsNotNull(_stickWidgetPrefab);
        Assert.IsNotNull(_constraintRepsSlider);
    }

    protected virtual void Awake()
    {
        _pointWidgetPooler = new ObjectPooler<PointWidget>(_pointWidgetPrefab, parent: transform);
        _stickWidgetPooler = new ObjectPooler<StickWidget>(_stickWidgetPrefab, parent: transform);

        _constraintRepsSlider.value = _constraintReps;
        _constraintRepsSlider.onValueChanged.AddListener((value) => _constraintReps = (int)value);

        _solveButton.OnPressChanged += HandleSolveButtonPressChanged;
    }

    private void OnDestroy()
    {
        _constraintRepsSlider.onValueChanged.RemoveAllListeners();
        _solveButton.OnPressChanged -= HandleSolveButtonPressChanged;
    }

    private void HandleMaxSizeReached()
    {
        throw new NotImplementedException("Max Size Reached");
    }

    private void HandleSolveButtonPressChanged(bool value)
    {
        _solveButtonPressed = value;
    }

    protected virtual void Update()
    {
        if (_solveButtonPressed && _points.Count > 0)
        {
            Solve();
        }
    }

    protected virtual void Solve()
    {
        UpdatePointWidgets();
        UpdateStickWidgets();
    }

    protected void UpdatePointWidgets()
    {
        var widgets = _pointWidgetPooler.ActivePool;
        if (widgets.Count != _points.Count)
        {
            Debug.LogError($"point widgets count ({widgets.Count}) was different from actual point count ({_points.Count})!");
        }
        for (int i = 0; i < _points.Count; i++)
        {
            var point = _points[i];
            widgets[i].UpdateState(point.Position, point.Locked.IsTrue(), _selected == i);
        }
    }

    protected void UpdateStickWidgets()
    {
        var widgets = _stickWidgetPooler.ActivePool;
        if (widgets.Count != _sticks.Count)
        {
            Debug.LogError($"stick widgets count ({widgets.Count}) was different from actual stick count ({_sticks.Count})!");
        }
        for (int i = 0; i < _sticks.Count; i++)
        {
            var stick = _sticks[i];
            var color = _knifeActive && Utils.Math.Intersects(_points[stick.A].Position, _points[stick.B].Position, _knifeStart, _knifeEnd) ? _cutPreviewColor : _themeColor;
            widgets[i].UpdateState(_points[stick.A].Position, _points[stick.B].Position, color);
        }
    }

    // returns if a new point was indeed created
    protected bool CreatePoint(Vector2 pos, bool locked)
    {
        for (int i = 0; i < _points.Count; i++)
        {
            if (_points[i].Position == pos)
            {
                Debug.Log("Point with same position found");
                if (locked != _points[i].Locked.IsTrue())
                {
                    Point p = _points[i];
                    p.Locked = locked ? 1 : 0;
                    _points[i] = p;
                }
                return false;
            }
        }

        _points.Add(new Point(pos, locked ? 1 : 0));
        _pointWidgetPooler.SpawnFromPool().Initialize(_themeColor, gameObject.layer);
        return true;
    }

    protected void ErasePoint(int indexToRemove)
    {
        _points.RemoveAt(indexToRemove);
        _pointWidgetPooler.ReturnToPoolAt(0);

        // special logic for sticks
        for (int i = _sticks.Count - 1; i >= 0; i--)
        {
            Stick s = _sticks[i];
            if (s.A == indexToRemove || s.B == indexToRemove)
            {
                // stick contains deleted point
                _sticks.RemoveAt(i);
                _stickWidgetPooler.ReturnToPoolAt(0);
            } else
            {
                // weird stuff because we're working with structs not classes
                int aIndexShift = s.A > indexToRemove ? -1 : 0;
                int bIndexShift = s.B > indexToRemove ? -1 : 0;

                _sticks[i] = new Stick(s.A + aIndexShift, s.B + bIndexShift, s.Length);
            }
        }

        // shift selected point
        if (_selected > indexToRemove)
        {
            --_selected;
        } else if (_selected == indexToRemove)
        {
            _selected = -1;
        }

        UpdatePointWidgets();
        UpdateStickWidgets();
    }

    // sets the start and end points of the knife
    public void SetKnife(Vector2 start, Vector2 end)
    {
        _knifeStart = start;
        _knifeEnd = end;
        UpdateStickWidgets();
    }

    // sets the start and end points of the knife
    public void SetKnifeActive(bool active)
    {
        _knifeActive = active;
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

    // Selects the point at the index
    protected void SelectPoint(int index)
    {
        if (index >= _points.Count)
        {
            Debug.LogError($"index was out of bounds");
            return;
        }
        _selected = index;
        UpdatePointWidgets();
    }

    // Links the given point with the selected point
    protected void LinkToSelected(int index)
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
    }

    // Links 2 points
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

        _stickWidgetPooler.SpawnFromPool().Initialize(_themeColor, gameObject.layer);
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
}
