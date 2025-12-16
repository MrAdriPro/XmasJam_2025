using UnityEngine;

public class MissionObject : MonoBehaviour
{
    public string itemId;

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

        carry.PickUp(itemId, missionPrefab);
        gameObject.SetActive(false);
    }
}
