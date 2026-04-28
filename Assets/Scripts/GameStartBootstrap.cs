using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameStartBootstrap
{
    private const string StartSceneName = "Central_island";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void EnsureStartSceneLoaded()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        if (string.Equals(activeScene.name, StartSceneName, StringComparison.Ordinal))
        {
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(StartSceneName))
        {
            Debug.LogWarning($"Start scene '{StartSceneName}' is not in Build Settings.");
            return;
        }

        SceneManager.LoadScene(StartSceneName);
    }
}
