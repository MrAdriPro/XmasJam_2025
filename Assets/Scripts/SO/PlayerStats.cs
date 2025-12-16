using UnityEngine;

[CreateAssetMenu(fileName = "Players", menuName = "New Player/Player")]
public class PlayerStats : ScriptableObject
{
    public float health;
    public float moveSpeed;
    public float attackSpeed;
    public int meleDamage;
    public float critRate;
}
