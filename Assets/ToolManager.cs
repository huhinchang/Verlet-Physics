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
        public GameObject GameObject;
        [TextArea(3, 5)]
        public string Desc;
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
            metaTool.GameObject.GetComponent<Tool>().Setup(_solvers);
            metaTool.GameObject.SetActive(false);
            metaTool.Button.onClick.AddListener(() => ChangeTool(metaTool));
        }
        ChangeTool(_tools[0]);
    }

    private void OnDestroy()
    {
        foreach (var tool in _tools)
        {
            tool.Button.onClick.RemoveAllListeners();
        }
    }

    private void ChangeTool(MetaTool tool)
    {
        _currentTool?.GameObject.SetActive(false);
        _currentTool = tool;

        _currentTool.GameObject.SetActive(true);
        _descText.text = _currentTool.Desc;
    }
}
