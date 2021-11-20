using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class KnifeTool : Tool
{
    private Vector2 _start, _end;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _start = Utils.Generic.GetMousePosition();
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                _solvers.ForEach(v => v.SetKnife(_start, _end));
            }
        }
        if (Input.GetMouseButton(0))
        {
            _end = Utils.Generic.GetMousePosition();
            _solvers.ForEach(v => v.SetKnife(_start, _end));
        }
        if (Input.GetMouseButtonUp(0))
        {
            _solvers.ForEach(v => v.Cut());
        }
    }

    private void OnDrawGizmos()
    {
        Debug.Log("Here");
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(_start, _end);
    }
}
