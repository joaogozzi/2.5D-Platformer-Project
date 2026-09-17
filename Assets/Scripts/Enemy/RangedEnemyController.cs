using UnityEngine;

/// <summary>
/// Inimigo à distância: mantém uma distância preferida do player e atira projéteis.
/// Não patrulha (fica parado até detectar o player), mas isso poderia ser trocado
/// facilmente sobrescrevendo Patrol().
/// </summary>
public class RangedEnemyController : EnemyController
{
    [Header("Ranged Attack")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float projectileSpeed = 8f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float preferredDistance = 4f;

    private float attackTimer;

    protected override void Update()
    {
        base.Update();

        if (attackTimer > 0f)
            attackTimer -= Time.deltaTime;
    }

    protected override void Patrol()
    {
        Move(0f);
    }

    protected override void Chase()
    {
        if (player == null)
            return;

        float distance = GetDistanceToPlayer();
        float direction = player.position.x > transform.position.x ? 1f : -1f;

        // Se estiver muito perto, recua para manter distância de tiro.
        if (distance < preferredDistance)
            Move(-direction);
        else
            Move(direction);
    }

    protected override void Attack()
    {
        Move(0f);

        if (attackTimer > 0f)
            return;

        attackTimer = attackCooldown;

        FireProjectile();

        //if (animator != null)
        //    animator.SetTrigger("Attack");
    }

    private void FireProjectile()
    {
        if (projectilePrefab == null || firePoint == null || player == null)
            return;

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        Vector3 direction = (player.position - firePoint.position).normalized;

        if (projectile.TryGetComponent(out Rigidbody projectileRb))
            projectileRb.linearVelocity = direction * projectileSpeed;
    }
}