using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class KnifeTool : Tool
{
    [SerializeField]
    private StickWidget _knifeIndicator = default;
    [SerializeField]
    private Color _knifeColor = default;
    private Vector2 _start, _end;

    private void Awake()
    {
        _knifeIndicator.Initialize(_knifeColor, gameObject.layer);
        _knifeIndicator.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButtonDown(0))
            {
                _start = Utils.Generic.GetMousePosition();
                _knifeIndicator.UpdateState(_start, _end);
                _knifeIndicator.gameObject.SetActive(true);

                _solvers.ForEach(v => v.SetKnife(_start, _end));
                _solvers.ForEach(v => v.SetKnifeActive(true));
            }
            if (Input.GetMouseButton(0))
            {
                _end = Utils.Generic.GetMousePosition();
                _knifeIndicator.UpdateState(_start, _end);

                _solvers.ForEach(v => v.SetKnife(_start, _end));
            }
            if (Input.GetMouseButtonUp(0))
            {
                _knifeIndicator.gameObject.SetActive(false);
                _solvers.ForEach(v => v.Cut());
                _solvers.ForEach(v => v.SetKnifeActive(false));
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawLine(_start, _end);
    }
}
