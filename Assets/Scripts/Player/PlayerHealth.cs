using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    #region Variables

    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    [Header("Invulnerability")]
    [SerializeField] private float invulnerabilityDuration = 1f;

    [Header("Components")]
    [SerializeField] private Animator animator;

    private bool isInvulnerable;
    private bool isDead;

    public event Action<float, float> OnHealthChanged; // (CurrentHealth, MaxHealth)
    public event Action OnDeath;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsDead => isDead;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    #endregion

    #region Damage

    public void TakeDamage(float amount)
    {
        if (isDead || isInvulnerable)
            return;

        currentHealth = Mathf.Max(currentHealth - amount, 0f);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0f)
        {
            Die();
            return;
        }

        StartInvulnerability();

        //if (animator != null)
        //    animator.SetTrigger("Hurt");
    }

    private void StartInvulnerability()
    {
        isInvulnerable = true;

        CancelInvoke(nameof(EndInvulnerability));
        Invoke(nameof(EndInvulnerability), invulnerabilityDuration);
    }

    private void EndInvulnerability()
    {
        isInvulnerable = false;
    }

    #endregion

    #region Heal

    public void Heal(float amount)
    {
        if (isDead)
            return;

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    #endregion

    #region Death

    private void Die()
    {
        isDead = true;

        CancelInvoke();

        //if (animator != null)
        //    animator.SetTrigger("Death");

        OnDeath?.Invoke();
    }

    #endregion
}