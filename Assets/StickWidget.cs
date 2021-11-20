using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickWidget : MonoBehaviour
{
    [SerializeField]
    LineRenderer _lineRenderer = default;

    public void UpdateState(Vector2 a, Vector2 b) {
        _lineRenderer.SetPosition(0, a);
        _lineRenderer.SetPosition(1, b);
    }
}
