using UnityEngine;

/// <summary>
/// Coloque este script num GameObject filho posicionado nos pés do player,
/// com um Collider pequeno marcado como "Is Trigger" (cobrindo só a base dos pés).
/// Quando esse trigger encosta num inimigo, ele é morto instantaneamente e o
/// player recebe um pequeno impulso para cima (igual ao clássico "pulo na cabeça").
/// </summary>
[RequireComponent(typeof(Collider))]
public class PlayerStompDetector : MonoBehaviour
{
    [Header("Stomp")]
    [SerializeField] private string enemyTag = "Enemy";
    [SerializeField] private PlayerCharacterController playerController;

    private void Awake()
    {
        if (playerController == null)
            playerController = GetComponentInParent<PlayerCharacterController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(enemyTag))
            return;

        if (!other.TryGetComponent(out EnemyController enemy))
            return;

        enemy.Kill();

        if (playerController != null)
            playerController.PerformStompBounce();
    }
}
