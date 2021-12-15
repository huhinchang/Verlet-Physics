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

    private MetaTool _currentTool = default;

    void Start()
    {
        foreach (var metaTool in _tools)
        {
            metaTool.Tool.Setup(_solvers);
            metaTool.Tool.gameObject.SetActive(false);
            metaTool.Toggle.onValueChanged.AddListener((value) => ToggleTool(metaTool, value));
        }
        ChangeTool(_tools[0]);
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

    private void ChangeTool(MetaTool tool)
    {
        _currentTool?.Tool.Deselect();

        _currentTool = tool;
        _currentTool.Tool.Select();
        _descText.text = _currentTool.Tool.Tooltip;
        _solvers.ForEach(s => s.ShowNearestIndicator = _currentTool.Tool.ShowNearestIndicator);
    }
}
