using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tool : MonoBehaviour
{
    [SerializeField, TextArea(3,5)]
    private string _tooltip = default;

    protected List<VerletSolverWrapper> _solvers;

    public string Tooltip => _tooltip;

    public void Setup(List<VerletSolverWrapper> solvers)
    {
        _solvers = solvers;
    }

    public void Select() {
        gameObject.SetActive(true);
    }

    public void Deselect() {
        gameObject.SetActive(false);
    }
}
