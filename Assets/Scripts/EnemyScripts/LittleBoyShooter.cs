using UnityEngine;

public class LittleBoyShooter : MonoBehaviour
{
    [SerializeField] private EnemyController enemyController;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private EnemyStats enemyStats;
    [SerializeField] AudioManager audioManager;

    private Animator animator;

    private float timer;
    private float attackRate;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        attackRate = enemyStats.attackRate;
    }

    private void Update()
    {
        if (enemyController.actionActive) 
        {
            timer += Time.deltaTime;

            if (timer >= attackRate) 
            {
                animator.SetTrigger("isAttacking");
                timer = 0f; 
            }
        }
        else 
        {
            timer = 0f;
        }   
    }

    public void ExecuteShoot() 
    {
        if (enemyController.playerCol.Length == 0) return;

        Transform targetPoint = enemyController.playerCol[0].transform;
        
        var lookPos = targetPoint.position - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        
        audioManager.PlaySong(0);
        Instantiate(projectilePrefab, transform.position + new Vector3(0f, 0.2f, 0f), rotation);
        
        
    }
}

