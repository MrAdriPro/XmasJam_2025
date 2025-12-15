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
        [Serializable]
        public class EnemyChance
        {
            public EnemyType type;
            [Tooltip("Peso relativo para seleccionar este enemigo (no necesita sumar 1; son pesos relativos)")]
            public float weight = 1f;
        }

        [Tooltip("Lista de enemigos con sus pesos relativos para elegir según probabilidad")]
        public EnemyChance[] enemyChances = new EnemyChance[0];
        [Tooltip("Si true: comportamiento de spawn continuo; si false: spawnes con pausas según spawnInterval")]
        public bool continuous = false;

        public EnemyType GetRandomEnemyType()
        {
            if (enemyChances == null || enemyChances.Length == 0)
            {
                return EnemyType.Sheep; 
            }

            float total = 0f;
            foreach (var e in enemyChances)
            {
                if (e != null) total += Mathf.Max(0f, e.weight);
            }

            if (total <= 0f)
            {
                return enemyChances[0].type;
            }

            float r = UnityEngine.Random.value * total;
            float acc = 0f;
            foreach (var e in enemyChances)
            {
                if (e == null) continue;
                acc += Mathf.Max(0f, e.weight);
                if (r <= acc)
                {
                    return e.type;
                }
            }

            return enemyChances[enemyChances.Length - 1].type;
        }
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
                    enemyChances = new[] { new DifficultyProfile.EnemyChance { type = EnemyType.Sheep, weight = 1f } },
                    continuous = false
                };
            case DifficultyType.Medium:
                return new DifficultyProfile
                {
                    type = t,
                    activeSpawnPoints = 2,
                    spawnInterval = 4f,
                    spawnPerWave = 1,
                    enemyChances = new[] { new DifficultyProfile.EnemyChance { type = EnemyType.Sheep, weight = 70f }, new DifficultyProfile.EnemyChance { type = EnemyType.Cow, weight = 30f } },
                    continuous = false
                };
            case DifficultyType.Hard:
                return new DifficultyProfile
                {
                    type = t,
                    activeSpawnPoints = 4,
                    spawnInterval = 3f,
                    spawnPerWave = 2,
                    enemyChances = new[] { new DifficultyProfile.EnemyChance { type = EnemyType.Sheep, weight = 40f }, new DifficultyProfile.EnemyChance { type = EnemyType.Cow, weight = 30f }, new DifficultyProfile.EnemyChance { type = EnemyType.LittleBoy, weight = 30f } },
                    continuous = false
                };
            case DifficultyType.Crazy:
                return new DifficultyProfile
                {
                    type = t,
                    activeSpawnPoints = 4,
                    spawnInterval = 2f,
                    spawnPerWave = 3,
                    enemyChances = new[] { new DifficultyProfile.EnemyChance { type = EnemyType.Sheep, weight = 10f }, new DifficultyProfile.EnemyChance { type = EnemyType.Cow, weight = 20f }, new DifficultyProfile.EnemyChance { type = EnemyType.LittleBoy, weight = 60f }, new DifficultyProfile.EnemyChance { type = EnemyType.Camel, weight = 40f } },
                    continuous = false
                };
            case DifficultyType.AntiChrist:
                return new DifficultyProfile
                {
                    type = t,
                    activeSpawnPoints = 5,
                    spawnInterval = 1.5f,
                    spawnPerWave = 4,
                    enemyChances = Enum.GetValues(typeof(EnemyType)).Cast<EnemyType>().Select(t => new DifficultyProfile.EnemyChance { type = t, weight = 1f }).ToArray(),
                    continuous = false
                };
            case DifficultyType.RunawayNow:
                return new DifficultyProfile
                {
                    type = t,
                    activeSpawnPoints = 5,
                    spawnInterval = 0.7f,
                    spawnPerWave = 6,
                    enemyChances = Enum.GetValues(typeof(EnemyType)).Cast<EnemyType>().Select(t => new DifficultyProfile.EnemyChance { type = t, weight = 1f }).ToArray(),
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
