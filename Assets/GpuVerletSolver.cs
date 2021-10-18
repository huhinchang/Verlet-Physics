using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GpuVerletSolver : VerletSolver {
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
    private int contraintReps;

    private int selected = -1;

    private static readonly Vector2 GRAVITY = new Vector2(0, -9.81f);
    private List<Point> _points = new List<Point>();
    private List<Stick> _sticks = new List<Stick>();
    private Vector2 _knifeStart, _knifeEnd;

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

        for (int i = 0; i < contraintReps; i++) {
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
        if (Input.GetKey(KeyCode.Space))
            Solve();
    }

    public override void PlaceNode(bool locked) {
        _points.Add(new Point(Camera.main.ScreenToWorldPoint(Input.mousePosition), Input.GetMouseButton(1) ? 1 : 0));
        uint newPoint = (uint)_points.Count - 1;
        if (selected != -1) {
            _sticks.Add(new Stick((uint)selected, newPoint, Vector2.Distance(_points[selected].Position, _points[(int)newPoint].Position)));
        }
        selected = (int)newPoint;
    }

    public override void SetNodeLocked(bool locked) {
        var closestPoint = GetPointClosestToMouse(_points, point => point.Position);
        _points[closestPoint.Item3] = new Point(closestPoint.Item1.Position, locked ? 1 : 0);
    }

    public override void SetKnife(Vector2 p1, Vector2 p2) {
        _knifeStart = p1;
        _knifeEnd = p2;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.magenta;
        foreach (var p in _points) {
            Gizmos.DrawSphere(p.Position, p.Locked == 1 ? 0.2f : 0.1f);
        }

        foreach (var s in _sticks) {
            Gizmos.DrawLine(_points[(int)s.A].Position, _points[(int)s.B].Position);
        }
    }
}
