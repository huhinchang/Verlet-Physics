using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TabController : MonoBehaviour
{
    [System.Serializable]
    class Tab
    {
        public Button Button;
        public GameObject GameObject;
    }
    [SerializeField]
    bool _collapsable = default;

    [SerializeField]
    private Tab[] _tabs = default;

    private Tab _activeTab;

    void Start()
    {
        Array.ForEach(_tabs, tab => tab.Button.onClick.AddListener(() => HandleTabChange(tab)));
        Array.ForEach(_tabs, tab => tab.GameObject.SetActive(false));
        HandleTabChange(_tabs[0]);
    }

    private void OnDisable()
    {
        Array.ForEach(_tabs, tab => tab.Button.onClick.RemoveAllListeners());
    }

    private void HandleTabChange(Tab tab)
    {
        if (_activeTab == null || _activeTab != tab)
        {
            _activeTab?.GameObject.SetActive(false);
            tab.GameObject.SetActive(true);
            _activeTab = tab;
        } else if (_collapsable)
        {
            _activeTab?.GameObject.SetActive(false);
            _activeTab = null;
        }
    }
}
