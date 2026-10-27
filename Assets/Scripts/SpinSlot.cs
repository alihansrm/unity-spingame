using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class SpinSlot : MonoBehaviour
{
    [SerializeField, Range(0, 7)] private int index;

    public int Index => index;
}
