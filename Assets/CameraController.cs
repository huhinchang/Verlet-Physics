using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using UnityEngine.Assertions;

public class CameraController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField]
    Toggle _renderCpuAboveToggle = default;
    [SerializeField]
    Toggle _renderCpuToggle = default;
    [SerializeField]
    Toggle _renderGpuToggle = default;

    [Header("Other References")]
    [SerializeField]
    UniversalAdditionalCameraData _camData = default;
    [SerializeField]
    Camera _cpuCam = default;
    [SerializeField]
    Camera _gpuCam = default;
    [SerializeField]
    Camera _toolsCam = default;

    private void OnValidate()
    {
        Assert.IsNotNull(_renderCpuAboveToggle);
        Assert.IsNotNull(_renderCpuToggle);
        Assert.IsNotNull(_renderGpuToggle);
        Assert.IsNotNull(_camData);
        Assert.IsNotNull(_cpuCam);
        Assert.IsNotNull(_gpuCam);
        Assert.IsNotNull(_toolsCam);
    }

    private void Awake()
    {
        _renderCpuAboveToggle.isOn = true;
        _renderCpuToggle.isOn = true;
        _renderGpuToggle.isOn = true;

        _renderCpuAboveToggle.onValueChanged.AddListener(FlipOrder);
        _renderCpuToggle.onValueChanged.AddListener(ToggleCpu);
        _renderGpuToggle.onValueChanged.AddListener(ToggleGpu);
    }

    private void OnDestroy()
    {
        _renderCpuAboveToggle.onValueChanged.RemoveListener(FlipOrder);
        _renderCpuToggle.onValueChanged.RemoveListener(ToggleCpu);
        _renderGpuToggle.onValueChanged.RemoveListener(ToggleGpu);
    }

    public void FlipOrder(bool cpuAbove)
    {
        _camData.cameraStack.Clear();
        if (cpuAbove)
        {
            _camData.cameraStack.Add(_gpuCam);
            _camData.cameraStack.Add(_cpuCam);
            _camData.cameraStack.Add(_toolsCam);
        } else
        {
            _camData.cameraStack.Add(_cpuCam);
            _camData.cameraStack.Add(_gpuCam);
            _camData.cameraStack.Add(_toolsCam);
        }
    }

    public void ToggleCpu(bool value)
    {
        _cpuCam.enabled = value;
    }

    public void ToggleGpu(bool value)
    {
        _gpuCam.enabled = value;
    }
}
