using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GlobalLightController2D : MonoBehaviour
{
    [Header("Target Light")]
    [SerializeField] private Light2D globalLight;

    [Header("Intensity Limits")]
    [SerializeField] private float minIntensity = 0.15f;
    [SerializeField] private float maxIntensity = 1.0f;

    [Header("Change Settings")]
    [SerializeField] private float defaultIncreaseStep = 0.1f;
    [SerializeField] private bool smoothTransition = true;
    [SerializeField] private float transitionDuration = 0.35f;

    private Coroutine runningTransition;

    private void Awake()
    {
        if (globalLight == null)
        {
            globalLight = GetComponent<Light2D>();
        }

        if (globalLight == null)
        {
            Debug.LogError("GlobalLightController2D: No Light2D assigned.");
            enabled = false;
            return;
        }

        globalLight.intensity = Mathf.Clamp(globalLight.intensity, minIntensity, maxIntensity);
    }

    public void IncreaseIntensity()
    {
        IncreaseIntensity(defaultIncreaseStep);
    }

    public void IncreaseIntensity(float amount)
    {
        if (!enabled || globalLight == null)
        {
            return;
        }

        float targetIntensity = Mathf.Clamp(globalLight.intensity + amount, minIntensity, maxIntensity);

        if (!smoothTransition || transitionDuration <= 0f)
        {
            globalLight.intensity = targetIntensity;
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
        float start = globalLight.intensity;
        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / transitionDuration);
            globalLight.intensity = Mathf.Lerp(start, target, t);
            yield return null;
        }

        globalLight.intensity = target;
        runningTransition = null;
    }
}
