using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class LightIntensityTrigger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GlobalLightController lightController;

    [Header("Trigger Settings")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float increaseAmount = 0.1f;
    [SerializeField] private bool triggerOnlyOnce = true;
    [SerializeField] private bool disableAfterTrigger = true;

    private bool hasTriggered;

    private void Reset()
    {
        Collider2D col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void Awake()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (!col.isTrigger)
        {
            col.isTrigger = true;
        }

        if (lightController == null)
        {
            lightController = FindObjectOfType<GlobalLightController>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"LightIntensityTrigger touched by: {other.name}");

        if (triggerOnlyOnce && hasTriggered)
        {
            return;
        }

        if (!HasTagInHierarchy(other.transform, playerTag))
        {
            return;
        }

        if (lightController == null)
        {
            Debug.LogWarning("LightIntensityTrigger: No GlobalLightController found in scene.");
            return;
        }

        float before = lightController.CurrentIntensity;
        lightController.IncreaseIntensity(increaseAmount);
        float after = lightController.CurrentIntensity;
        Debug.Log($"LightIntensityTrigger applied increase. Before: {before:F2}, After: {after:F2}, Delta: {(after - before):F2}");
        hasTriggered = true;

        if (disableAfterTrigger)
        {
            gameObject.SetActive(false);
        }
    }

    private static bool HasTagInHierarchy(Transform start, string tag)
    {
        if (start == null || string.IsNullOrEmpty(tag))
        {
            return false;
        }

        Transform current = start;
        while (current != null)
        {
            if (current.CompareTag(tag))
            {
                return true;
            }

            current = current.parent;
        }

        return false;
    }
}
