using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("EnemyConfiguration")]
    public int maxHealth = 3;
    private float currentHealth;
    public float moveSpeed = 2f;
    public int experienceDrop = 1;

    private Transform playerTransform; 

    void Start()
    {
        currentHealth = maxHealth;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    void Update()
    {
        if (playerTransform != null)
        {
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;

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


        Destroy(gameObject);
    }

}
