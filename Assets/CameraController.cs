using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    UniversalAdditionalCameraData _camData = default;
    [SerializeField]
    Camera _cpuCam = default;
    [SerializeField]
    Camera _gpuCam = default;

    public void SetOrder(bool cpuFirst) {
        _camData.cameraStack.Clear();
        if (cpuFirst) {
            _camData.cameraStack.Add(_cpuCam);
            _camData.cameraStack.Add(_gpuCam);
        } else {
            _camData.cameraStack.Add(_gpuCam);
            _camData.cameraStack.Add(_cpuCam);
        }
    }
}
