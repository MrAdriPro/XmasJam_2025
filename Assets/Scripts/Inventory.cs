using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] PlayerController player;
    public Dictionary<string, float> _upgrades = new Dictionary<string, float>();

    private void Start()
    {
        _upgrades.Add("health", 0);
        _upgrades.Add("attackSpeed", 0);
        _upgrades.Add("damage", 0);
        _upgrades.Add("speed", 0);
        _upgrades.Add("critChance", 0);
    }

    public void AddUpgrade(string stat, float amount)
    {
        if (stat == "damage" || stat == "attackSpeed" || stat == "speed" || stat == "critChance")
        {
            _upgrades[stat] += amount;
            player.RefreshStats();
        }

        if (stat == "health")
        {
            player.currentHealth += amount;
        }
    }
}
