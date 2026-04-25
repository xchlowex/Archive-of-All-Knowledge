using UnityEngine;

public class TaskBoardStarsController : MonoBehaviour
{
    [Header("Optional: hide these when completed")]
    [SerializeField] private GameObject[] unlitStars = new GameObject[3];

    [Header("Optional: show these when completed")]
    [SerializeField] private GameObject[] litStars = new GameObject[3];

    private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStarCompleted += HandleStarCompleted;
        }
    }

    private void Start()
    {
        RefreshAllStars();
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStarCompleted -= HandleStarCompleted;
        }
    }

    private void HandleStarCompleted(int islandIndex, bool completed)
    {
        if (!completed)
        {
            return;
        }

        UpdateStarVisual(islandIndex, true);
    }

    public void RefreshAllStars()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        int count = Mathf.Max(unlitStars.Length, litStars.Length);
        for (int i = 0; i < count; i++)
        {
            bool completed = GameManager.Instance.IsStarComplete(i);
            UpdateStarVisual(i, completed);
        }
    }

    private void UpdateStarVisual(int index, bool completed)
    {
        if (index >= 0 && index < unlitStars.Length && unlitStars[index] != null)
        {
            unlitStars[index].SetActive(!completed);
        }

        if (index >= 0 && index < litStars.Length && litStars[index] != null)
        {
            litStars[index].SetActive(completed);
        }
    }
}
