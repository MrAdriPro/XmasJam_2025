using System;
using Unity.VisualScripting;
using UnityEngine;

public class CowExplotion : MonoBehaviour
{
    public int damage = 4;
    public float duration = 0.5f;
    public float radius = 5f;
    
    private bool exploded = false;
    
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private LayerMask enemyMask;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        
        Gizmos.DrawWireSphere(transform.position, radius);
    }
    
    private void Update()
    {
        // Object lifetime;
        duration -= Time.deltaTime;

        if (duration <= 0)
        {
            Destroy(this.gameObject);
        }
        
        // Damage all
        Collider[] playerTargets = Physics.OverlapSphere(transform.position, radius, playerMask);
        Collider[] enemyTargets = Physics.OverlapSphere(transform.position, radius, enemyMask);

        if (exploded == false)
        {
            foreach (var player in playerTargets)
            {
                PlayerController playerController = player.GetComponent<PlayerController>();

                playerController.TakeDamage(damage);
            }

            foreach (var enemy in enemyTargets)
            {
                EnemyController enemyController = enemy.GetComponent<EnemyController>();

                enemyController.TakeDamage(damage);
            }
            
            exploded = true;
        }
    }
}
