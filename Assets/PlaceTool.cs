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
            PlaceNode(false);
        } else if (Input.GetMouseButtonDown(1))
        {
            PlaceNode(true);
        }
    }

    private void PlaceNode(bool locked)
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("Placing");
            _solvers.ForEach(v => v.PlaceNode(locked));
        }
    }
}
