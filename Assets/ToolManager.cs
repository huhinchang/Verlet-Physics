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
        public Button Button;
        public Tool Tool;
    }

    [SerializeField]
    private TMP_Text _descText = default;
    [SerializeField]
    private List<VerletSolver> _solvers;
    [SerializeField]
    private MetaTool[] _tools = default;

    private MetaTool _currentTool = default;

    void Start()
    {
        foreach (var metaTool in _tools)
        {
            //_toolDictionary.Add(tool.Button, tool);
            metaTool.Tool.Setup(_solvers);
            metaTool.Tool.gameObject.SetActive(false);
            metaTool.Button.onClick.AddListener(() => ChangeTool(metaTool));
            metaTool.Tool.OnChangeTooltip += HandleShowTooltip;
        }
        ChangeTool(_tools[0]);
    }

    private void OnDestroy()
    {
        foreach (var metaTool in _tools)
        {
            metaTool.Button.onClick.RemoveAllListeners();
            metaTool.Tool.OnChangeTooltip -= HandleShowTooltip;
        }
    }

    private void ChangeTool(MetaTool tool)
    {
        _currentTool?.Tool.Deselect();

        _currentTool = tool;
        _currentTool.Tool.Select();
    }

    private void HandleShowTooltip(string desc) {
        _descText.text = desc;
    }
}
