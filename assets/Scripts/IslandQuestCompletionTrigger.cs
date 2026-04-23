using UnityEngine;

public class IslandQuestCompletionTrigger : MonoBehaviour
{
    [Header("Completion")]
    [SerializeField] private int islandIndex = 0;
    [SerializeField] private bool triggerOnce = true;

    [Header("Trigger Settings")]
    [SerializeField] private bool completeOnTriggerEnter = true;
    [SerializeField] private string requiredTag = "Player";

    private bool hasCompleted;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!completeOnTriggerEnter)
        {
            return;
        }

        if (!string.IsNullOrEmpty(requiredTag) && !other.CompareTag(requiredTag))
        {
            return;
        }

        CompleteQuest();
    }

    public void CompleteQuest()
    {
        if (triggerOnce && hasCompleted)
        {
            return;
        }

        if (GameManager.Instance == null)
        {
            Debug.LogWarning($"{name}: GameManager.Instance is missing. Cannot mark island completion.");
            return;
        }

        GameManager.Instance.CompleteStar(islandIndex);
        hasCompleted = true;
    }
}
