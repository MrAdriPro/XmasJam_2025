using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyLoot
{
    public GameObject itemPrefab;
    public float weight = 10f;
}

public class EnemyController : MonoBehaviour
{
    [Header("Enemy Configuration")]
    public int maxHealth = 3;
    private float currentHealth;
    public float moveSpeed = 2f;
    public float damage = 1f;
    public int experienceDrop = 1;
    public EnemyStats data;
    public GameObject model;

    [Header("Loot System")]
    [Range(0, 100)] public float globalDropChance = 50f; 
    public List<EnemyLoot> lootTable; 

    [Header("Combat & Detection")]
    [SerializeField] GameObject explotion;
    public float overlapRadius;
    public LayerMask playerMask;
    public Collider[] playerCol;
    public bool actionActive = false;

    private Animator animator;
    private bool isDead = false;
    private Collider enemyCollider;
    private Rigidbody _rb;
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
        _rb = GetComponent<Rigidbody>();

        if (data != null)
        {
            currentHealth = data.maxHealth;
            moveSpeed = data.moveSpeed;
            damage = data.damage;
        }

        player = FindFirstObjectByType<PlayerController>();
        if (model != null) initialLocalScale = model.transform.localScale;
    }

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }

    void Update()
    {
        if (isDead) return;
        Movement();
        UpdateFacing();
    }

    private void Movement()
    {
        if (playerTransform == null) return;

        if (data.enemyType == EnemyType.Sheep || data.enemyType == EnemyType.Cow)
        {
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
        }

        if (data.enemyType == EnemyType.LittleBoy || data.enemyType == EnemyType.Camel)
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

        if (data.enemyType != EnemyType.Sheep && model != null)
        {
            Vector3 s = initialLocalScale;
            s.x = Mathf.Abs(initialLocalScale.x) * -sign;
            model.transform.localScale = s;
        }
        if (data.enemyType == EnemyType.LittleBoy && model != null)
        {
            Vector3 s = initialLocalScale;
            s.x = Mathf.Abs(initialLocalScale.x) * sign;
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
        if (isDead) return;
        currentHealth -= damageAmount;
        if (currentHealth <= 0) Die();
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isDead) Attack();
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        LevelManager.Instance.AddExperience(experienceDrop);

        DropLoot();

        if (enemyCollider != null) enemyCollider.enabled = false;
        if(_rb != null) Destroy(_rb);

        if (data.enemyType == EnemyType.Cow)
        {
            if (explotion != null)
                Instantiate(explotion, transform.position + new Vector3(0f, 0.10f, 0f), Quaternion.identity);
                animator.SetTrigger("dead");
            Destroy(gameObject,0.6f);
        }
        else if (data.enemyType == EnemyType.Sheep || data.enemyType == EnemyType.LittleBoy)
        {
            if (animator != null) animator.SetTrigger("isDead");
            Destroy(gameObject, 1f);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void DropLoot()
    {
        if (lootTable == null || lootTable.Count == 0) return;

        float globalRoll = Random.Range(0f, 100f);
        if (globalRoll > globalDropChance) return; 

        float totalWeight = 0f;
        foreach (var loot in lootTable)
        {
            totalWeight += loot.weight;
        }

        float randomWeightRoll = Random.Range(0f, totalWeight);
        float currentWeightSum = 0f;

        foreach (var loot in lootTable)
        {
            currentWeightSum += loot.weight;
            if (randomWeightRoll <= currentWeightSum)
            {
                if (loot.itemPrefab != null)
                {
                    Instantiate(loot.itemPrefab, transform.position + new Vector3(0,1,0), Quaternion.identity);
                }
                break; 
            }
        }
    }

    private void Attack()
    {
        if (player != null) player.TakeDamage(data.damage, transform.position);
    }
}


