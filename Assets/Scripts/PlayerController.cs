using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public PlayerMelee playerMelee;
    [Header("MovementConfiguration")]
    public float inputDeadZone = 0.1f;
    private Vector3 movementInput;
    public GameObject model;

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

    [Header("Knockback")]
    public float knockbackForce = 6f;
    public float knockbackDuration = 0.25f;
    public bool disableInputDuringKnockback = true;
    private Vector3 knockbackVelocity = Vector3.zero;
    private float knockbackTimeRemaining = 0f;

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
        FlipOrientation();
    }

    private void Inmunerable()
    {
        CancelInvoke(nameof(ResetInmune));
        inmune = true;
        Invoke(nameof(ResetInmune), inmuneDuration);
    }

    private void ResetInmune()
    {
        inmune = false;
    }

    private void HandleMovementInput()
    {
        if (disableInputDuringKnockback && knockbackTimeRemaining > 0f)
        {
            movementInput = Vector3.zero;
            return;
        }

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

        if (knockbackTimeRemaining > 0f)
        {
            displacement += knockbackVelocity * Time.deltaTime;
            // Decaimiento suave del knockback
            float decay = Time.deltaTime / Mathf.Max(knockbackDuration, 0.0001f);
            knockbackVelocity = Vector3.Lerp(knockbackVelocity, Vector3.zero, decay);
            knockbackTimeRemaining -= Time.deltaTime;
            if (knockbackTimeRemaining <= 0f)
            {
                knockbackTimeRemaining = 0f;
                knockbackVelocity = Vector3.zero;
            }
        }

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
    private void FlipOrientation()
    {
        if(movementInput.x < 0)
        {
            model.transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (movementInput.x > 0)
        {
            model.transform.localScale = new Vector3(1, 1, 1);
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

    public void TakeDamage(float amount, Vector3? hitSource = null)
    {
        if (inmune)
            return;

        currentHealth -= amount;
        Inmunerable();

        if (hitSource.HasValue)
        {
            Vector3 dir = transform.position - hitSource.Value;
            dir.y = 0f;
            if (dir.sqrMagnitude < 0.0001f)
            {
                // si la fuente coincide, empujar hacia atrás del jugador
                dir = -transform.forward;
            }
            Vector3 knockDir = dir.normalized;
            knockbackVelocity = knockDir * knockbackForce;
            knockbackTimeRemaining = knockbackDuration;
        }

        if (currentHealth <= 0)
        {
            Debug.Log("Player has died.");
            // Aquí puedes llamar al gestor de nivel / muerte
        }
    }
}
