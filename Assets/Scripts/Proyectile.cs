using UnityEngine;

public class Proyectile : MonoBehaviour
{
    public float lifetime = 3f;
    private PlayerController player;
    public float multiplyDamageBy = 1f;
    public float multiplySpeedBy = 1f;

    [Header("Criticals")]
    public float criticalMultiplier = 2f;

    void Start()
    {
        Destroy(gameObject, lifetime);
        player = Object.FindFirstObjectByType<PlayerController>();
    }

    void Update()
    {
        if (player == null) return;
        transform.Translate(Vector3.forward * player.bulletSpeed * multiplySpeedBy * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            EnemyController enemy = collision.GetComponent<EnemyController>();
            if (enemy != null && player != null)
            {
                float baseDamage = player.bulletDamage * multiplyDamageBy;

                bool isCritical = Random.value < Mathf.Clamp01(player.bulletCriticalChance);

                float finalDamageFloat = baseDamage * (isCritical ? criticalMultiplier : 1f);

                int finalDamage = Mathf.CeilToInt(finalDamageFloat);

                enemy.TakeDamage(finalDamage);

                if (isCritical)
                {
                    Debug.Log($"Critical! Damage: {finalDamage}");
                }
            }

            Destroy(gameObject);
        }
    }
}
