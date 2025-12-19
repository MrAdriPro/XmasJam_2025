using UnityEngine;

public class Proyectile : MonoBehaviour
{
    public float lifetime = 3f;
    private PlayerController player;
    public float multiplyDamageBy = 1f;
    public float multiplySpeedBy = 1f;

    public bool isPlayerShot = true;
    public bool isSniper = false;

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
        if (isPlayerShot && collision.CompareTag("Enemy"))
        {
            EnemyController enemy = collision.GetComponent<EnemyController>();
            if (enemy != null && player != null)
            {
                float sniperMultiplier = isSniper ? 100f : 1f;
                float baseDamage = player.bulletDamage * multiplyDamageBy;

                bool isCritical = Random.value < Mathf.Clamp01(player.criticalChance);

                float finalDamageFloat = baseDamage * (isCritical ? criticalMultiplier : 1f);

                int finalDamage = Mathf.CeilToInt(finalDamageFloat);

                enemy.TakeDamage(finalDamage);

                if (isCritical)
                {
                    Debug.Log($"Critical! Damage: {finalDamage}");
                }
            }
            if(!isSniper) Destroy(gameObject);


        }
        if (isPlayerShot == false && collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>()?.TakeDamage(1,transform.position);

            Destroy(gameObject);
        }
    }
}
