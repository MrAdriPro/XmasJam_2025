using UnityEngine;

public class UpgradePickUp : MonoBehaviour
{
    enum StatName
    {
        health,
        speed,
        attackSpeed,
        damage,
        critChance,
    }
    
    [SerializeField] StatName statName;
    public int amount;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Inventory inv = other.gameObject.GetComponent<Inventory>();
            
            inv.AddUpgrade(statName.ToString().ToLower() , amount);
            
            Destroy(gameObject);
        }
    }
}
