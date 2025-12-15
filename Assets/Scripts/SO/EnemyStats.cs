using UnityEngine;

[CreateAssetMenu(fileName = "Enemies", menuName = "New Enemy/Enemy")]
public class EnemyStats : ScriptableObject
{
    public EnemyType enemyType;
    public float health;
    public float moveSpeed;
    public float damage;
    public float attackRate;
}
public enum EnemyType
{
    Sheep,
    Cow,
    LittleBoy,
    Camel
}
