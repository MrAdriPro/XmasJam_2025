using TMPro;
using UnityEngine;

public class StatChanger : MonoBehaviour
{
    private PlayerController player;
    private TextMeshProUGUI text;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    void Update()
    {
    }
    //here we will change the text 
}
