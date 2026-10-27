using UnityEngine;

public class SpinDetector : MonoBehaviour
{
    [SerializeField, Min(1f)] private float detectionRadius = 12f;
    private readonly Collider2D[] hits = new Collider2D[16];

    public SpinSlot Detect(Transform spinWheel)
    {
        // Canvas coordinates are scaled into world coordinates for the physics query.
        float radius = detectionRadius * Mathf.Abs(transform.lossyScale.x);
        Physics2D.SyncTransforms();
        var filter = new ContactFilter2D().NoFilter();
        filter.useTriggers = true;
        int count = Physics2D.OverlapCircle(transform.position, radius, filter, hits);
        SpinSlot winner = null;
        float closestDistance = float.PositiveInfinity;

        for (int i = 0; i < count; i++)
        {
            SpinSlot slot = hits[i].GetComponent<SpinSlot>();
            if (slot == null || !slot.transform.IsChildOf(spinWheel))
                continue;

            float distance = ((Vector2)slot.transform.position - (Vector2)transform.position).sqrMagnitude;
            if (distance < closestDistance)
            {
                closestDistance = distance;
                winner = slot;
            }
        }

        return winner;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius * Mathf.Abs(transform.lossyScale.x));
    }
}
