using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SpikeTrap : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private float damage = 10f;
    [SerializeField] private string playerTag = "Player";

    [Header("Damage Cooldown")]
    [SerializeField] private float damageCooldown = 0.5f;

    private float cooldownTimer;

    private void Update()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        TryDamage(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryDamage(other);
    }

    private void TryDamage(Collider other)
    {
        if (cooldownTimer > 0f)
            return;

        if (!other.CompareTag(playerTag))
            return;

        if (!other.TryGetComponent(out IDamageable playerHealth))
            return;

        playerHealth.TakeDamage(damage);

        cooldownTimer = damageCooldown;
    }
}
