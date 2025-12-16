using UnityEngine;

public class ExplotionController : MonoBehaviour
{
    private PlayerController data;
    public float duration = 0.5f;
    private int damage = 4;
    public float radius = 1.75f;

    private bool exploded;
    public bool playerAttack;

    private LayerMask targetMask;

    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, radius);
    }

    private void Awake()
    {
        if (playerAttack)
        {
           PlayerController playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

           damage = playerController.meleeDamage;
        }
        
        targetMask = LayerMask.GetMask("Player") | LayerMask.GetMask("Enemy");
    }

    // Update is called once per frame
    void Update()
    {
        // Explotion Duration
        duration -= Time.deltaTime;

        if (duration <= 0)
        {
            Destroy(this.gameObject);
        }


        // Explotion Damage 

        Collider[] explodeTargets = Physics.OverlapSphere(transform.position, radius, targetMask);

        if(exploded == false) 
        {
            foreach (Collider target in explodeTargets)
            {
                if (playerAttack == false)
                {
                    if (target.gameObject.CompareTag("Player"))
                    {
                        PlayerController player = target.GetComponent<PlayerController>();

                        player.TakeDamage(damage, transform.position);
                    }
                }
                
                
                if (target.gameObject.CompareTag("Enemy"))
                {
                    
                    EnemyController enemy = target.GetComponent<EnemyController>();

                    enemy.TakeDamage(damage);
                }
            }

            exploded = true;
        }
    }
}
