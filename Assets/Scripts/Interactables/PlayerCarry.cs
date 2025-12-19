using UnityEngine;

public class PlayerCarry : MonoBehaviour
{
    public string carriedItemId;
    public GameObject carriedPrefab;

    [Header("Wood System")]
    public int carriedWood;
    public int maxWood = 6;

    public bool HasItem => !string.IsNullOrEmpty(carriedItemId);

    public bool PickUp(string id, GameObject prefab)
    {
        if (HasItem) return false;

        carriedItemId = id;
        carriedPrefab = prefab;
        return true;
    }

    public void Drop()
    {
        carriedItemId = null;
        carriedPrefab = null;
    }

    public void AddWood(int amount)
    {
        carriedWood += amount;
        if (carriedWood > maxWood) carriedWood = maxWood;
    }
}