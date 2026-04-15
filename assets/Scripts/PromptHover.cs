using UnityEngine;

public class PromptHover : MonoBehaviour
{
    private NPCBase npcScript;

    void Start()
    {
        // This looks up the hierarchy to find the NPCBase script on the robot
        npcScript = GetComponentInParent<NPCBase>();
        
        if (npcScript == null)
        {
            Debug.LogWarning("PromptHover: Could not find NPCBase on parent!");
        }
    }

    void OnMouseEnter()
    {
        // When mouse hovers over the bubble, tell the NPC to act like it's talking
        if (npcScript != null) 
        {
            npcScript.SetTalking(true);
            Debug.Log("Mouse Hover: Start Talking");
        }
    }

    void OnMouseExit()
    {
        // When mouse leaves, stop the animation
        if (npcScript != null) 
        {
            npcScript.SetTalking(false);
            Debug.Log("Mouse Exit: Stop Talking");
        }
    }
}