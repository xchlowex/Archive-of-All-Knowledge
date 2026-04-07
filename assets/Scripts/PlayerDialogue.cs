using System.Collections.Generic;
using UnityEngine;

public class PlayerDialogue : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    private readonly HashSet<IInteractable> interactablesInRange = new HashSet<IInteractable>();
    private IInteractable currentInteractable;

    private void Update()
    {
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
        {
            return;
        }

        RefreshCurrentInteractable();

        if (Input.GetKeyDown(interactKey) && currentInteractable != null)
        {
            currentInteractable.Interact();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        IInteractable interactable = GetInteractableFromCollider(other);
        if (interactable == null)
        {
            return;
        }

        if (interactablesInRange.Add(interactable))
        {
            interactable.ShowPrompt();
            RefreshCurrentInteractable();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        IInteractable interactable = GetInteractableFromCollider(other);
        if (interactable == null)
        {
            return;
        }

        if (interactablesInRange.Remove(interactable))
        {
            interactable.HidePrompt();
            RefreshCurrentInteractable();
        }
    }

    private void RefreshCurrentInteractable()
    {
        interactablesInRange.RemoveWhere(item => item == null);

        IInteractable bestCandidate = null;
        float bestDistanceSqr = float.MaxValue;

        foreach (IInteractable interactable in interactablesInRange)
        {
            Component component = interactable as Component;
            if (component == null)
            {
                continue;
            }

            float distanceSqr = (component.transform.position - transform.position).sqrMagnitude;
            if (distanceSqr < bestDistanceSqr)
            {
                bestDistanceSqr = distanceSqr;
                bestCandidate = interactable;
            }
        }

        currentInteractable = bestCandidate;
    }

    private static IInteractable GetInteractableFromCollider(Collider2D other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable != null)
        {
            return interactable;
        }

        return other.GetComponentInParent<IInteractable>();
    }

    private void OnDisable()
    {
        foreach (IInteractable interactable in interactablesInRange)
        {
            interactable?.HidePrompt();
        }

        interactablesInRange.Clear();
        currentInteractable = null;
    }
}

public interface IInteractable
{
    void Interact();
    void ShowPrompt();
    void HidePrompt();
}
