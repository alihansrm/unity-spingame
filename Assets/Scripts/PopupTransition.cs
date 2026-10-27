using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class PopupTransition : MonoBehaviour
{
    [SerializeField] private CanvasGroup group;
    [SerializeField] private RectTransform content;
    private Sequence transition;

    private void OnValidate()
    {
        group = GetComponent<CanvasGroup>();
        content = transform.Find("ui_animator_content") as RectTransform;
    }

    public void Show()
    {
        transition?.Kill();
        gameObject.SetActive(true);
        group.alpha = 0;
        group.blocksRaycasts = true;
        group.interactable = false;
        content.localScale = Vector3.one * .92f;
        transition = DOTween.Sequence().SetUpdate(true).SetTarget(this);
        transition.Join(group.DOFade(1, .2f));
        transition.Join(content.DOScale(1, .3f).SetEase(Ease.OutBack));
        transition.OnComplete(() => { transition = null; group.interactable = true; });
    }

    public void Hide()
    {
        transition?.Kill();
        transition = null;
        group.alpha = 0;
        group.blocksRaycasts = false;
        group.interactable = false;
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        transition?.Kill();
        transition = null;
    }
}
