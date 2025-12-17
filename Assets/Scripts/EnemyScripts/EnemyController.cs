using Unity.VisualScripting;
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
    public GameObject model;
    private Animator animator;
    private bool isDead = false;
    private Collider enemyCollider;


    [SerializeField] GameObject explotion;

    public float overlapRadius;
    public LayerMask playerMask;
    public Collider[] playerCol;

    public bool actionActive = false;

    private Transform playerTransform;
    private PlayerController player;

    private Vector3 initialLocalScale;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(transform.position, overlapRadius);
    }

    private void Awake()
    {
        enemyCollider = GetComponent<BoxCollider>();
        animator = GetComponentInChildren<Animator>();
        currentHealth = data.maxHealth;
        moveSpeed = data.moveSpeed;
        damage = data.damage;
        player = FindFirstObjectByType<PlayerController>();

        initialLocalScale = model.transform.localScale;
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
        Movement();
        UpdateFacing(); 
    }
    private void Movement()
    {
        if(isDead != true)
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
        
    }

    private void UpdateFacing()
    {
        if (playerTransform == null) return;
        if (animator == null) animator = GetComponentInChildren<Animator>();

        float dirX = playerTransform.position.x - transform.position.x;

        const float epsilon = 0.01f;
        if (Mathf.Abs(dirX) <= epsilon)
        {
            if (data.enemyType == EnemyType.Sheep && animator != null)
            {
                animator.SetBool("isWalkingRight", false);
                animator.SetBool("isWalkingLeft", false);
            }
            return;
        }

        float sign = Mathf.Sign(dirX);

        if (data.enemyType != EnemyType.Sheep)
        {
            Vector3 s = initialLocalScale;
            s.x = Mathf.Abs(initialLocalScale.x) * -sign;
            model.transform.localScale = s;
        }

        if (data.enemyType == EnemyType.Sheep && animator != null)
        {
            if (dirX > 0f)
            {
                animator.SetBool("isWalkingRight", false);
                animator.SetBool("isWalkingLeft", true);
            }
            else
            {
                animator.SetBool("isWalkingRight", true);
                animator.SetBool("isWalkingLeft", false);
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
    private void OnCollisionStay(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            Attack();
        }
    }

    void Die()
    {
        LevelManager.Instance.AddExperience(experienceDrop);
        Destroy(enemyCollider);
        if (data.enemyType == EnemyType.Cow) 
        {
            Instantiate(explotion, transform.position + new Vector3(0f,0.10f,0f), Quaternion.identity);
        }

        if (data.enemyType == EnemyType.Sheep) 
        {
            isDead = true;
            animator.SetTrigger("isDead");
            Destroy(this.gameObject, 1f);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    private void Attack()
    {
        player.TakeDamage(data.damage,transform.position);
    }

}
