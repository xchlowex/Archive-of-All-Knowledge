using UnityEngine;

public class EnemyAI : MonoBehaviour {
    public float speed;
    public float attackRange; // Slightly larger than physical radius
    public float attackCooldown;
    
    private Transform _player;
    private bool _isChasing = false;
    private float _nextAttackTime;
    private Animator _anim;

    void Start() {
        _anim = GetComponent<Animator>();
    }

    void Update() {
        if (_isChasing && _player != null) {
            float distance = Vector2.Distance(transform.position, _player.position);

            if (distance > attackRange) {
                transform.position = Vector2.MoveTowards(transform.position, 
                                     _player.position, speed * Time.deltaTime);
            } else {
                // Only attack if the timer has passed
                if (Time.time >= _nextAttackTime) {
                    Attack();
                }
            }
        }
    }

    private void Attack() {
        Debug.Log("Enemy Attacking!");
        _anim.SetTrigger("OnAttack");
        _nextAttackTime = Time.time + attackCooldown;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            _player = other.transform;
            _isChasing = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            _isChasing = false;
            _player = null;
        }
    }
}