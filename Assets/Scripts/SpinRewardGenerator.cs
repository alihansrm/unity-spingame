using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public readonly struct SpinRewardResult
{
    public string ItemId { get; }
    public string DisplayName { get; }
    public Sprite Icon { get; }
    public int Amount { get; }
    public bool IsBomb { get; }

    public SpinRewardResult(string itemId, string displayName, Sprite icon, int amount, bool isBomb = false)
    {
        ItemId = itemId;
        DisplayName = displayName;
        Icon = icon;
        Amount = amount;
        IsBomb = isBomb;
    }
}

public static class SpinRewardGenerator
{
    public const int SlotCount = 8;

    public static SpinRewardResult[] Generate(SpinRewardPool pool)
    {
        if (pool == null) throw new ArgumentNullException(nameof(pool));
        int rewardCount = pool.HasBomb ? SlotCount - 1 : SlotCount;
        if (pool.rewards == null || pool.rewards.Length < rewardCount)
            throw new InvalidOperationException($"{pool.name}: En az {rewardCount} farklı ödül gerekli.");
        if (pool.wheelSprite == null || (pool.HasBomb && pool.bombSprite == null))
            throw new InvalidOperationException($"{pool.name}: Çark veya bomba sprite eksik.");

        var ids = new HashSet<string>();
        var candidates = new List<SpinRewardOption>(pool.rewards.Length);
        foreach (var option in pool.rewards)
        {
            if (option == null || string.IsNullOrWhiteSpace(option.itemId) || !ids.Add(option.itemId)
                || option.icon == null || option.possibleAmounts == null || option.possibleAmounts.Length == 0)
                throw new InvalidOperationException($"{pool.name}: Eksik veya tekrarlanan ödül tanımı.");
            foreach (int amount in option.possibleAmounts)
                if (amount < 1) throw new InvalidOperationException("Ödül miktarı pozitif olmalı.");
            candidates.Add(option);
        }

        var results = new SpinRewardResult[SlotCount];
        for (int i = 0; i < rewardCount; i++)
        {
            // Each remaining item has an equal chance; remove it to prevent duplicates.
            int index = Random.Range(0, candidates.Count);
            var option = candidates[index];
            candidates.RemoveAt(index);
            int amount = option.possibleAmounts[Random.Range(0, option.possibleAmounts.Length)];
            results[i] = new SpinRewardResult(option.itemId, option.displayName, option.icon, amount);
        }

        if (pool.HasBomb)
            results[SlotCount - 1] = new SpinRewardResult("bomb", "Bomba", pool.bombSprite, 0, true);

        // Shuffle all slots so the bomb and rewards can appear anywhere.
        for (int i = results.Length - 1; i > 0; i--)
        {
            int index = Random.Range(0, i + 1);
            var temporary = results[i];
            results[i] = results[index];
            results[index] = temporary;
        }
        return results;
    }
}
