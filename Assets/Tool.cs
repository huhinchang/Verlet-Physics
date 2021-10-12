using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tool : MonoBehaviour
{
    protected CpuVerletSolver _cpu = default;
    protected GpuVerletSolver _gpu = default;

    public void Setup(CpuVerletSolver cpu, GpuVerletSolver gpu) {
        _cpu = cpu;
        _gpu = gpu;
    }
}
