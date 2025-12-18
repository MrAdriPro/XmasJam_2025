using Unity.VisualScripting;
using UnityEngine;

public class FointainController : MonoBehaviour
{
    [SerializeField] private EnemyController fontain;
    [SerializeField] private GameObject model;
    [SerializeField] private GameObject brokenModel;
    [SerializeField] private GameObject reward;


    private void Update()
    {
        if (fontain.isDead)
        {
            model.SetActive(false);
            brokenModel.SetActive(true);
            reward.SetActive(true);
        }
    }
}
