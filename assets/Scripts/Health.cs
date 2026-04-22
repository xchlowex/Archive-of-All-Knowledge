using UnityEngine;
public class Health : MonoBehaviour {
    public int currentHealth;
    public int maxHealth;

    public void TakeDamage(int damage) {
        currentHealth -= damage;
        if (currentHealth <= 0) Die();
    }

    private void Die() {
        // Placeholder for death logic
        Debug.Log("Entity Died!");
    }
}
