using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HealthPickup : MonoBehaviour
{
    [Header("Heal")]
    [SerializeField] private float healAmount = 20f;
    [SerializeField] private string playerTag = "Player";

    [Header("Feedback")]
    [SerializeField] private GameObject pickupEffect;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag))
            return;

        if (!other.TryGetComponent(out PlayerHealth playerHealth))
            return;

        if (playerHealth.IsDead)
            return;

        playerHealth.Heal(healAmount);

        //if (pickupEffect != null)
        //    Instantiate(pickupEffect, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
