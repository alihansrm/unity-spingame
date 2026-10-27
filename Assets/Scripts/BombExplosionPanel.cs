using System;
using DG.Tweening;
using UnityEngine;

public class BombExplosionPanel : MonoBehaviour
{
    [SerializeField] private CanvasGroup overlay;
    [SerializeField] private RectTransform content;
    [SerializeField] private RectTransform bombCard;
    [SerializeField] private UnityEngine.UI.Button triggerButton;
    [SerializeField] private UnityEngine.UI.Button giveUpButton;
    [SerializeField] private UnityEngine.UI.Button currencyReviveButton;
    [SerializeField] private UnityEngine.UI.Button adReviveButton;
    [SerializeField, Min(0.1f)] private float openingDuration = 0.4f;

    private Sequence transition;

    public bool IsVisible { get; private set; }
    public bool IsAnimating => transition != null;

    // The game can subscribe here to implement loss, currency or rewarded ads.
    public event Action GiveUpRequested;
    public event Action CurrencyReviveRequested;
    public event Action AdReviveRequested;

    private void OnValidate()
    {
        openingDuration = Mathf.Max(0.1f, openingDuration);
        ResolveReferences();
    }

    private void ResolveReferences()
    {
        overlay = transform.Find("ui_bomb_overlay")?.GetComponent<CanvasGroup>();
        content = transform.Find("ui_bomb_overlay/ui_bomb_content") as RectTransform;
        bombCard = content?.Find("ui_bomb_card") as RectTransform;
        triggerButton = transform.Find("ui_button_trigger_bomb")
            ?.GetComponent<UnityEngine.UI.Button>();
        giveUpButton = content?.Find("ui_bomb_actions/ui_button_give_up")
            ?.GetComponent<UnityEngine.UI.Button>();
        currencyReviveButton = content?.Find("ui_bomb_actions/ui_button_revive_currency")
            ?.GetComponent<UnityEngine.UI.Button>();
        adReviveButton = content?.Find("ui_bomb_actions/ui_button_revive_ad")
            ?.GetComponent<UnityEngine.UI.Button>();
    }

    private void Awake()
    {
        ResolveReferences();
        if (overlay == null || content == null || bombCard == null ||
            giveUpButton == null ||
            currencyReviveButton == null || adReviveButton == null)
        {
            Debug.LogError("Bomb explosion UI references are missing.", this);
            enabled = false;
            return;
        }

        ResetHidden();
    }

    private void OnEnable()
    {
        if (triggerButton != null)
            triggerButton.onClick.AddListener(ShowBombExplosion);
        if (giveUpButton != null)
            giveUpButton.onClick.AddListener(GiveUp);
        if (currencyReviveButton != null)
            currencyReviveButton.onClick.AddListener(ReviveWithCurrency);
        if (adReviveButton != null)
            adReviveButton.onClick.AddListener(ReviveWithAd);
    }

    private void OnDisable()
    {
        if (triggerButton != null)
            triggerButton.onClick.RemoveListener(ShowBombExplosion);
        if (giveUpButton != null)
            giveUpButton.onClick.RemoveListener(GiveUp);
        if (currencyReviveButton != null)
            currencyReviveButton.onClick.RemoveListener(ReviveWithCurrency);
        if (adReviveButton != null)
            adReviveButton.onClick.RemoveListener(ReviveWithAd);

        StopTransition();
        ResetHidden();
    }

    // Call this from the real wheel's bomb-result logic as well as the test button.
    public void ShowBombExplosion()
    {
        if (!isActiveAndEnabled || IsVisible || overlay == null)
            return;

        IsVisible = true;
        if (triggerButton != null)
        {
            triggerButton.interactable = false;
            triggerButton.gameObject.SetActive(false);
        }
        overlay.gameObject.SetActive(true);
        overlay.alpha = 0f;
        overlay.blocksRaycasts = true;
        overlay.interactable = false;
        content.localScale = Vector3.one * 0.9f;
        content.anchoredPosition = new Vector2(0f, -24f);
        bombCard.localRotation = Quaternion.identity;

        transition = DOTween.Sequence().SetUpdate(true).SetTarget(this);
        transition.Join(overlay.DOFade(1f, openingDuration * 0.75f));
        transition.Join(content.DOScale(1f, openingDuration).SetEase(Ease.OutBack));
        transition.Join(content.DOAnchorPos(Vector2.zero, openingDuration)
            .SetEase(Ease.OutCubic));
        transition.Insert(openingDuration * 0.25f,
            bombCard.DOPunchRotation(new Vector3(0f, 0f, 3f), 0.35f, 6, 0.5f));
        transition.OnComplete(() =>
        {
            transition = null;
            overlay.interactable = true;
        });
    }

    public void Hide()
    {
        if (!IsVisible)
            return;

        StopTransition();
        overlay.interactable = false;
        transition = DOTween.Sequence().SetUpdate(true).SetTarget(this);
        transition.Join(overlay.DOFade(0f, 0.2f));
        transition.Join(content.DOScale(0.96f, 0.2f).SetEase(Ease.InCubic));
        transition.OnComplete(() =>
        {
            transition = null;
            ResetHidden();
        });
    }

    public void CloseImmediately()
    {
        StopTransition();
        ResetHidden();
    }

    private void GiveUp()
    {
        Hide();
        GiveUpRequested?.Invoke();
    }

    private void ReviveWithCurrency()
    {
        Hide();
        CurrencyReviveRequested?.Invoke();
    }

    private void ReviveWithAd()
    {
        Hide();
        AdReviveRequested?.Invoke();
    }

    private void StopTransition()
    {
        transition?.Kill();
        transition = null;
    }

    private void ResetHidden()
    {
        IsVisible = false;
        if (overlay != null)
        {
            overlay.alpha = 0f;
            overlay.interactable = false;
            overlay.blocksRaycasts = false;
            overlay.gameObject.SetActive(false);
        }
        if (content != null)
        {
            content.localScale = Vector3.one;
            content.anchoredPosition = Vector2.zero;
        }
        if (bombCard != null)
            bombCard.localRotation = Quaternion.identity;
        if (triggerButton != null)
        {
            triggerButton.gameObject.SetActive(true);
            triggerButton.interactable = true;
        }
    }
}
