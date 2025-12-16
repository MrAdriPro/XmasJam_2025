using UnityEngine;

public class MissionObject : MonoBehaviour
{
    public GameObject missionPrefab;
    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player") && Input.GetKeyDown(KeyCode.E))
        {
            gameObject.SetActive(false);
            missionPrefab.SetActive(true);
        }
    }
}
