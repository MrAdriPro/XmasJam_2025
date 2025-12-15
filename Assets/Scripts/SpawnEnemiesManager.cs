using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class SpawnEnemiesManager : MonoBehaviour
{
	[Header("References")]
	public Difficulty difficultySO;
	public Transform player;

	public List<Transform> spawnPoints = new List<Transform>();

	[System.Serializable]
	public struct EnemyPrefabEntry
	{
		public EnemyType type;
		public GameObject prefab;
	}

	[Header("Enemy prefabs mapping")]
	public List<EnemyPrefabEntry> enemyPrefabs = new List<EnemyPrefabEntry>();

	private List<Transform> activeSpawnPoints = new List<Transform>();

	private Coroutine spawnRoutine;
	private Coroutine updateActiveRoutine;
	private Coroutine progressionRoutine;

	[Header("Difficulty progression")]
	[Tooltip("Si está en true, la dificultad irá subiendo automáticamente con el tiempo")]
	public bool autoIncreaseDifficulty = true;
	[Tooltip("Segundos que tarda en avanzar a la siguiente dificultad (por etapa)")]
	public float timePerStage = 60f;
	[Tooltip("Segundos de espera antes de empezar a subir la dificultad")]
	public float initialDelay = 10f;

	private DifficultyType currentDifficulty;

	[Header("Spawn timing")]
	[Tooltip("Retraso (segundos) entre cada enemigo cuando se spawnean varios en el mismo punto para evitar solapamiento")]
	public float perEnemyDelay = 0.3f;

	private void Start()
	{
		


		currentDifficulty = (difficultySO != null) ? difficultySO.current : DifficultyType.Easy;

		UpdateActiveSpawnPoints();
		spawnRoutine = StartCoroutine(SpawnLoop());
		updateActiveRoutine = StartCoroutine(UpdateActiveSpawnPointsLoop());
		if (autoIncreaseDifficulty)
		{
			progressionRoutine = StartCoroutine(DifficultyProgressionLoop());
		}
	}

	private void OnValidate()
	{
		UpdateActiveSpawnPoints();
	}

	private IEnumerator UpdateActiveSpawnPointsLoop()
	{
		while (true)
		{
			UpdateActiveSpawnPoints();
			yield return new WaitForSeconds(1f); 
		}
	}

	private void UpdateActiveSpawnPoints()
	{
		if (spawnPoints == null || spawnPoints.Count == 0 || player == null || difficultySO == null)
		{
			return;
		}

		var profile = difficultySO.GetProfile(currentDifficulty);
		int n = Mathf.Clamp(profile.activeSpawnPoints, 1, spawnPoints.Count);

		activeSpawnPoints = spawnPoints.OrderBy(sp => Vector3.Distance(sp.position, player.position))
										.Take(n)
										.ToList();
	}

	private IEnumerator SpawnLoop()
	{
		while (true)
		{
			if (difficultySO == null)
			{
				yield return new WaitForSeconds(1f);
				continue;
			}

			var profile = difficultySO.GetProfile(currentDifficulty);

			if (activeSpawnPoints == null || activeSpawnPoints.Count == 0)
			{
				yield return new WaitForSeconds(1f);
				continue;
			}

			for (int i = 0; i < activeSpawnPoints.Count; i++)
			{
				var sp = activeSpawnPoints[i];
				if (sp == null) continue;

				for (int j = 0; j < Mathf.Max(1, profile.spawnPerWave); j++)
				{
					SpawnAt(sp, profile);
					float delay = Mathf.Max(0.01f, perEnemyDelay);
					if (profile.continuous) delay *= 0.8f;
					yield return new WaitForSeconds(delay);
				}

				float waitBetweenPoints = profile.spawnInterval / Mathf.Max(1, activeSpawnPoints.Count);
				if (profile.continuous) waitBetweenPoints *= 0.7f;

				yield return new WaitForSeconds(waitBetweenPoints);
			}

			if (!profile.continuous)
			{
				yield return new WaitForSeconds(Mathf.Max(0.01f, profile.spawnInterval * 0.25f));
			}
			else
			{
				yield return null;
			}
		}
	}

	private IEnumerator DifficultyProgressionLoop()
	{
		yield return new WaitForSeconds(initialDelay);

		int maxIndex = System.Enum.GetValues(typeof(DifficultyType)).Length - 1;

		while (autoIncreaseDifficulty)
		{
			yield return new WaitForSeconds(Mathf.Max(1f, timePerStage));

			int currentIndex = (int)currentDifficulty;
			if (currentIndex < maxIndex)
			{
				currentIndex++;
				currentDifficulty = (DifficultyType)currentIndex;
				Debug.Log($"Difficulty increased to: {currentDifficulty}");
				UpdateActiveSpawnPoints();
			}
			else
			{
				Debug.Log("DifficultyProgression: reached max difficulty.");
				yield break;
			}
		}
	}

	private void SpawnAt(Transform spawnPoint, Difficulty.DifficultyProfile profile)
	{
		if (profile == null)
		{
			Debug.LogWarning("SpawnEnemiesManager: profile es null.");
			return;
		}

		// elegir por pesos si hay enemyChances definidos
		EnemyType chosenType = profile.GetRandomEnemyType();

		var entry = enemyPrefabs.FirstOrDefault(e => e.type == chosenType);
		if (entry.prefab == null)
		{
			Debug.LogWarning($"SpawnEnemiesManager: no hay prefab asignado para EnemyType {chosenType}");
			return;
		}

		Instantiate(entry.prefab, spawnPoint.position, spawnPoint.rotation);
	}

	private void OnDrawGizmos()
	{
		if (spawnPoints != null)
		{
			foreach (var sp in spawnPoints)
			{
				if (sp == null) continue;
				Gizmos.color = (activeSpawnPoints != null && activeSpawnPoints.Contains(sp)) ? Color.red : Color.yellow;
				Gizmos.DrawSphere(sp.position, 0.4f);
			}
		}
	}
}
