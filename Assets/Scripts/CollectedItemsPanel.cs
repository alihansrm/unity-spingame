using System;
using System.Collections.Generic;
using UnityEngine;

public class CollectedItemsPanel : MonoBehaviour
{
    [SerializeField] private RectTransform content;
    [SerializeField] private CollectedItemRow rowPrefab;
    [SerializeField] private UnityEngine.UI.Button exitButton;
    private readonly Dictionary<string, CollectedItemRow> rows = new Dictionary<string, CollectedItemRow>(StringComparer.Ordinal);

    public int ItemCount => rows.Count;
    public event Action ExitRequested;

    private void OnValidate()
    {
        content = transform.Find("ui_scroll_items/ui_viewport/ui_content_items") as RectTransform;
        exitButton = transform.Find("ui_button_exit")?.GetComponent<UnityEngine.UI.Button>();
    }

    private void Awake()
    {
        // Include example rows already placed in the scene.
        foreach (var row in content.GetComponentsInChildren<CollectedItemRow>())
            if (!string.IsNullOrWhiteSpace(row.ItemId) && !rows.ContainsKey(row.ItemId))
                rows.Add(row.ItemId, row);
    }

    private void OnEnable() => exitButton.onClick.AddListener(RequestExit);
    private void OnDisable() => exitButton.onClick.RemoveListener(RequestExit);
    private void RequestExit() => ExitRequested?.Invoke();

    public void AddItem(string itemId, Sprite icon, long amount)
    {
        if (string.IsNullOrWhiteSpace(itemId)) throw new ArgumentException("Item kimliği gerekli.", nameof(itemId));
        if (icon == null) throw new ArgumentNullException(nameof(icon));
        if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));

        if (rows.TryGetValue(itemId, out var row))
        {
            row.Add(amount);
            return;
        }

        row = Instantiate(rowPrefab, content);
        row.name = "ui_item_" + itemId;
        row.Bind(itemId, icon, amount);
        rows.Add(itemId, row);
    }

    public long GetAmount(string itemId) => rows.TryGetValue(itemId, out var row) ? row.Amount : 0;

    public void ClearItems()
    {
        foreach (var row in rows.Values)
        {
            row.gameObject.SetActive(false);
            Destroy(row.gameObject);
        }
        rows.Clear();
    }
}
