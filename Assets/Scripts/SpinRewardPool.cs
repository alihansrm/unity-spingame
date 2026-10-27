using System;
using UnityEngine;

public enum SpinType { Bronze, Silver, Golden }

[Serializable]
public class SpinRewardOption
{
    public string itemId;
    public string displayName;
    public Sprite icon;
    public int[] possibleAmounts;
}

[CreateAssetMenu(menuName = "Spin Game/Reward Pool")]
public class SpinRewardPool : ScriptableObject
{
    public SpinType spinType;
    public Sprite wheelSprite;
    public Sprite indicatorSprite;
    public Sprite bombSprite;
    public SpinRewardOption[] rewards;

    public bool HasBomb => spinType == SpinType.Bronze;
}
