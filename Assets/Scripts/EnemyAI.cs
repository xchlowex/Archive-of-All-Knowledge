using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class EnemyAI : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed; 
    public float stopDistance; 

    [Header("Battle Settings")]
    public float attackRange;
    public float attackCooldown;
    
    [Header("References")]
    public Tilemap walkableTilemap; 
    public Animator anim;

    private Transform _player;
    private bool _isChasing = false;
    private float _nextAttackTime;

    private List<Vector3Int> pathToPlayer = new List<Vector3Int>();
    private Dictionary<Vector3Int, Vector3Int> prevCellMap;
    private Vector3Int lastPlayerCell;
    private Vector3Int lastMonsterCell;

    void Update()
    {
        if (_isChasing && _player != null)
        {
            Vector3Int currentPlayerCell = GetCell(_player.position);
            Vector3Int currentMonsterCell = GetCell(transform.position);

            float distance = Vector2.Distance(transform.position, _player.position);

            if (distance > attackRange)
            {
                Debug.Log("Current Distance: " + distance + " | Attack Range: " + attackRange);
                // Re-calculate if player or monster moved tiles
                if (currentPlayerCell != lastPlayerCell || currentMonsterCell != lastMonsterCell || pathToPlayer.Count == 0)
                {
                    FindPathToPlayer(currentMonsterCell, currentPlayerCell);
                    UpdatePathList(currentMonsterCell, currentPlayerCell);
                    
                    lastPlayerCell = currentPlayerCell;
                    lastMonsterCell = currentMonsterCell;
                }
                TrackPlayer();
            }
            else if (Time.time >= _nextAttackTime)
            {
                Debug.Log("Current Distance: " + distance + " | Attack Range: " + attackRange);
                Attack();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _player = other.transform;
            _isChasing = true;
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _isChasing = false;
            _player = null;
            pathToPlayer.Clear();
        }
    }

    void FindPathToPlayer(Vector3Int start, Vector3Int destination)
    {
        prevCellMap = new Dictionary<Vector3Int, Vector3Int>();
        Queue<Vector3Int> planTravelCells = new Queue<Vector3Int>();
        HashSet<Vector3Int> visitedCells = new HashSet<Vector3Int>();

        planTravelCells.Enqueue(start);
        visitedCells.Add(start);

        while (planTravelCells.Count > 0)
        {
            Vector3Int current = planTravelCells.Dequeue();
            if (current == destination) return;

            foreach (Vector3Int neighbor in GetNeighbors(current))
            {
                if (!visitedCells.Contains(neighbor))
                {
                    visitedCells.Add(neighbor);
                    planTravelCells.Enqueue(neighbor);
                    prevCellMap[neighbor] = current;
                }
            }
        }
    }

    void UpdatePathList(Vector3Int start, Vector3Int destination)
    {
        pathToPlayer.Clear();
        if (prevCellMap == null || !prevCellMap.ContainsKey(destination)) return;

        Vector3Int current = destination;
        while (current != start)
        {
            pathToPlayer.Add(current);
            if (prevCellMap.ContainsKey(current))
                current = prevCellMap[current];
            else
                break;
        }
        pathToPlayer.Reverse();
    }

    void TrackPlayer()
    {
        if (pathToPlayer.Count > 0)
        {
            Vector3 targetWorldPos = walkableTilemap.GetCellCenterWorld(pathToPlayer[0]);
            transform.position = Vector3.MoveTowards(transform.position, targetWorldPos, speed * Time.deltaTime);

            if (Vector2.Distance(transform.position, targetWorldPos) < stopDistance)
            {
                pathToPlayer.RemoveAt(0);
            }
        }
    }

    List<Vector3Int> GetNeighbors(Vector3Int current)
    {
        List<Vector3Int> neighbors = new List<Vector3Int>();
        Vector3Int[] directions = { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right };

        foreach (var dir in directions)
        {
            Vector3Int checkPos = current + dir;
            // The logic: if the ghost map has a tile here, the robot can walk here
            if (walkableTilemap.HasTile(checkPos))
            {
                neighbors.Add(checkPos);
            }
        }
        return neighbors;
    }

    Vector3Int GetCell(Vector3 worldPos) => walkableTilemap.WorldToCell(worldPos);

    private void Attack()
    {
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();

        if (sr != null && _player!= null){
            if (anim != null)
            {
            
                    // if(_player.position.x < transform.position.x){
                // if (_player.position.x < transform.position.x)
                        sr.flipX = (_player.position.x < transform.position.x);
                        // anim.SetTrigger("OnAttack");
                        // sr.flipX = false;
                    // }
                    // else
                    // {
                        anim.SetTrigger("OnAttack");
                    // }
                    Debug.Log("Robot Attacking!");
                }
            
            _nextAttackTime = Time.time + attackCooldown;
        }
    }

    // This helps you SEE the path in the Scene View
    void OnDrawGizmos()
    {
        if (pathToPlayer == null || pathToPlayer.Count == 0 || walkableTilemap == null) return;
        Gizmos.color = Color.cyan;
        foreach (var cell in pathToPlayer)
        {
            Gizmos.DrawSphere(walkableTilemap.GetCellCenterWorld(cell), 0.2f);
        }
    }
    
}