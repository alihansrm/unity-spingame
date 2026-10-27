using DG.Tweening;
using TMPro;
using UnityEngine;

public class ZoneBarController : MonoBehaviour
{
    private const int VisibleCellCount = 13;
    private const int CenterIndex = VisibleCellCount / 2;

    [SerializeField, Min(1)] private int currentZone = 11;
    [SerializeField, Min(0.05f)] private float transitionDuration = 0.3f;
    [SerializeField, Min(1f)] private float cellWidth = 76f;
    [SerializeField] private RectTransform numbersStrip;
    [SerializeField] private TMP_Text[] zoneLabels;
    [SerializeField] private TMP_Text currentZoneLabel;

    private Tween transition;
    private bool initialized;
    private System.Action transitionCompleted;

    public int CurrentZone => currentZone;
    public bool IsAnimating => transition != null;

    private void OnValidate()
    {
        currentZone = Mathf.Max(1, currentZone);
        transitionDuration = Mathf.Max(0.05f, transitionDuration);
        cellWidth = Mathf.Max(1f, cellWidth);
        ResolveReferences();
    }

    private void ResolveReferences()
    {
        numbersStrip = transform.Find("ui_zone_viewport/ui_zone_numbers")
            as RectTransform;
        if (numbersStrip != null)
            zoneLabels = numbersStrip.GetComponentsInChildren<TMP_Text>(true);

        currentZoneLabel = transform
            .Find("ui_image_current_zone/ui_text_current_zone_value")
            ?.GetComponent<TMP_Text>();

    }

    private void Awake()
    {
        ResolveReferences();
        if (numbersStrip == null || currentZoneLabel == null ||
            zoneLabels == null || zoneLabels.Length != VisibleCellCount + 1)
        {
            Debug.LogError("Zone bar needs a strip with 14 labels and a current label.", this);
            enabled = false;
        }
        initialized = enabled;
    }

    private void OnDisable()
    {
        CancelTransition();
        if (initialized)
            RefreshLabels();
    }

    private void Start()
    {
        initialized = true;
        SetZone(currentZone);
    }

    public void SetZone(int zone)
    {
        CancelTransition();
        currentZone = Mathf.Max(1, zone);
        RefreshLabels();
    }

    public void AdvanceZone(System.Action onCompleted = null)
    {
        // Ignore repeated requests while the strip is already moving.
        if (!initialized || !isActiveAndEnabled || IsAnimating)
            return;

        transitionCompleted = onCompleted;
        transition = DOTween.To(
                () => numbersStrip.anchoredPosition,
                position => numbersStrip.anchoredPosition = position,
                new Vector2(-cellWidth, 0f),
                transitionDuration)
            .SetEase(Ease.InOutSine)
            .SetTarget(this)
            .OnComplete(CompleteTransition);
    }

    private void CompleteTransition()
    {
        transition = null;
        currentZone++;

        // Rebind and reset together so the visible numbers do not jump.
        numbersStrip.anchoredPosition = Vector2.zero;
        RefreshLabels();
        var callback = transitionCompleted;
        transitionCompleted = null;
        callback?.Invoke();
    }

    private void CancelTransition()
    {
        transition?.Kill();
        transition = null;
        if (numbersStrip != null)
            numbersStrip.anchoredPosition = Vector2.zero;
        transitionCompleted = null;
    }

    private void RefreshLabels()
    {
        currentZoneLabel.text = currentZone.ToString();

        for (int i = 0; i < zoneLabels.Length; i++)
        {
            int displayedZone = currentZone + i - CenterIndex;
            TMP_Text label = zoneLabels[i];

            if (displayedZone < 1)
            {
                label.text = "";
                continue;
            }

            label.text = displayedZone.ToString();
            label.color = GetZoneColor(displayedZone);
        }
    }

    private Color GetZoneColor(int zone)
    {
        bool isPast = zone < currentZone;

        if (zone % 30 == 0)
            return isPast
                ? new Color32(130, 100, 30, 255)
                : new Color32(255, 190, 30, 255);

        if (zone % 5 == 0)
            return isPast
                ? new Color32(56, 107, 19, 255)
                : new Color32(118, 230, 0, 255);

        return isPast
            ? new Color32(112, 112, 112, 255)
            : Color.white;
    }
}
