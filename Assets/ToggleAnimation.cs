using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Assertions;

public class ToggleAnimation : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
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
        _anim.SetBool(_kIsToggleOnHash, _toggle.isOn);
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
        StartCoroutine(SetTriggerInstant(_kPointerClickHash));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StartCoroutine(SetTriggerInstant(_kPointerEnterHash));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StartCoroutine(SetTriggerInstant(_kPointerExitHash));
    }

    IEnumerator SetTriggerInstant(int hash) {
        _anim.SetTrigger(hash);
        yield return new WaitForEndOfFrame();
        _anim.ResetTrigger(hash);
    }
}
