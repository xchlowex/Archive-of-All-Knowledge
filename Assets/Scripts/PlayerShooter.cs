using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    public GameObject bulletPrefab;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // 1. Get mouse position in World Space
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;

            // 2. Calculate direction from player to mouse
            Vector2 direction = (mousePos - transform.position).normalized;
            
            // 3. Fire!
            Shoot(direction);
        }
    }

    void Shoot(Vector2 dir)
    {
        // Calculate rotation so the bullet "faces" the mouse
        // Mathf.Atan2 returns angle in radians, so we convert to degrees
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        // Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // Quaternion bulletRotation = Quaternion.LookRotation(Vector3.forward, dir);

        Quaternion bulletRotation = Quaternion.Euler(0, 0, angle);
        // Spawn bullet at the player's current position
        Instantiate(bulletPrefab, transform.position, bulletRotation);
    }
}