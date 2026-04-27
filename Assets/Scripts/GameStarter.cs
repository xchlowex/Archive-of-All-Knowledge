// using UnityEngine;
// using UnityEngine.Events;

// public class GameStarter : MonoBehaviour
// {
//     [Header("State")]
//     public bool isGameRunning = false;

//     [Header("Events")]
//     public UnityEvent onGameStart;
//     public UnityEvent onGameReset;

//     // This is called by your NPC's Interact() or a Button click
//     public void RequestStartGame()
//     {
//         if (!isGameRunning)
//         {
//             StartGameSequence();
//         }
//         else
//         {
//             Debug.Log("Game is already in progress.");
//         }
//     }

//     private void StartGameSequence()
//     {
//         isGameRunning = true;
//         onGameStart?.Invoke();
//         Debug.Log("Modular Game Started!");
//     }

//     // Call this from your RoboticsGameManager's Win/Loss logic
//     public void ResetStarter()
//     {
//         isGameRunning = false;
//         onGameReset?.Invoke();
//         Debug.Log("Modular Game Reset - Ready to play again.");
//     }
// }