using UnityEngine;
using UnityEngine.UI;

public class GameplayInterface : MonoBehaviour
{
    [Header("Player Health")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private PlayerHealth playerHealth;

    private void Start()
    {
        playerHealth.OnHealthChanged += UpdateHealth;
    }

    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        float value = currentHealth / maxHealth;

        healthSlider.value = value;
    }

    private void OnDestroy()
    {
        playerHealth.OnHealthChanged -= UpdateHealth;
    }
}