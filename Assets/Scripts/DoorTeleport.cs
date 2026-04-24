using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorTeleport : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;
    [SerializeField] private GameObject tutorialObject; // Drag your "Press E" sprite/text here
    [SerializeField] private string spawnPointName; // Name of the entrance in the NEXT scene
    
    private bool isPlayerInRange = false;


    private void Start()
    {
        // Make sure the tutorial is hidden when the game starts
        if (tutorialObject != null)
        {
            tutorialObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            PlayerPrefs.SetString("LastExitName", spawnPointName);
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (tutorialObject != null) tutorialObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (tutorialObject != null) tutorialObject.SetActive(false);
        }
    }
}