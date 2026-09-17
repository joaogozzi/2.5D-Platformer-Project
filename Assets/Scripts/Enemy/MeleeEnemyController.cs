using UnityEngine;

public class MeleeEnemyController : EnemyController
{
    [Header("Melee Attack")]
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float attackDamage = 10f;

    private float attackTimer;
    private int patrolDirection = 1;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Update()
    {
        base.Update();

        if (attackTimer > 0f)
            attackTimer -= Time.deltaTime;
    }

    protected override void Patrol()
    {
        float distanceFromSpawn = transform.position.x - spawnPosition.x;

        if (Mathf.Abs(distanceFromSpawn) >= patrolDistance)
        {
            int boundaryDirection = distanceFromSpawn > 0f ? -1 : 1;

            if (patrolDirection != boundaryDirection)
            {
                patrolDirection = boundaryDirection;
            }
        }

        if (IsEdgeAhead(patrolDirection))
        {
            patrolDirection *= -1;
        }

        Move(patrolDirection);
    }

    protected override void Chase()
    {
        if (player == null)
            return;

        float direction = player.position.x > transform.position.x ? 1f : -1f;

        Move(direction);
    }

    protected override void Attack()
    {
        Move(0f);

        if (attackTimer > 0f)
            return;

        attackTimer = attackCooldown;

        if (player != null && player.TryGetComponent(out IDamageable playerHealth))
            playerHealth.TakeDamage(attackDamage);

        //if (animator != null)
        //    animator.SetTrigger("Attack");
    }
}