using UnityEngine;

[RequireComponent(typeof(Collider))]
public class LevelEndTrigger : MonoBehaviour
{
    [Header("Trigger")]
    [SerializeField] private string playerTag = "Player";

    [Header("UI")]
    [SerializeField] private GameObject endScreenPanel;

    private bool hasTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered || !other.CompareTag(playerTag))
            return;

        hasTriggered = true;

        Time.timeScale = 0f;

        if (endScreenPanel != null)
            endScreenPanel.SetActive(true);
    }
}
