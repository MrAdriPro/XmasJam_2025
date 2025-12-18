using UnityEngine;
using System.Collections.Generic;

public class HealthUIController : MonoBehaviour
{
    [SerializeField] private GameObject heartPrefab;
    private List<GameObject> hearts = new List<GameObject>();

    public void UpdateHearts(float currentHealth)
    {
        int healthInt = Mathf.CeilToInt(currentHealth);
        while (hearts.Count > healthInt)
        {
            int index = hearts.Count - 1;
            GameObject heartToDestroy = hearts[index];
            hearts.RemoveAt(index);
            Destroy(heartToDestroy);
        }
        while (hearts.Count < healthInt)
        {
            GameObject newHeart = Instantiate(heartPrefab, transform);
            hearts.Add(newHeart);
        }

    }
}
