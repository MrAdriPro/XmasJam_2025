using UnityEngine;

public class MissionObject : MonoBehaviour
{
    [Tooltip("Identificador simple del item (ej: \"baby\", \"toy\").")]
    public string itemId;

    [Tooltip("Prefab que representa el objeto asociado (se guardará como referencia en el jugador).")]
    public GameObject missionPrefab;

    public bool collectable = true;

    private void OnTriggerStay(Collider other)
    {
        if (!collectable) return;
        if (!other.CompareTag("Player")) return;
            if (!Input.GetKeyDown(KeyCode.E)) return;

        var carry = other.GetComponentInChildren<PlayerCarry>() ?? other.GetComponentInParent<PlayerCarry>();
        if (carry == null)
        {
            Debug.LogWarning("MissionObject: Player no tiene PlayerCarry.");
            return;
        }

        if (carry.HasItem)
        {
            Debug.Log("MissionObject: ya llevas un objeto, suelta o usa el actual antes de recoger otro.");
            return;
        }

        bool picked = carry.PickUp(itemId, missionPrefab);
        if (picked)
        {
            gameObject.SetActive(false);
        }
    }
}
