using UnityEngine;

public class PlayerSpawnHandler : MonoBehaviour
{
    void Start()
    {
        string targetName = PlayerPrefs.GetString("LastExitName");

        if (!string.IsNullOrEmpty(targetName))
        {
            // Find the object in the scene with that name
            GameObject spawnPoint = GameObject.Find(targetName);

            if (spawnPoint != null)
            {
                // Move player to that spot
                transform.position = spawnPoint.transform.position;
            }
        }
    }
}