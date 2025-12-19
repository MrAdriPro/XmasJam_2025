using UnityEngine;
using System.Linq;

public class Cradle : MonoBehaviour
{
    [System.Serializable]
    public struct Mapping
    {
        public string itemId;
        public GameObject spawnObject; 
    }

    public Mapping[] mappings;
    public KeyCode interactKey = KeyCode.E;

    [Header("Configuración Madera")]
    public bool woodDelivered = false;
    public GameObject maderaVisual; 

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (!Input.GetKeyDown(interactKey)) return;

        PlayerCarry carry = other.GetComponent<PlayerCarry>();
        if (carry == null) return;

        if (carry.carriedWood >= carry.maxWood && !woodDelivered)
        {
            woodDelivered = true;
            if (maderaVisual != null) maderaVisual.SetActive(true);
            carry.carriedWood = 0; 
            Debug.Log("Madera entregada correctamente");
            return; 
        }

        if (carry.HasItem)
        {
            var map = mappings.FirstOrDefault(m => m.itemId == carry.carriedItemId);
            if (!string.IsNullOrEmpty(map.itemId) && map.spawnObject != null)
            {
                map.spawnObject.SetActive(true);
                carry.Drop();
                Debug.Log("Objeto entregado: " + map.itemId);
            }
        }
    }
}