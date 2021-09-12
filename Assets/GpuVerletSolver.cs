using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GpuVerletSolver : MonoBehaviour {
    public struct Point {
        public Vector2 Position, PrevPosition;
        public int Locked;
        public Point(Vector2 initialPosition, int l) {
            Position = initialPosition;
            PrevPosition = initialPosition;
            Locked = l;
        }
    }

    public struct Stick {
        public uint A { get; private set; }
        public uint B { get; private set; }
        public float Length { get; private set; }
        public Stick(uint a, uint b, float l) {
            A = a;
            B = b;
            Length = l;
        }
    }

    [SerializeField]
    private ComputeShader _verletShader;
    [SerializeField]
    private int _updatesPerSec;
    private float _updateDelta;
    private float _nextUpdateTime;

    private int selected = -1;

    private static readonly Vector2 GRAVITY = new Vector2(0, -9.81f);
    private List<Point> _points = new List<Point>();
    private List<Stick> _sticks = new List<Stick>();

    private void Awake() {
        _updateDelta = 1f / _updatesPerSec;
        Debug.Log(_updateDelta);
        _nextUpdateTime = Time.time;
    }

    public void Solve() {
        // ######################## gravity ############################
        var gravityKernel = _verletShader.FindKernel("ApplyGravity");

        _verletShader.SetFloat("DeltaTime", Time.deltaTime);
        _verletShader.SetFloats("Gravity", GRAVITY.x, GRAVITY.y);
        _verletShader.SetInt("PointsLength", _points.Count);
        _verletShader.SetInt("SticksLength", _sticks.Count);

        ComputeBuffer pointsBuffer = new ComputeBuffer(_points.Count, System.Runtime.InteropServices.Marshal.SizeOf(typeof(Point)));
        pointsBuffer.SetData(_points);
        _verletShader.SetBuffer(gravityKernel, "Points", pointsBuffer);

        _verletShader.Dispatch(gravityKernel, _points.Count, 1, 1);

        // ######################## constraints ############################
        var constraintsKernel = _verletShader.FindKernel("Constraints");

        _verletShader.SetBuffer(constraintsKernel, "Points", pointsBuffer);

        ComputeBuffer sticksBuffer = new ComputeBuffer(_sticks.Count, System.Runtime.InteropServices.Marshal.SizeOf(typeof(Stick)));
        sticksBuffer.SetData(_sticks);
        _verletShader.SetBuffer(constraintsKernel, "Sticks", sticksBuffer);

        _nextUpdateTime = Time.time + _updateDelta;
        while (Time.time < _nextUpdateTime) {
            Debug.Log("Here");
            _verletShader.Dispatch(constraintsKernel, _points.Count, 1, 1);
        }

        {
            Point[] output = new Point[_points.Count];
            pointsBuffer.GetData(output);
            pointsBuffer.Release();
            _points = output.ToList();
        }
        {
            Stick[] output = new Stick[_sticks.Count];
            sticksBuffer.GetData(output);
            sticksBuffer.Release();
            _sticks = output.ToList();
        }

    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            _points.Add(new Point(Camera.main.ScreenToWorldPoint(Input.mousePosition), Input.GetMouseButton(1) ? 1 : 0));
            uint newPoint = (uint)_points.Count - 1;
            if (selected != -1) {
                _sticks.Add(new Stick((uint)selected, newPoint, Vector2.Distance(_points[selected].Position, _points[(int)newPoint].Position)));
            }
            selected = (int)newPoint;
        }

        if (Input.GetKeyDown(KeyCode.Space))
            Solve();
    }

    private void OnDrawGizmos() {
        foreach (var p in _points) {
            Gizmos.color = p.Locked == 1 ? Color.red : Color.blue;
            Gizmos.DrawSphere(p.Position, 0.1f);
        }

        foreach (var s in _sticks) {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(_points[(int)s.A].Position, _points[(int)s.B].Position);
        }
    }
}
