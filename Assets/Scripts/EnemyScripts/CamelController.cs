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
    private Rigidbody rb;
    private bool isjumping;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        attackCD = data.attackRate;
        jumpDur = jumpDuration;
    }

    // Update is called once per frame
    void Update()
    {
        JumpHandler();

        if (isjumping == true)
        {
            jumpDur -= Time.deltaTime;
        }

        if (jumpDur < 0)
        {
            isjumping = false;
            jumpDur = jumpDuration;
        }
    }

    void JumpHandler() 
    {
        if (enemyController.actionActive)
        {
            attackCD -= Time.deltaTime;

            if (attackCD <= 0 && isjumping == false)
            {
                JumpAttack();
                print("CamelJump");
                attackCD = data.attackRate;

            }
        }
    }
    
    void JumpAttack() 
    {
        if (isjumping == true) return;
        // Get player Transform and Direcction
        Transform target = enemyController.playerCol[0].transform;

        if (target != null)
        {
            Vector3 targetDir = target.position - transform.position;

            // Make it jump
            if (jumpDur < jumpDuration)
            {
                print("Jumping");

                jumpDur += Time.deltaTime;

                transform.position = Vector3.Lerp(transform.position, target.position + (targetDir * jumpForce), jumpDur);
            }

            rb.AddForce(targetDir * jumpForce, ForceMode.Impulse);


            isjumping = true;
        }
    }
}
