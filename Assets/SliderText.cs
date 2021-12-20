using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Assertions;
using System;

public class SliderText : MonoBehaviour
{
    [SerializeField]
    Slider _slider = default;
    [SerializeField]
    TMP_Text _text = default;
    [SerializeField, TextArea(1,3)]
    string _format = default;

    private Color _baseColor;

    private void OnValidate()
    {
        Assert.IsNotNull(_slider);
        Assert.IsNotNull(_text);
    }

    private void OnEnable()
    {
        _slider.onValueChanged.AddListener(HandleSliderValueChanged);
        HandleSliderValueChanged(_slider.value);
    }

    private void OnDisable()
    {
        _slider.onValueChanged.RemoveListener(HandleSliderValueChanged);
    }

    private void HandleSliderValueChanged(float value)
    {
        _text.text = string.Format(_format, value);
    }
}
