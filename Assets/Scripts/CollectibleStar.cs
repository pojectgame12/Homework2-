using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CollectibleStar : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        StarCounter.Collected++;
        Destroy(gameObject);
    }
}
