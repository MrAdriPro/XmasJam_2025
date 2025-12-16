using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class LittleBoyShooter : MonoBehaviour
{
    [SerializeField] private EnemyController enemyController;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private EnemyStats enemyStats;

    public float attackRate = 2f;

    private float timer;

    private void Awake()
    {
        attackRate = enemyStats.attackRate;
    }

    private void Update()
    {
        if (enemyController.actionActive) 
        {
            timer += Time.deltaTime;

            if (timer > attackRate) 
            {
                Shoot();
            }
        }
        else if(enemyController.actionActive == false)
        {
            timer = 0f;
            print("Shootn't");
        }   
    }

    private void Shoot() 
    {
        print("Shoot!");

        Transform targetPoint = enemyController.playerCol[0].transform;

        //Vector3 dir = targetpoint.position - transform.position;

        //Quaternion quaternion = Quaternion.Euler(dir.x, dir.y, dir.z);

        var lookPos = targetPoint.position - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);


        Instantiate(projectilePrefab, transform.position + new Vector3(0f,0.2f,0f) , rotation);

        timer = 0f;
    }
}
