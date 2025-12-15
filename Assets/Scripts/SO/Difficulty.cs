using UnityEngine;

[CreateAssetMenu(fileName = "Difficulty", menuName = "newDifficulty/Difficulty")]
public class Difficulty : ScriptableObject
{
    
}
public enum DifficultyLevel : byte
{
    Easy,
    Medium,
    Hard,
    Crazy,
    Antichrist,
    RunawayNow,
}
