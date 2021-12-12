using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlaceTool : Tool
{
    void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButtonDown(0))
            {
                _solvers.ForEach(v => v.CreatePointAtMouse(locked: false));
            } else if (Input.GetMouseButtonDown(1))
            {
                _solvers.ForEach(v => v.CreatePointAtMouse(locked: true));
            }
        }
    }
}
