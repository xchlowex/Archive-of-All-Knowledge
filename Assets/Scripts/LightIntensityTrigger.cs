using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class LightIntensityTrigger2D : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GlobalLightController2D lightController;

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
        if (lightController == null)
        {
            lightController = FindObjectOfType<GlobalLightController2D>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggerOnlyOnce && hasTriggered)
        {
            return;
        }

        if (!other.CompareTag(playerTag))
        {
            return;
        }

        if (lightController == null)
        {
            Debug.LogWarning("LightIntensityTrigger2D: No GlobalLightController2D found in scene.");
            return;
        }

        lightController.IncreaseIntensity(increaseAmount);
        hasTriggered = true;

        if (disableAfterTrigger)
        {
            gameObject.SetActive(false);
        }
    }
}
