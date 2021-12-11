using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class StickWidget : MonoBehaviour
{
    [SerializeField]
    LineRenderer _lineRenderer = default;

    private Color _baseColor;

    private void OnValidate()
    {
        Assert.IsNotNull(_lineRenderer);
    }

    public void Initialize(Color baseColor, int layer)
    {
        _lineRenderer.startColor = baseColor;
        _lineRenderer.endColor = baseColor;
        gameObject.layer = layer;
    }

    public void UpdateState(Vector2 a, Vector2 b) {
        _lineRenderer.SetPosition(0, a);
        _lineRenderer.SetPosition(1, b);
    }

    public void UpdateState(Vector2 a, Vector2 b, Color color)
    {
        UpdateState(a, b);
        _lineRenderer.startColor = color;
        _lineRenderer.endColor = color;
    }
}
