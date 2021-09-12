using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VerletSolver : MonoBehaviour {
    public class Point {
        public Vector2 Position, PrevPosition;
        public bool Locked;
        public Point(Vector2 initialPosition, bool l) {
            Position = initialPosition;
            PrevPosition = initialPosition;
            Locked = l;
        }
    }

    public class Stick {
        public Point A { get; private set; }
        public Point B { get; private set; }
        public float Length { get; private set; }
        public Stick(Point a, Point b, float l) {
            A = a;
            B = b;
            Length = l;
        }
    }

    protected static readonly Vector2 GRAVITY = new Vector2(0, -9.81f);
    protected List<Point> _points = new List<Point>();
    protected List<Stick> _sticks = new List<Stick>();
    public abstract void Solve();

    private void OnDrawGizmos() {
        foreach (var p in _points) {
            Gizmos.color = p.Locked ? Color.red : Color.white;
            Gizmos.DrawSphere(p.Position, 0.1f);
        }

        foreach (var s in _sticks) {
            Gizmos.DrawLine(s.A.Position, s.B.Position);
        }
    }
}
