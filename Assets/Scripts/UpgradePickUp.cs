using UnityEngine;

public class UpgradePickUp : MonoBehaviour
{
    enum StatName :byte
    {
        health,
        speed,
        attackSpeed,
        damage,
        critChance,
    }
    
    [SerializeField] StatName statName;
    public float amount;


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Inventory inv = other.gameObject.GetComponent<Inventory>();
            
            inv.AddUpgrade(statName.ToString() , amount);
            
            Destroy(gameObject);
        }
    }
}
