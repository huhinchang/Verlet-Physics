using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GpuVerletSolver : VerletSolverWrapper
{
    [SerializeField]
    private ComputeShader _gravityShader = default;
    [SerializeField]
    private ComputeShader _constraintShader = default;

    protected override void Solve()
    {
        // ######################## gravity ############################
        var gravityKernel = _gravityShader.FindKernel("ApplyGravity");

        _gravityShader.SetFloat("DeltaTime", Time.deltaTime);
        _gravityShader.SetFloats("Gravity", _kGravity.x, _kGravity.y);
        _gravityShader.SetInt("PointsLength", _points.Count);

        ComputeBuffer pointsBuffer = new ComputeBuffer(_points.Count, System.Runtime.InteropServices.Marshal.SizeOf(typeof(Point)));
        pointsBuffer.SetData(_points);
        _gravityShader.SetBuffer(gravityKernel, "Points", pointsBuffer);

        _gravityShader.Dispatch(gravityKernel, _points.Count, 1, 1);

        // ######################## constraints ############################
        var constraintsKernel = _constraintShader.FindKernel("Constraints");

        _constraintShader.SetBuffer(constraintsKernel, "Points", pointsBuffer);
        _constraintShader.SetInt("SticksLength", _sticks.Count);

        ComputeBuffer sticksBuffer = new ComputeBuffer(_sticks.Count, System.Runtime.InteropServices.Marshal.SizeOf(typeof(Stick)));
        sticksBuffer.SetData(_sticks);
        _constraintShader.SetBuffer(constraintsKernel, "Sticks", sticksBuffer);

        for (int i = 0; i < _constraintReps; i++)
        {
            _constraintShader.Dispatch(constraintsKernel, _points.Count, 1, 1);
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

        base.Solve();
    }
}
