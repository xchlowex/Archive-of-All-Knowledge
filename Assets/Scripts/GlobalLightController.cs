using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GlobalLightController : MonoBehaviour
{
    [Header("Target Light")]
    [SerializeField] private Light2D globalLight;

    [Header("Debug")]
    [SerializeField] private bool debugLogs = true;

    [Header("Intensity Limits")]
    [SerializeField] private float minIntensity = 0.15f;
    [SerializeField] private float maxIntensity = 1.0f;

    [Header("Change Settings")]
    [SerializeField] private float defaultIncreaseStep = 0.1f;

    public float CurrentIntensity => globalLight != null ? globalLight.intensity : -1f;

    private void Awake()
    {
        ResolveLightReference();

        if (globalLight == null)
        {
            return;
        }

        if (maxIntensity < minIntensity)
        {
            Debug.LogWarning($"GlobalLightController: maxIntensity ({maxIntensity:F2}) is lower than minIntensity ({minIntensity:F2}). Swapping values.");
            float temp = minIntensity;
            minIntensity = maxIntensity;
            maxIntensity = temp;
        }

        globalLight.intensity = Mathf.Clamp(globalLight.intensity, minIntensity, maxIntensity);

        if (debugLogs)
        {
            Debug.Log($"GlobalLightController ready on '{name}'. Target Light: '{globalLight.name}', intensity: {globalLight.intensity:F2}, min: {minIntensity:F2}, max: {maxIntensity:F2}");
        }
    }

    public void IncreaseIntensity()
    {
        IncreaseIntensity(defaultIncreaseStep);
    }

    public void IncreaseIntensity(float amount)
    {
        if (!enabled || globalLight == null)
        {
            if (debugLogs)
            {
                Debug.LogWarning("GlobalLightController: IncreaseIntensity was called but controller is disabled or target light is missing.");
            }
            return;
        }

        if (amount <= 0f)
        {
            Debug.LogWarning($"GlobalLightController: Increase amount is {amount:F2}. It should be > 0 to brighten the scene.");
        }

        float currentIntensity = globalLight.intensity;
        float targetIntensity = Mathf.Clamp(currentIntensity + amount, minIntensity, maxIntensity);

        if (debugLogs)
        {
            Debug.Log($"GlobalLightController IncreaseIntensity called. Current: {currentIntensity:F2}, Amount: {amount:F2}, Target: {targetIntensity:F2}");
            if (Mathf.Approximately(currentIntensity, targetIntensity))
            {
                Debug.Log("GlobalLightController: Intensity did not change (likely already at clamp limit).");
            }
        }

        globalLight.intensity = targetIntensity;

        if (debugLogs)
        {
            Debug.Log($"GlobalLightController applied intensity immediately. New intensity: {globalLight.intensity:F2}");
        }
    }

    private void ResolveLightReference()
    {
        if (globalLight != null)
        {
            return;
        }

        globalLight = GetComponent<Light2D>();

        if (globalLight == null)
        {
            globalLight = GetComponentInChildren<Light2D>();
        }

        if (globalLight == null)
        {
            globalLight = GetComponentInParent<Light2D>();
        }

        if (globalLight == null)
        {
            Debug.LogError("GlobalLightController: No Light2D assigned or found on this object, its children, or its parent.");
            enabled = false;
        }
    }
}
