using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class CollectedReward
{
    public string ItemId { get; }
    public string DisplayName { get; }
    public Sprite Icon { get; }
    public long Amount { get; private set; }

    public CollectedReward(SpinRewardResult reward)
    {
        ItemId = reward.ItemId;
        DisplayName = reward.DisplayName;
        Icon = reward.Icon;
        Amount = reward.Amount;
    }

    public void Add(int amount) => Amount = checked(Amount + amount);
}

// All run data lives in memory. Returning to the menu discards the run.
public sealed class RunSession
{
    private readonly Dictionary<string, CollectedReward> rewards = new Dictionary<string, CollectedReward>(StringComparer.Ordinal);
    public int Zone { get; private set; } = 1;
    public IReadOnlyCollection<CollectedReward> Rewards => rewards.Values;

    public static SpinType GetSpinType(int zone) => zone % 30 == 0 ? SpinType.Golden : zone % 5 == 0 ? SpinType.Silver : SpinType.Bronze;
    public void Advance() => Zone++;

    public void Award(SpinRewardResult reward)
    {
        if (reward.IsBomb) throw new InvalidOperationException("Bomba ödül olarak eklenemez.");
        if (rewards.TryGetValue(reward.ItemId, out var existing)) existing.Add(reward.Amount);
        else rewards.Add(reward.ItemId, new CollectedReward(reward));
    }

    public void Reset()
    {
        Zone = 1;
        rewards.Clear();
    }
}
