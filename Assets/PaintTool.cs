using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

public class PaintTool : Tool
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Paint(false);
        } else if (Input.GetMouseButtonDown(1))
        {
            Paint(true);
        }
    }

    private void Paint(bool locked)
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            _solvers.ForEach(v => v.LockClosest(locked));
        }
    }
}
