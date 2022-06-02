using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class SwitchToggle : MonoBehaviour
{
    [SerializeField] private RectTransform uiHandleRectTransform = null;
    [SerializeField] private Color backgroundActiveColor;
    [SerializeField] private Color handleActiveColor;

    [SerializeField] private Toggle toggle = null;

    [SerializeField] private Image backgroundImage = null;
    [SerializeField] private Image handleImage = null;

    private Color backgroundDefaultColor;
    private Color handleDefaultColor;

    private Vector2 handlePosition = Vector2.zero;

    public Action<Toggle> OnToggleChanged;

    private void Awake()
    {
        handlePosition = uiHandleRectTransform.anchoredPosition;

        backgroundDefaultColor = backgroundImage.color;
        handleDefaultColor = handleImage.color;

        toggle.onValueChanged.AddListener(OnSwitch);

        if (toggle.isOn)
            OnSwitch(true);
    }

    private void OnSwitch(bool on)
    {
        OnToggleChanged?.Invoke(toggle);

        uiHandleRectTransform.DOAnchorPos(on ? handlePosition * -1 : handlePosition, .4f).SetEase(Ease.InOutBack);

        backgroundImage.DOColor(on ? backgroundActiveColor : backgroundDefaultColor, .6f);

        handleImage.DOColor(on ? handleActiveColor : handleDefaultColor, .4f);
    }

    private void OnDestroy()
    {
        toggle.onValueChanged.RemoveListener(OnSwitch);
    }
}