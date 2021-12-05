using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using UnityEngine.Assertions;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    Toggle _renderCpuAboveToggle = default;
    [SerializeField]
    UniversalAdditionalCameraData _camData = default;
    [SerializeField]
    Camera _cpuCam = default;
    [SerializeField]
    Camera _gpuCam = default;

    private void OnValidate()
    {
        Assert.IsNotNull(_renderCpuAboveToggle);
        Assert.IsNotNull(_camData);
        Assert.IsNotNull(_cpuCam);
        Assert.IsNotNull(_gpuCam);
    }

    private void Awake()
    {
        _renderCpuAboveToggle.onValueChanged.AddListener(FlipOrder);
    }

    private void OnDestroy()
    {
        _renderCpuAboveToggle.onValueChanged.RemoveListener(FlipOrder);
    }

    public void FlipOrder(bool cpuAbove) {
        _camData.cameraStack.Clear();
        if (cpuAbove) {
            _camData.cameraStack.Add(_gpuCam);
            _camData.cameraStack.Add(_cpuCam);
        } else {
            _camData.cameraStack.Add(_cpuCam);
            _camData.cameraStack.Add(_gpuCam);
        }
    }
}
