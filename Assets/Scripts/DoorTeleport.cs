using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorTeleport : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;
    [SerializeField] private GameObject tutorialObject; // Drag your "Press E" sprite/text here
    [SerializeField] private GameObject lockedTutorialObject; // Optional: shown when door is locked
    [SerializeField] private string spawnPointName; // Name of the entrance in the NEXT scene
    [Header("Lock Settings")]
    [SerializeField] private bool requireAllIslandsComplete = false;
    
    private bool isPlayerInRange = false;


    private void Start()
    {
        // Make sure the tutorial is hidden when the game starts
        if (tutorialObject != null)
        {
            tutorialObject.SetActive(false);
        }

        if (lockedTutorialObject != null)
        {
            lockedTutorialObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (isPlayerInRange)
        {
            UpdatePromptState();
        }

        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (IsDoorLocked())
            {
                Debug.Log($"{name}: Final island door is locked until all 3 islands are complete.");
                return;
            }

            PlayerPrefs.SetString("LastExitName", spawnPointName);
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            UpdatePromptState();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (tutorialObject != null) tutorialObject.SetActive(false);
            if (lockedTutorialObject != null) lockedTutorialObject.SetActive(false);
        }
    }

    private bool IsDoorLocked()
    {
        if (!requireAllIslandsComplete)
        {
            return false;
        }

        if (GameManager.Instance == null)
        {
            return true;
        }

        return !GameManager.Instance.AreAllStarsComplete();
    }

    private void UpdatePromptState()
    {
        bool isLocked = IsDoorLocked();

        if (tutorialObject != null)
        {
            tutorialObject.SetActive(!isLocked);
        }

        if (lockedTutorialObject != null)
        {
            lockedTutorialObject.SetActive(isLocked);
        }
    }
}