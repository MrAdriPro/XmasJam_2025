using System.Collections;
using UnityEngine;

public class PlayerMelee : MonoBehaviour
{
    public GameObject meleeHitbox;
    public PlayerController player;
    private float nextMeleeTime;
    public float MeleeCooldownRate = 1f;


    void Update()
    {
        HandleAiming();
        HandleMelee();
    }
    private void HandleAiming()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            meleeHitbox.transform.LookAt(point);
            meleeHitbox.transform.localEulerAngles = new Vector3(0, meleeHitbox.transform.localEulerAngles.y, 0);
        }
    }
    private void HandleMelee()
    {
        if (Input.GetButton("Fire2") && Time.time > nextMeleeTime)
        {
            nextMeleeTime = Time.time + 1f / MeleeCooldownRate * player.meleemultiplyMeleeRateBy;
            StartCoroutine(MeleeCoroutine());
        }
    }
    private IEnumerator MeleeCoroutine()
    {
        meleeHitbox.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        meleeHitbox.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyController enemy = other.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(player.meleeDamage);
            }
        }
    }

}
