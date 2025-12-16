using UnityEngine;
using System.Linq;

public class Cradle : MonoBehaviour
{
    [System.Serializable]
    public struct Mapping
    {
        public string itemId;
        public GameObject spawnObject;
        public bool consumeOnUse;
    }

    public Mapping[] mappings;
    public KeyCode interactKey = KeyCode.E;
    public Vector3 spawnOffset = Vector3.up * 0.5f;

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (!Input.GetKeyDown(interactKey)) return;

        var carry = other.GetComponentInChildren<PlayerCarry>() ?? other.GetComponentInParent<PlayerCarry>();
        if (carry == null || !carry.HasItem)
        {
            Debug.Log("Cradle: no llevas ningún item válido.");
            return;
        }

        var map = mappings.FirstOrDefault(m => m.itemId == carry.carriedItemId);
        if (string.IsNullOrEmpty(map.itemId) || map.spawnObject == null)
        {
            Debug.Log($"Cradle: no hay mapeo para el item '{carry.carriedItemId}'.");
            return;
        }

        if (map.spawnObject.scene.IsValid())
        {
            map.spawnObject.SetActive(true);
            map.spawnObject.transform.position = transform.position + spawnOffset;
            map.spawnObject.transform.rotation = transform.rotation;
        }
        else
        {
            var inst = Instantiate(map.spawnObject, transform.position + spawnOffset, transform.rotation);
            inst.transform.SetParent(transform);
        }

        if (map.consumeOnUse)
            carry.Drop();

        Debug.Log($"Cradle: activado objeto para item '{map.itemId}'.");
    }
}
