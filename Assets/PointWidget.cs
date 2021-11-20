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

    // Update is called once per frame
    void Update()
    {
        
    }
}
