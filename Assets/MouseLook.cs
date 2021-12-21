using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [SerializeField]
    private Camera[] _cams = default;
    [SerializeField]
    private Bounds _camBounds;
    [SerializeField]
    private int _moveEdgeThickness = 10;
    [SerializeField]
    private float _moveSensitivity = 10f;
    [SerializeField]
    private float _minZoomSize = 3f;
    [SerializeField]
    private float _maxZoomSize = 25f;
    [SerializeField]
    private float scrollSensitivity = 10f;

    void Update()
    {
        Move();
        Zoom();
    }

    private void Move()
    {
        Vector3 moveDir = Vector3.zero;
        Vector3 newPos = transform.position;

        // movement
        if (Input.mousePosition.x < _moveEdgeThickness || Input.mousePosition.x > Screen.width - _moveEdgeThickness || Input.mousePosition.y < _moveEdgeThickness || Input.mousePosition.y > Screen.height - _moveEdgeThickness)
        {
            moveDir = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
            moveDir.Normalize();
        }

        newPos += moveDir * _moveSensitivity * Time.deltaTime;

        // clamping
        newPos.x = Mathf.Clamp(newPos.x, _camBounds.min.x, _camBounds.max.x);
        newPos.y = Mathf.Clamp(newPos.y, _camBounds.min.y, _camBounds.max.y);

        transform.position = newPos;
    }

    void Zoom()
    {
        float zoomDelta = Input.mouseScrollDelta.y * scrollSensitivity;
        foreach (var cam in _cams)
        {
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize + zoomDelta, _minZoomSize, _maxZoomSize);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(_camBounds.center, _camBounds.size);
    }
}
