using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Assertions;

public class ToggleAnimation : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler, IDragHandler
{
    private const string _kPointerEnter = "PointerEnter";
    private const string _kPointerClick = "PointerClick";
    private const string _kPointerExit = "PointerExit";
    private const string _kIsToggleOn = "IsToggleOn";

    private readonly int _kPointerEnterHash = Animator.StringToHash(_kPointerEnter);
    private readonly int _kPointerClickHash = Animator.StringToHash(_kPointerClick);
    private readonly int _kPointerExitHash = Animator.StringToHash(_kPointerExit);
    private readonly int _kIsToggleOnHash = Animator.StringToHash(_kIsToggleOn);

    [SerializeField]
    private Toggle _toggle = default;
    [SerializeField]
    private Animator _anim = default;

    private void OnValidate()
    {
        Assert.IsNotNull(_toggle);
        Assert.IsNotNull(_anim);
    }

    private void OnEnable()
    {
        _toggle.onValueChanged.AddListener(HandleToggleValueChanged);
    }

    private void HandleToggleValueChanged(bool isOn) {
        _anim.SetBool(_kIsToggleOnHash, isOn);
    }

    private void OnDisable()
    {
        _toggle.onValueChanged.RemoveListener(HandleToggleValueChanged);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _anim.SetTrigger(_kPointerClickHash);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _anim.SetTrigger(_kPointerEnterHash);
        
        Debug.Log($"{eventData.pointerEnter.gameObject.name} AAAAA");
        Debug.Log($"{gameObject.name} OnPointerEnter");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _anim.SetTrigger(_kPointerExitHash);
    }

    public void OnDrag(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }
}
