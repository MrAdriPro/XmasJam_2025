using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class FointainController : MonoBehaviour
{
    [SerializeField] private EnemyController fontain;
    [SerializeField] private GameObject model;
    [SerializeField] private GameObject brokenModel;
    [SerializeField] private GameObject reward;

    public Color damageColor;
    public Color baseColor;
    public Material modelMat;
    public float timeWait;

    private void Start()
    {
        modelMat.SetColor("_BaseColor", baseColor);
    }

    private void Update()
    {
        if (fontain.isDead)
        {
            model.SetActive(false);
            brokenModel.SetActive(true);
            reward.SetActive(true);
        }

        if (fontain.fontainTookDamage) 
        {
            modelMat.SetColor("_BaseColor", damageColor);
            print("changeColor");

            StartCoroutine(DelayColor());
        }
    }

    private IEnumerator DelayColor() 
    {
        yield return new WaitForSeconds(timeWait);
        fontain.fontainTookDamage = false;
        modelMat.SetColor("_BaseColor", baseColor);
        print("backtocolor");
    }
}
