using UnityEngine;


public class SpecialPickup : MonoBehaviour
{
    [Tooltip("Índice del proyectil en 'especialProjectiles' del PlayerController")]
    public int specialIndex = 1;

    [Tooltip("Cantidad de munición que da este pickup")]
    public int ammoAmount = 5;

    public float multiplyFireRateBy = 1f;
    public float multiplyMeleeRateBy = 1f;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();

            player.PickupSpecialAmmo(specialIndex, ammoAmount, multiplyFireRateBy,multiplyMeleeRateBy);
            Destroy(gameObject);
        }


        
    }
}
