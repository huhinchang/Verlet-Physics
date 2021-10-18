using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeTool : Tool
{
    private Vector2 _start, _end;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            _start = Utils.GetMousePosition();
            _cpu.SetKnife(_start, _end);
        }
        if (Input.GetMouseButton(0)) {
            _end = Utils.GetMousePosition();
            _cpu.SetKnife(_start, _end);
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(_start, _end);
    }
}
