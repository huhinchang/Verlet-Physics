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
        public Toggle Toggle;
        public GameObject GameObject;

        public void HandleTabChange(bool isOn)
        {
            GameObject.SetActive(isOn);
        }
    }

    [SerializeField]
    private Tab[] _tabs = default;

    void Start()
    {
        foreach (var tab in _tabs)
        {
            tab.Toggle.onValueChanged.AddListener(tab.HandleTabChange);
            tab.GameObject.SetActive(tab.Toggle.isOn);
        }
    }

    private void OnDisable()
    {
        Array.ForEach(_tabs, tab => tab.Toggle.onValueChanged.RemoveAllListeners());
    }
}
