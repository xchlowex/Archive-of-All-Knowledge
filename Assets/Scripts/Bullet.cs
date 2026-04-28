
using UnityEngine;
using UnityEngine.Events;

public class Bullet : MonoBehaviour
{
    public float speed = 12f;
    public bool isPlayerBullet = true;

    void Start()
    {
        Destroy(gameObject, 5f); // Cleanup after 5 seconds
    }
    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
        // Destroy(gameObject, 3f); // Cleanup
    }

   private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if we hit an enemy
        if (other.CompareTag("Enemy"))
        {
            // Get the Health component from the enemy
            Health enemyHealth = other.GetComponent<Health>();
            enemyHealth.TakeDamage(1);

    
            if (enemyHealth.currentHealth > 0)
            {
                Debug.Log("Bullet hit enemy!");
            }
            else if (enemyHealth.currentHealth == 0)
            {
                Destroy(other.gameObject); // Kill monster
                Debug.Log("Enemy died!");
            }

            // Destroy the bullet after it hits
            Destroy(gameObject);
        }
        // Optional: Destroy if it hits a wall
        else if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }

}