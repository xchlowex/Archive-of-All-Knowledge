using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class RoboticsGameManager : MonoBehaviour
{
    public static RoboticsGameManager Instance;
    public static bool CompleteChallenge = false;

    [Header("Game State")]
    public float gameTimer = 30f; // 30 second survival
    private bool _isGameActive = false;
    private float _currentTime;

    [Header("Spawning")]
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public float spawnRate = 2f;
    private float _spawnTimer;

    [Header("UI References")]

    public TextMeshProUGUI timerText;
    public GameObject victoryPanel;
    public GameObject gameOverPanel;
    // public GameStarter gameStarter; // Assign in Inspector

    // void Awake() { Instance = this; }

    void Awake() 
    {
        // Check if an instance already exists to avoid duplicates
        if (Instance == null) 
        {
            Instance = this;
        } 
        else 
        {
            Destroy(gameObject);
        }
    }
    public void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        // startGame
        // Reset everything for a fresh start
        _isGameActive = true;
        _currentTime = gameTimer;
        _spawnTimer = 0;

        
        victoryPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        Time.timeScale = 1; // Ensure game isn't paused
        
        Debug.Log("Robotics Survival Started!");
    }

    void Update()
    {
        // if (!_isGameActive) return;

        // 1. Timer Logic
        _currentTime -= Time.deltaTime;
        timerText.text = "Time Remaining: " + Mathf.CeilToInt(_currentTime);

        if (_currentTime <= 0)
        {
            EndGame(true);
        }

        // 2. Spawning Logic
        _spawnTimer += Time.deltaTime;
        if (_spawnTimer >= spawnRate)
        {
            SpawnEnemy();
            _spawnTimer = 0;
        }
                // game over restart logic
        GameObject playerObj = GameObject.FindWithTag("Player");
        int playerHealth = playerObj.GetComponent<Health>().currentHealth;
        if (playerHealth <= 0)
        {
            Debug.Log("Waiting for Restart Input R...");
            EndGame(false);
            if (Input.GetKeyDown(KeyCode.R))
            {                
                RestartGame();
            }
        }
    }
    void RestartGame()
    {
        // Use SceneManager to completely fresh-start the level
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
        
        // CRITICAL: If you paused time in EndGame, you MUST unpause it!
        Time.timeScale = 1f;    
    }

    void SpawnEnemy()
    {
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Vector3 pos = spawnPoints[randomIndex].position;
        Instantiate(enemyPrefab, new Vector3(pos.x, pos.y, 0), Quaternion.identity);
    }
    public void EndGame(bool didWin)
    {
        _isGameActive = false;
        Time.timeScale = 0;
        
        if (!didWin)
        {
            gameOverPanel.SetActive(true);
            // if keyboard input "R" is pressed, then restart the game
        }

        
        
        if (didWin)
        {
            // Safe way to check for the NPC script
            // StartRoboticsChallenge npc = FindObjectOfType<StartRoboticsChallenge>();
            // if (npc != null) npc.MarkChallengeAsDone();
            CompleteChallenge = true;
            if (victoryPanel != null) victoryPanel.SetActive(true);

            Invoke("LoadIslandScene", 10f);

        }
         
        
        else if (gameOverPanel != null) 
        {
            gameOverPanel.SetActive(true);
        
            // LoadIslandScene();
        }
        // if (gameStarter != null)
        // {
        //     gameStarter.ResetStarter();
        // }
    }

    public void LoadIslandScene()
    {
        // Placeholder for scene loading logic
        Time.timeScale = 1; // Crucial: Reset time before switching!
        SceneManager.LoadScene("RoboticsIsland");
        SceneManager.UnloadSceneAsync("RoboticsChallenge");
        Debug.Log("Loading Island Scene...");
        // Example: SceneManager.LoadScene("IslandScene");
    }

}