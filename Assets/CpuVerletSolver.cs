using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class CpuVerletSolver : VerletSolverWrapper
{
    protected override void Solve()
    {
        for (int i = 0; i < _points.Count; i++)
        {
            Point p = _points[i];
            if (p.Locked.IsFalse())
            {
                Vector2 newPos = p.Position;

                newPos += p.Position - p.PrevPosition;
                newPos += _kGravity * Time.deltaTime * Time.deltaTime;

                _points[i] = new Point(newPos, p.Position, p.Locked);
            }
        }

        for (int i = 0; i < _constraintReps; i++)
        {
            for (int j = 0; j < _sticks.Count; j++)
            {
                Stick s = _sticks[j];
                Point a = _points[s.A];
                Point b = _points[s.B];

                float actualLength = Vector2.Distance(_points[s.A].Position, _points[s.B].Position);
                float lengthDelta = s.Length - actualLength;
                Vector2 stickDir = (a.Position - b.Position).normalized;

                if (a.Locked.IsFalse())
                {
                    a.Position += stickDir * lengthDelta / 2;
                }
                if (b.Locked.IsFalse())
                {
                    b.Position -= stickDir * lengthDelta / 2;
                }

                _points[s.A] = a;
                _points[s.B] = b;
            }
        }

        base.Solve();
    }
}
