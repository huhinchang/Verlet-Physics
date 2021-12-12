using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LinkTool : Tool
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SelectClosestPoint(false);
        }
        if (Input.GetMouseButtonDown(1))
        {
            LinkClosestPoint(false);
        }
    }

    private void SelectClosestPoint(bool locked)
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            _solvers.ForEach(v => v.SelectClosest());
        }
    }

    private void LinkClosestPoint(bool locked)
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            _solvers.ForEach(v => v.LinkClosestToSelected());
        }
    }
}
