using System;
using UnityEngine;

public class CollectedItemRow : MonoBehaviour
{
    [SerializeField] private string itemId;
    [SerializeField] private long amount;
    [SerializeField] private UnityEngine.UI.Image icon;
    [SerializeField] private TMPro.TextMeshProUGUI amountText;

    public string ItemId => itemId;
    public long Amount => amount;

    private void OnValidate()
    {
        icon = transform.Find("ui_image_item_value")?.GetComponent<UnityEngine.UI.Image>();
        amountText = transform.Find("ui_text_amount_value")?.GetComponent<TMPro.TextMeshProUGUI>();
    }

    public void Bind(string id, Sprite sprite, long quantity)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Item kimliği gerekli.", nameof(id));
        if (sprite == null) throw new ArgumentNullException(nameof(sprite));
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));
        itemId = id;
        icon.sprite = sprite;
        SetAmount(quantity);
    }

    public void Add(long quantity)
    {
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));
        SetAmount(checked(amount + quantity));
    }

    private void SetAmount(long value)
    {
        amount = value;
        amountText.text = amount.ToString("N0", System.Globalization.CultureInfo.InvariantCulture);
    }
}
