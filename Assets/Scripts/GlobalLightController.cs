using UnityEngine;
#if UNITY_RENDER_PIPELINE_UNIVERSAL
using UnityEngine.Rendering.Universal;
#endif

public class GlobalLightController : MonoBehaviour
{
    [Header("Target Light")]
#if UNITY_RENDER_PIPELINE_UNIVERSAL
    [SerializeField] private Light2D globalLight;
#else
    [SerializeField] private Light globalLight;
#endif

    [Header("Debug")]
    [SerializeField] private bool debugLogs = true;

    [Header("Intensity Limits")]
    [SerializeField] private float minIntensity = 0.15f;
    [SerializeField] private float maxIntensity = 1.0f;

    [Header("Change Settings")]
    [SerializeField] private float defaultIncreaseStep = 0.1f;
    [SerializeField] private bool smoothTransition = true;
    [SerializeField] private float transitionDuration = 0.35f;

    private Coroutine runningTransition;

    public float CurrentIntensity => GetCurrentIntensity();

    private void Awake()
    {
        if (globalLight == null)
        {
            globalLight = GetComponent<
#if UNITY_RENDER_PIPELINE_UNIVERSAL
                Light2D
#else
                Light
#endif
            >();
        }

        if (globalLight == null)
        {
            Debug.LogError("GlobalLightController: No compatible light assigned on this object.");
            enabled = false;
            return;
        }

        if (maxIntensity < minIntensity)
        {
            Debug.LogWarning($"GlobalLightController: maxIntensity ({maxIntensity:F2}) is lower than minIntensity ({minIntensity:F2}). Swapping values.");
            float temp = minIntensity;
            minIntensity = maxIntensity;
            maxIntensity = temp;
        }

        SetIntensity(Mathf.Clamp(GetCurrentIntensity(), minIntensity, maxIntensity));

        if (debugLogs)
        {
            Debug.Log($"GlobalLightController ready on '{name}'. Target Light: '{globalLight.name}', intensity: {GetCurrentIntensity():F2}, min: {minIntensity:F2}, max: {maxIntensity:F2}");
#if !UNITY_RENDER_PIPELINE_UNIVERSAL
            Debug.LogWarning("GlobalLightController: URP is not active in this project. Using UnityEngine.Light fallback instead of Light2D.");
#endif
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

        float currentIntensity = GetCurrentIntensity();
        float targetIntensity = Mathf.Clamp(currentIntensity + amount, minIntensity, maxIntensity);

        if (debugLogs)
        {
            Debug.Log($"GlobalLightController IncreaseIntensity called. Current: {currentIntensity:F2}, Amount: {amount:F2}, Target: {targetIntensity:F2}");
            if (Mathf.Approximately(currentIntensity, targetIntensity))
            {
                Debug.Log("GlobalLightController: Intensity did not change (likely already at clamp limit).");
            }
        }

        if (!smoothTransition || transitionDuration <= 0f)
        {
            SetIntensity(targetIntensity);
            return;
        }

        if (runningTransition != null)
        {
            StopCoroutine(runningTransition);
        }

        runningTransition = StartCoroutine(AnimateIntensity(targetIntensity));
    }

    private System.Collections.IEnumerator AnimateIntensity(float target)
    {
        float start = GetCurrentIntensity();
        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / transitionDuration);
            SetIntensity(Mathf.Lerp(start, target, t));
            yield return null;
        }

        SetIntensity(target);
        runningTransition = null;
    }

    private float GetCurrentIntensity()
    {
        return globalLight != null ? globalLight.intensity : -1f;
    }

    private void SetIntensity(float value)
    {
        if (globalLight != null)
        {
            globalLight.intensity = value;
        }
    }
}
