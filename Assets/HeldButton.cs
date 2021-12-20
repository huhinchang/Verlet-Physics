using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Assertions;
using System;

public class HeldButton : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public Action<bool> OnPressChanged;

    private const string _kPointerEnter = "PointerEnter";
    private const string _kPointerDown = "PointerDown";
    private const string _kPointerUp = "PointerUp";
    private const string _kPointerExit = "PointerExit";

    private readonly int _kPointerEnterHash = Animator.StringToHash(_kPointerEnter);
    private readonly int _kPointerDownHash = Animator.StringToHash(_kPointerDown);
    private readonly int _kPointerUpHash = Animator.StringToHash(_kPointerUp);
    private readonly int _kPointerExitHash = Animator.StringToHash(_kPointerExit);

    [SerializeField]
    private Animator _anim = default;
    [SerializeField]
    private Image _graphic = default;

    private void OnValidate()
    {
        Assert.IsNotNull(_anim);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StartCoroutine(SetTriggerInstant(_kPointerEnterHash));
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        UpdatePressed(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        UpdatePressed(false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UpdatePressed(false);
    }

    private void UpdatePressed(bool pressed)
    {
        OnPressChanged?.Invoke(pressed);
        if (_graphic != null)
        {
            _graphic.enabled = pressed;
        }
        if (pressed)
        {
            StartCoroutine(SetTriggerInstant(_kPointerDownHash));
        } else
        {
            StartCoroutine(SetTriggerInstant(_kPointerUpHash));
        }
    }

    IEnumerator SetTriggerInstant(int hash)
    {
        _anim.SetTrigger(hash);
        yield return new WaitForEndOfFrame();
        _anim.ResetTrigger(hash);
    }
}
