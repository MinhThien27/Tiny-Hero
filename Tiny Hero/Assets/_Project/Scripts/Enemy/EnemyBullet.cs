using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 1;
    public float lifetime = 3f;

    private Rigidbody rb;
    private Transform target;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SetTarget(Transform player)
    {
        target = player;
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
        Fire();
    }

    private void Fire()
    {
        if (target == null) return;

        Vector3 targetPos = target.position + Vector3.up * 1f; // Aim slightly above the player's position
        Vector3 direction = (targetPos - transform.position).normalized;
        rb.velocity = direction * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        else if (!other.CompareTag("Enemy") && !other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}
