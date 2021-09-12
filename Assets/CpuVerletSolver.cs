using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CpuVerletSolver : VerletSolver {
    [SerializeField]
    Point selected = null;

    public override void Solve() {
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
        if (Input.GetMouseButtonDown(0)) {
            Point newPoint = new Point(Camera.main.ScreenToWorldPoint(Input.mousePosition), Input.GetMouseButton(1));
            _points.Add(newPoint);
            if (selected != null) {
                _sticks.Add(new Stick(selected, newPoint, Vector2.Distance(selected.Position, newPoint.Position)));
            }
            selected = newPoint;
        }

        if (Input.GetKey(KeyCode.Space))
            Solve();
    }
}
