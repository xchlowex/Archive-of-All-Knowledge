using UnityEngine;
using UnityEngine.Events;
public class Health : MonoBehaviour {
    [SerializeField] public int currentHealth;
    [SerializeField] private int maxHealth;

    void Start() {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage) {
        currentHealth -= damage;
        Debug.Log(gameObject.name + " took " + damage + " damage. Current health: " + currentHealth);   
        if (currentHealth <= 0) Die();
    }
    private void Die() {
            // Debug.Log(gameObject.name + " Died!");

            // If the object that died is the Player, tell the manager it's Game Over
            if (gameObject.CompareTag("Player")) {
                // Passing 'false' because the player died (Defeat)
                RoboticsGameManager.Instance.EndGame(false); 
            } 
            else {
                // If it was a monster that died, just destroy it
                Destroy(gameObject);
            }
    }
}
