using TMPro;
using UnityEngine;

public class StatChanger : MonoBehaviour
{
    private PlayerController player;
    public TextMeshProUGUI text;
    public StatType statType;
    void Start()
    {



        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    void Update()
    {
        if (statType == StatType.AttackSpeed)
        {
            ChangeText(player.multiplyFireRateBy, player.asLimit);
        }
        if (statType == StatType.MoveSpeed)
        {
            ChangeText(player.playerMoveSpeed, player.speedLimit);
        }

        if(statType == StatType.MeleeDamage)
        {
            text.text = player.meleeDamage.ToString("F1");
        }
        if(statType == StatType.CritRate)
        {
            text.text = player.criticalChance.ToString("F1") + "%";
        }
        
    }
    //here we will change the text the text with the current stat value compare to the cap/limit max value he can have
    private void ChangeText(float statValue, float statCap)
    {
        text.text = statValue.ToString("F1") + " / " + statCap.ToString("F1");
    }
}
public enum StatType :byte 
{
    MoveSpeed,
    AttackSpeed,
    MeleeDamage,
    CritRate
}
