using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CpuVerletSolver : VerletSolver {
    public class Point {
        public Vector2 Position, PrevPosition;
        public bool Locked;
        public Point(Vector2 initialPosition, bool locked) {
            Position = initialPosition;
            PrevPosition = initialPosition;
            Locked = locked;
        }
    }

    public class Stick {
        public Point A { get; private set; }
        public Point B { get; private set; }
        public float Length { get; private set; }
        public Stick(Point a, Point b, float length) {
            A = a;
            B = b;
            Length = length;
        }
    }

    [SerializeField]
    Point _selectedNode = null;

    private static readonly Vector2 GRAVITY = new Vector2(0, -9.81f);
    private List<Point> _points = new List<Point>();
    private List<Stick> _sticks = new List<Stick>();
    private Vector2 _knifeStart, _knifeEnd;

    public void Solve() {
        const int STICK_ITERATIONS = 5;
        foreach (var p in _points) {
            if (!p.Locked) {
                Vector2 oldPos = p.Position;
                p.Position += p.Position - p.PrevPosition;
                p.Position += GRAVITY * Time.deltaTime * Time.deltaTime;
                p.PrevPosition = oldPos;
            }
        }

        for (int i = 0; i < STICK_ITERATIONS; i++) {
            foreach (var s in _sticks) {
                float actualLength = Vector2.Distance(s.A.Position, s.B.Position);
                float lengthDelta = s.Length - actualLength;
                Vector2 stickDir = (s.A.Position - s.B.Position).normalized;

                if (!s.A.Locked) {
                    s.A.Position += stickDir * lengthDelta / 2;
                }
                if (!s.B.Locked) {
                    s.B.Position -= stickDir * lengthDelta / 2;
                }
                /*

                Vector2 stickCenter = (s.A.Position + s.B.Position) / 2;
                Vector2 stickDir = (s.A.Position - s.B.Position).normalized;


            if (!s.A.Locked)
                s.A.Position = stickCenter + stickDir * s.Length / 2;
            if (!s.B.Locked)
                s.B.Position = stickCenter - stickDir * s.Length / 2;
                */
            }
        }
    }

    void Update() {
        if (Input.GetKey(KeyCode.Space))
            Solve();
    }

    public override void PlaceNode(bool locked) {
        var newPoint = new Point(Camera.main.ScreenToWorldPoint(Input.mousePosition), locked);
        _points.Add(newPoint);
        if (_selectedNode != null) {
            _sticks.Add(new Stick(_selectedNode, newPoint, Vector2.Distance(_selectedNode.Position, newPoint.Position)));
        }
        _selectedNode = newPoint;
    }

    public override void SetNodeLocked(bool locked) {
        var closestPoint = GetPointClosestToMouse(_points, point => point.Position);
        closestPoint.Item1.Locked = locked;
    }

    public override void SetKnife(Vector2 p1, Vector2 p2) {
        _knifeStart = p1;
        _knifeEnd = p2;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.cyan;
        foreach (var p in _points) {
            Gizmos.DrawSphere(p.Position, p.Locked ? 0.2f : 0.1f);
        }

        foreach (var s in _sticks) {
            bool indicateCut = Utils.Intersects(_knifeStart, _knifeEnd, s.A.Position, s.B.Position);
            Debug.Log(indicateCut);
            Gizmos.color = indicateCut ? Color.red : Color.cyan;

            /*
            Vector2? cutLocation = Utils.GetIntersection(_knifeStart, _knifeEnd, s.A.Position, s.B.Position);
            if (cutLocation != null) {
                Gizmos.DrawSphere((Vector2) cutLocation, 0.1f);
                Debug.Log(cutLocation);
            }
            */
            Gizmos.DrawLine(s.A.Position, s.B.Position);
        }
    }
}
