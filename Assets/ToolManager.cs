using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class ToolManager : MonoBehaviour
{
    [System.Serializable]
    class MetaTool
    {
        public Toggle Toggle;
        public Tool Tool;
    }

    [SerializeField]
    private TMP_Text _descText = default;
    [SerializeField]
    private List<VerletSolverWrapper> _solvers;
    [SerializeField]
    private MetaTool[] _tools = default;

    void Start()
    {
        foreach (var metaTool in _tools)
        {
            metaTool.Tool.Setup(_solvers);
            metaTool.Tool.gameObject.SetActive(false);
            metaTool.Toggle.onValueChanged.AddListener((value) => ToggleTool(metaTool, value));
        }
        _tools[0].Tool.Select();
        _tools[0].Toggle.isOn = true;
        _descText.text = _tools[0].Tool.Tooltip;
        _solvers.ForEach(s => s.ShowNearestIndicator = _tools[0].Tool.ShowNearestIndicator);
    }

    private void OnDestroy()
    {
        foreach (var metaTool in _tools)
        {
            metaTool.Toggle.onValueChanged.RemoveAllListeners();
        }
    }

    private void ToggleTool(MetaTool tool, bool isOn)
    {
        if (isOn)
        {
            tool.Tool.Select();
            _descText.text = tool.Tool.Tooltip;
            _solvers.ForEach(s => s.ShowNearestIndicator = tool.Tool.ShowNearestIndicator);
        } else
        {
            tool.Tool.Deselect();
        }

    }
}
