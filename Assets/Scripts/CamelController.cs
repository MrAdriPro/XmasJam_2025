using Unity.VisualScripting;
using UnityEngine;

public class CamelController : MonoBehaviour
{
    [SerializeField] EnemyController enemyController;
    [SerializeField] EnemyStats data;

    public float jumpForce;

    public float jumpDuration;
    private float jumpDur = 0f;

    private float attackCD;

    private bool jumping;


    private void Awake()
    {
        attackCD = data.attackRate;
    }

    // Update is called once per frame
    void Update()
    {
        JumpHandler();
    }

    void JumpHandler() 
    {
        if (enemyController.actionActive)
        {
            attackCD -= Time.deltaTime;

            if (attackCD <= 0)
            {
                JumpAttack();
                print("CamelJump");
                //attackCD = data.attackRate;
            }
        }
    }
    
    void JumpAttack() 
    {
        // Get player Transform and Direcction
        Transform target = enemyController.playerCol[0].transform;

        Vector3 targetDir = target.position - transform.position;

        // Make it jump
        if (jumpDur < jumpDuration)
        {
            print("Jumping");

            jumpDur += Time.deltaTime;

            transform.position = Vector3.Lerp(transform.position, target.position + (targetDir * jumpForce), jumpDur);
        }
    }
}
