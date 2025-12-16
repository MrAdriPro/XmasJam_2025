using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public PlayerMelee playerMelee;
    [Header("MovementConfiguration")]
    public float inputDeadZone = 0.1f;
    private Vector3 movementInput;

    [Header("Combat Configuration")]
    public Transform shootingPivot;
    public GameObject projectilePrefab;           
    public GameObject[] especialProjectiles;      
    public bool inmune = false;
    public float inmuneDuration = 2f;
    [Tooltip("Especial Ammo")]
    public int[] especialAmmo;
    private int currentSpecialIndex = 0; 
    private float nextFireTime;

    [Header("Stats")]
    public float bulletFireRate = 0.5f;
    public float multiplyFireRateBy = 1f;
    private float originalFireRate;
    public float playerMoveSpeed = 5f;
    public float bulletDamage = 1f;
    public float currentHealth = 3;
    public float bulletCriticalChance = 1f;
    public float bulletSpeed = 10f;
    public float meleeDamage = 1f;
    public float meleemultiplyMeleeRateBy = 1f;

    void Start()
    {
        originalFireRate = bulletFireRate;
        nextFireTime = 0f;
        EnsureAmmoArrayMatchesProjectiles();
        if (especialProjectiles == null || especialProjectiles.Length == 0)
            currentSpecialIndex = -1; 
        else
            currentSpecialIndex = 0; 
    }

    void OnValidate()
    {
        EnsureAmmoArrayMatchesProjectiles();
    }

    private void EnsureAmmoArrayMatchesProjectiles()
    {
        if (especialProjectiles == null)
        {
            especialAmmo = null;
            return;
        }

        if (especialAmmo == null || especialAmmo.Length != especialProjectiles.Length)
        {
            int[] newAmmo = new int[especialProjectiles.Length];
            if (especialAmmo != null)
            {
                int copyLen = Mathf.Min(especialAmmo.Length, newAmmo.Length);
                for (int i = 0; i < copyLen; i++) newAmmo[i] = especialAmmo[i];
            }
            especialAmmo = newAmmo;
        }
    }

    void Update()
    {
        HandleMovementInput();
        ApplyMovement();
        HandleAiming();
        HandleShooting();
    }
    private void Inmunerable()
    {
        inmune = true;
        Invoke("ResetInmune", inmuneDuration);
        inmune = false;
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
            nextFireTime = Time.time + 1f / bulletFireRate * multiplyFireRateBy;
            Shoot();
        }
    }

    private GameObject GetActiveProjectilePrefab()
    {
        if (especialProjectiles != null &&
            currentSpecialIndex >= 0 &&
            currentSpecialIndex < especialProjectiles.Length &&
            especialProjectiles[currentSpecialIndex] != null &&
            especialAmmo != null &&
            especialAmmo.Length > currentSpecialIndex &&
            especialAmmo[currentSpecialIndex] > 0)
        {
            return especialProjectiles[currentSpecialIndex];
        }

        if (especialProjectiles != null && especialProjectiles.Length > 0 && especialProjectiles[0] != null)
            return especialProjectiles[0];

        return projectilePrefab;
    }

    private void Shoot()
    {
        GameObject prefabToSpawn = GetActiveProjectilePrefab();
        if (prefabToSpawn == null)
        {
            Debug.LogWarning("No projectile prefab assigned (prefabToSpawn is null).");
            return;
        }

        if (especialProjectiles != null &&
            currentSpecialIndex >= 0 &&
            currentSpecialIndex < especialProjectiles.Length &&
            especialProjectiles[currentSpecialIndex] == prefabToSpawn &&
            especialAmmo != null &&
            especialAmmo.Length > currentSpecialIndex)
        {
            especialAmmo[currentSpecialIndex] = Mathf.Max(0, especialAmmo[currentSpecialIndex] - 1);

            if (especialAmmo[currentSpecialIndex] == 0)
            {
                Debug.Log($"Special projectile index {currentSpecialIndex} depleted. Reverting to index 0.");
                currentSpecialIndex = (especialProjectiles != null && especialProjectiles.Length > 0) ? 0 : -1;
                bulletFireRate = originalFireRate;
            }
        }

        Instantiate(prefabToSpawn, shootingPivot.position, shootingPivot.rotation);
    }

  
    public void PickupSpecialAmmo(int index, int ammoAmount, float fireRate, float meleeRate)
    {
        if (especialProjectiles == null || index < 0 || index >= especialProjectiles.Length)
        {
            Debug.LogWarning("PickupSpecialAmmo: índice inválido o especialProjectiles no configurado.");
            return;
        }

        EnsureAmmoArrayMatchesProjectiles();
        especialAmmo[index] = Mathf.Max(0, especialAmmo[index] + ammoAmount);

        currentSpecialIndex = index;
        bulletFireRate = fireRate;
        //playerMelee.MeleeCooldownRate = meleeRate;
        Debug.Log($"Picked up special projectile index {index} (+{ammoAmount} ammo). Now active (ammo: {especialAmmo[index]}).");

    }

    public void TakeDamage(float amount)
    {
        if (inmune)
            return;
        currentHealth -= amount;
        Inmunerable();
        //now push to other direction that has been hit from
       
        if (currentHealth <= 0)
        {
            Debug.Log("Player has died.");
        }
    }
}
