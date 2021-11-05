using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tool : MonoBehaviour
{
    protected List<VerletSolver> _solvers;

    public void Setup(List<VerletSolver> solvers)
    {
        _solvers = solvers;
    }
}
