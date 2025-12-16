    using UnityEngine;

public class PlayerCarry : MonoBehaviour
{
    public string carriedItemId;
    public GameObject carriedPrefab;

    public bool HasItem => !string.IsNullOrEmpty(carriedItemId) && carriedPrefab != null;

    public void PickUp(string id, GameObject prefab)
    {
        carriedItemId = id;
        carriedPrefab = prefab;
        Debug.Log($"PlayerCarry: recogido {id}");
    }

    public void Drop()
    {
        carriedItemId = null;
        carriedPrefab = null;
        Debug.Log("PlayerCarry: objeto soltado/consumido");
    }
}
