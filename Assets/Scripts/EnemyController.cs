using UnityEditor;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("EnemyConfiguration")]
    public int maxHealth = 3;
    private float currentHealth;
    public float moveSpeed = 2f;
    public float damage = 1f;
    public int experienceDrop = 1;
    public EnemyStats data;

    [SerializeField] GameObject explotion;

    public float overlapRadius;
    public LayerMask playerMask;
    public Collider[] playerCol;

    public bool actionActive = false;

    private Transform playerTransform;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(transform.position, overlapRadius);
    }

    private void Awake()
    {
        currentHealth = data.maxHealth;
        moveSpeed = data.moveSpeed;
        damage = data.damage;
    }

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    void Update()
    {
        if (data.enemyType == EnemyType.Sheep || data.enemyType == EnemyType.Cow && playerTransform != null)
        {
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
        }

        if (data.enemyType == EnemyType.LittleBoy || data.enemyType == EnemyType.Camel && playerTransform != null)
        {
            playerCol = Physics.OverlapSphere(transform.position, overlapRadius, playerMask);

            if (playerCol.Length > 0)
            {
                actionActive = true;
            }
            else 
            {
                Vector3 direction = (playerTransform.position - transform.position).normalized;
                transform.position += direction * moveSpeed * Time.deltaTime;

                actionActive = false;
            }
        }
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        LevelManager.Instance.AddExperience(experienceDrop);

        if (data.enemyType == EnemyType.Cow) 
        {
            Instantiate(explotion, transform.position + new Vector3(0f,0.10f,0f), Quaternion.identity);
        }

        Destroy(gameObject);
    }

}
