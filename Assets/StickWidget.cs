using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class StickWidget : MonoBehaviour
{
    [SerializeField]
    LineRenderer _lineRenderer = default;

    private Color _baseColor;
    private Color _cutPreviewColor;

    private void OnValidate()
    {
        Assert.IsNotNull(_lineRenderer);
    }

    public void Initialize(Color baseColor, Color cutPreviewColor, int layer)
    {
        _baseColor = baseColor;
        _cutPreviewColor = cutPreviewColor;
        gameObject.layer = layer;
    }

    public void UpdateState(Vector2 a, Vector2 b, bool previewCut) {
        _lineRenderer.SetPosition(0, a);
        _lineRenderer.SetPosition(1, b);

        var colorToShow = previewCut ? _cutPreviewColor : _baseColor;
        _lineRenderer.startColor = colorToShow;
        _lineRenderer.endColor = colorToShow;
    }
}
