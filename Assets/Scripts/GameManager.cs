using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Game State")]
    [SerializeField] private int humanityScore = 0;
    [SerializeField] private int[] starCompletion = new int[3]; // 0=incomplete, 1=complete
    
    public static GameManager Instance { get; private set; }
    
    // Events for UI updates
    public System.Action<int> OnHumanityChanged;
    public System.Action<int, bool> OnStarCompleted;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void ModifyHumanity(int amount)
    {
        humanityScore += amount;
        Debug.Log($"Humanity Score: {humanityScore}");
        OnHumanityChanged?.Invoke(humanityScore);
    }
    
    public void CompleteStar(int islandIndex)
    {
        if (islandIndex >= 0 && islandIndex < starCompletion.Length)
        {
            if (starCompletion[islandIndex] == 1)
            {
                return;
            }

            starCompletion[islandIndex] = 1;
            OnStarCompleted?.Invoke(islandIndex, true);
            
            // Check if all stars are lit
            bool allComplete = true;
            foreach (int star in starCompletion)
            {
                if (star == 0) allComplete = false;
            }
            
            if (allComplete)
            {
                Debug.Log("All stars complete! Final island unlocked!");
                // Unlock final island
            }
        }
    }
    
    public int GetHumanityScore()
    {
        return humanityScore;
    }
    
    public bool IsStarComplete(int islandIndex)
    {
        if (islandIndex >= 0 && islandIndex < starCompletion.Length)
            return starCompletion[islandIndex] == 1;
        return false;
    }

    public bool AreAllStarsComplete()
    {
        foreach (int star in starCompletion)
        {
            if (star == 0)
            {
                return false;
            }
        }

        return true;
    }
}