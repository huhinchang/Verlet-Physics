using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class StickWidget : MonoBehaviour
{
    [SerializeField]
    LineRenderer _lineRenderer = default;

    private void OnValidate()
    {
        Assert.IsNotNull(_lineRenderer);
    }

    public void Initialize(Color color, int layer)
    {
        _lineRenderer.startColor = color;
        _lineRenderer.endColor = color;
        gameObject.layer = layer;
    }

    public void UpdateState(Vector2 a, Vector2 b) {
        _lineRenderer.SetPosition(0, a);
        _lineRenderer.SetPosition(1, b);
    }
}
