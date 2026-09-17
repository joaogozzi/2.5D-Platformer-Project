using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EnemyDetectionZone : MonoBehaviour
{
    public enum ZoneType
    {
        Detection,
        Attack
    }

    [SerializeField] private ZoneType zoneType = ZoneType.Detection;
    [SerializeField] private EnemyController enemyController;
    [SerializeField] private string playerTag = "Player";

    private void Awake()
    {
        if (enemyController == null)
            enemyController = GetComponentInParent<EnemyController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (enemyController == null || !other.CompareTag(playerTag))
            return;

        if (zoneType == ZoneType.Detection)
            enemyController.SetPlayerReference(other.transform);
        else
            enemyController.SetPlayerInAttackRange(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (enemyController == null || !other.CompareTag(playerTag))
            return;

        if (zoneType == ZoneType.Detection)
            enemyController.ClearPlayerReference();
        else
            enemyController.SetPlayerInAttackRange(false);
    }
}
