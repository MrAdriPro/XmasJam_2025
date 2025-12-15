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

	private void Start()
	{
		if (difficultySO == null)
		{
			Debug.LogWarning("SpawnEnemiesManager: difficultySO no asignado. Usando defaults.");
		}

		UpdateActiveSpawnPoints();
		spawnRoutine = StartCoroutine(SpawnLoop());
		updateActiveRoutine = StartCoroutine(UpdateActiveSpawnPointsLoop());
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

		var profile = difficultySO.GetProfile(difficultySO.current);
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

			var profile = difficultySO.GetProfile(difficultySO.current);

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

	private void SpawnAt(Transform spawnPoint, Difficulty.DifficultyProfile profile)
	{
		if (profile.allowedEnemyTypes == null || profile.allowedEnemyTypes.Length == 0)
		{
			Debug.LogWarning("SpawnEnemiesManager: no hay tipos de enemigos permitidos para esta dificultad.");
			return;
		}

		var chosenType = profile.allowedEnemyTypes[Random.Range(0, profile.allowedEnemyTypes.Length)];

		var entry = enemyPrefabs.FirstOrDefault(e => e.type == chosenType);
		if (entry.prefab == null)
		{
			Debug.LogWarning($"SpawnEnemiesManager: no hay prefab asignado para EnemyType {chosenType}");
			return;
		}

		Instantiate(entry.prefab, spawnPoint.position, spawnPoint.rotation);
	}

	private void OnDrawGizmosSelected()
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
