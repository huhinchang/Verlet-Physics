using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PointWidget : MonoBehaviour
{
    private const float _kLockedScale = 0.2f;
    private const float _kUnlockedScale = 0.1f;
    
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

    public void UpdateState(Vector2 pos, bool isLocked)
    {
        transform.position = pos;
        transform.localScale = Vector3.one * ( isLocked? _kLockedScale : _kUnlockedScale);
    }
}
