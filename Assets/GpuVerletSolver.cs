using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GpuVerletSolver : VerletSolverWrapper
{
    [SerializeField]
    private ComputeShader _verletShader = default;

    protected override void Solve()
    {
        // ######################## gravity ############################
        var gravityKernel = _verletShader.FindKernel("ApplyGravity");

        _verletShader.SetFloat("DeltaTime", Time.deltaTime);
        _verletShader.SetFloats("Gravity", _kGravity.x, _kGravity.y);
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

        for (int i = 0; i < _constraintReps; i++)
        {
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

        base.Solve();
    }
}
