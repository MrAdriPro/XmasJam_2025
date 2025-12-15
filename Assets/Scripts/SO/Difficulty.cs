using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "Difficulty", menuName = "newDifficulty/Difficulty")]
public class Difficulty : ScriptableObject
{
    [Serializable]
    public class DifficultyProfile
    {
        public DifficultyType type;
        [Tooltip("Número de spawn points activos (los más cercanos al player)")]
        public int activeSpawnPoints = 1;
        [Tooltip("Segundos entre oleadas (intervalo de spawn) — cuanto menor, más rápido")]
        public float spawnInterval = 5f;
        [Tooltip("Cantidad de enemigos por evento de spawn en cada punto")]
        public int spawnPerWave = 1;
        [Tooltip("Tipos de enemigos permitidos en este nivel de dificultad")]
        public EnemyType[] allowedEnemyTypes = new EnemyType[0];
        [Tooltip("Si true: comportamiento de spawn continuo; si false: spawnes con pausas según spawnInterval")]
        public bool continuous = false;
    }

    public DifficultyType current = DifficultyType.Easy;
    public List<DifficultyProfile> profiles = new List<DifficultyProfile>();

    public DifficultyProfile GetProfile(DifficultyType t)
    {
        var p = profiles.FirstOrDefault(x => x.type == t);
        if (p != null) return p;
        return GetDefaultProfile(t);
    }

    private DifficultyProfile GetDefaultProfile(DifficultyType t)
    {
        switch (t)
        {
            case DifficultyType.Easy:
                return new DifficultyProfile
                {
                    type = t,
                    activeSpawnPoints = 1,
                    spawnInterval = 5f,
                    spawnPerWave = 1,
                    allowedEnemyTypes = new[] { EnemyType.Sheep,  },
                    continuous = false
                };
            case DifficultyType.Medium:
                return new DifficultyProfile
                {
                    type = t,
                    activeSpawnPoints = 2,
                    spawnInterval = 4f,
                    spawnPerWave = 1,
                    allowedEnemyTypes = new[] { EnemyType.Sheep,  EnemyType.Cow,  },
                    continuous = false
                };
            case DifficultyType.Hard:
                return new DifficultyProfile
                {
                    type = t,
                    activeSpawnPoints = 4,
                    spawnInterval = 3f,
                    spawnPerWave = 2,
                    allowedEnemyTypes = new[] { EnemyType.Sheep,  EnemyType.Cow, EnemyType.LittleBoy  },
                    continuous = false
                };
            case DifficultyType.Crazy:
                return new DifficultyProfile
                {
                    type = t,
                    activeSpawnPoints = 4,
                    spawnInterval = 2f,
                    spawnPerWave = 3,
                    allowedEnemyTypes = new[] { EnemyType.Sheep,  EnemyType.Cow, EnemyType.LittleBoy, EnemyType.Camel },
                    continuous = false
                };
            case DifficultyType.AntiChrist:
                return new DifficultyProfile
                {
                    type = t,
                    activeSpawnPoints = 5,
                    spawnInterval = 1.5f,
                    spawnPerWave = 4,
                    allowedEnemyTypes = Enum.GetValues(typeof(EnemyType)).Cast<EnemyType>().ToArray(),
                    continuous = false
                };
            case DifficultyType.RunawayNow:
                return new DifficultyProfile
                {
                    type = t,
                    activeSpawnPoints = 5,
                    spawnInterval = 0.7f,
                    spawnPerWave = 6,
                    allowedEnemyTypes = Enum.GetValues(typeof(EnemyType)).Cast<EnemyType>().ToArray(),
                    continuous = true
                };
            default:
                return new DifficultyProfile { type = t };
        }
    }

    private void OnEnable()
    {
        if (profiles == null || profiles.Count == 0)
        {
            profiles = new List<DifficultyProfile>
            {
                GetDefaultProfile(DifficultyType.Easy),
                GetDefaultProfile(DifficultyType.Medium),
                GetDefaultProfile(DifficultyType.Hard),
                GetDefaultProfile(DifficultyType.Crazy),
                GetDefaultProfile(DifficultyType.AntiChrist),
                GetDefaultProfile(DifficultyType.RunawayNow)
            };
        }
    }
}

public enum DifficultyType
{
    Easy,
    Medium,
    Hard,
    Crazy,
    AntiChrist,
    RunawayNow
}
