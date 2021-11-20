using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlaceTool : Tool
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PlacePoint(false);
        } else if (Input.GetMouseButtonDown(1))
        {
            PlacePoint(true);
        }
    }

    private void PlacePoint(bool locked)
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            _solvers.ForEach(v => v.CreatePointAtMouse(locked));
        }
    }
}
