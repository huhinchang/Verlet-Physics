using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PointWidget : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer _spriteRenderer = default;

    private void OnValidate()
    {
        Assert.IsNotNull(_spriteRenderer);
    }

    public void Initialize(Color color, int layer)
    {
        _spriteRenderer.color = color;
        gameObject.layer = layer;
    }

    public void UpdateState(Vector2 pos)
    {
        transform.position = pos;
    }
}
