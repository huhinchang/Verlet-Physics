using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tool : MonoBehaviour
{
    public Action<string> OnChangeTooltip;

    [System.Serializable]
    class Tooltip
    {
        public string Context;
        [TextArea(3, 5)]
        public string Description;
    }

    [SerializeField]
    private Tooltip[] _tooltips;

    protected List<VerletSolver> _solvers;

    public void Setup(List<VerletSolver> solvers)
    {
        _solvers = solvers;
    }

    public void Select() {
        gameObject.SetActive(true);
        OnChangeTooltip?.Invoke(_tooltips[0].Description);
    }

    public void Deselect() {
        gameObject.SetActive(false);
    }

    protected void ShowTooltip(string context) {
        OnChangeTooltip?.Invoke(_tooltips.First(t => t.Context == context).Description);
    }
}
