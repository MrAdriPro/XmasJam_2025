using UnityEngine;

public class PlayerCarry : MonoBehaviour
{
    public string carriedItemId;    
    public GameObject carriedPrefab;

    public bool HasItem => !string.IsNullOrEmpty(carriedItemId) && carriedPrefab != null;

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
}
