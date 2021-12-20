using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Assertions;

public class HeldButton : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
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
        if (_graphic != null)
            _graphic.enabled = true;
        StartCoroutine(SetTriggerInstant(_kPointerDownHash));
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_graphic != null)
            _graphic.enabled = false;
        StartCoroutine(SetTriggerInstant(_kPointerUpHash));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("HERE");
        StartCoroutine(SetTriggerInstant(_kPointerExitHash));
    }

    IEnumerator SetTriggerInstant(int hash)
    {
        _anim.SetTrigger(hash);
        yield return new WaitForEndOfFrame();
        _anim.ResetTrigger(hash);
    }
}
