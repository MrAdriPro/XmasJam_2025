using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("MovementConfiguration")]
    public float inputDeadZone = 0.1f;
    private Vector3 movementInput;

    [Header("Combat Configuration")]
    public Transform shootingPivot; 
    public GameObject projectilePrefab;
    public GameObject[] especialProjectiles;
    private float nextFireTime;
    [Header("Stats")]
    public float bulletFireRate = 0.5f;
    public float playerMoveSpeed = 5f;
    public float bulletDamage = 1f;
    public int maxHealth = 3;
    public float bulletCriticalChance = 1f;
    public float bulletSpeed = 10f;



    void Start()
    {
        nextFireTime = 0f;
    }

    void Update()
    {
        HandleMovementInput();
        ApplyMovement();   
        HandleAiming();
        HandleShooting();
    }

    private void HandleMovementInput()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 rawInput = new Vector3(moveX, 0f, moveZ);

        if (rawInput.magnitude < inputDeadZone)
        {
            movementInput = Vector3.zero;
        }
        else
        {
            movementInput = rawInput.normalized;
        }
    }

    private void ApplyMovement()
    {
        Vector3 displacement = movementInput * playerMoveSpeed * Time.deltaTime;
        transform.position += displacement;
    }

    private void HandleAiming()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero); 

        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            shootingPivot.LookAt(point);
            shootingPivot.localEulerAngles = new Vector3(0, shootingPivot.localEulerAngles.y, 0);
        }
    }

    private void HandleShooting()
    {
        if (Input.GetButton("Fire1") && Time.time > nextFireTime)
        {
            nextFireTime = Time.time + 1f / bulletFireRate;
            Shoot();
        }
    }

    private void Shoot()
    {
        Instantiate(projectilePrefab, shootingPivot.position, shootingPivot.rotation);
    }

    public void TakeDamage(int amount)
    {
    }
}
