using UnityEngine;

public class PlayerCarry : MonoBehaviour
{
    public string carriedItemId;    
    public GameObject carriedPrefab;
    
    [Header("WoodVars")]
    public int carriedWood;
    public int maxWood;
    private bool completedWood = false;
    
    public bool HasItem => !string.IsNullOrEmpty(carriedItemId) && carriedPrefab != null;

    void Update()
    {
        LimitWood();
    }
    
    public bool PickUp(string id, GameObject prefab)
    {
        if (HasItem)
        {
            Debug.Log("PlayerCarry: ya llevas un objeto. Suelta/usa el actual antes de recoger otro.");
            return false;
        }

        carriedItemId = id;
        carriedPrefab = prefab;
        Debug.Log($"PlayerCarry: recogido {id}");
        return true;
    }

    public void Drop()
    {
        carriedItemId = null;
        carriedPrefab = null;
        Debug.Log("PlayerCarry: objeto soltado/consumido");
    }

    void LimitWood()
    {
        if (carriedWood > maxWood)
        {
            carriedWood = maxWood;
            completedWood = false;
        }
    }
}
