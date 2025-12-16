using System;
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

	// Valores para exponer progreso a la UI
	public DifficultyType CurrentDifficulty => currentDifficulty;
	public float CurrentStageElapsed { get; private set; } = 0f;
	public float CurrentStageDuration => Mathf.Max(1f, timePerStage);

	[Header("Spawn timing")]
	[Tooltip("Retraso (segundos) entre cada enemigo cuando se spawnean varios en el mismo punto para evitar solapamiento")]
	public float perEnemyDelay = 0.3f;

	[Header("Spawn limits")]
	[Tooltip("Número máximo de enemigos permitidos en la escena. 0 = sin límite")]
	public int maxEnemies = 0;
	[Tooltip("Etiqueta (Tag) usada para identificar enemigos en la escena. Dejar vacío para desactivar el conteo por tag")]
	public string enemyTag = "Enemy";

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
		else
		{
			// Si no hay progresión automática, fijar elapsed a duración (para que barra UI muestre posición fija)
			CurrentStageElapsed = CurrentStageDuration;
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

		float stageStartTime = Time.time;
		CurrentStageElapsed = 0f;

		while (autoIncreaseDifficulty)
		{
			while (CurrentStageElapsed < timePerStage)
			{
				CurrentStageElapsed = Time.time - stageStartTime;
				yield return null;
			}

			CurrentStageElapsed = timePerStage;

			int currentIndex = (int)currentDifficulty;
			if (currentIndex < maxIndex)
			{
				currentIndex++;
				currentDifficulty = (DifficultyType)currentIndex;
				Debug.Log($"Difficulty increased to: {currentDifficulty}");
				UpdateActiveSpawnPoints();

				stageStartTime = Time.time;
				CurrentStageElapsed = 0f;
			}
			else
			{
				Debug.Log("DifficultyProgression: reached max difficulty.");
				CurrentStageElapsed = timePerStage;
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

		if (maxEnemies > 0 && !string.IsNullOrEmpty(enemyTag))
		{
			int current = GetCurrentEnemyCount();
			if (current >= maxEnemies)
			{
				return;
			}
		}

		EnemyType chosenType = profile.GetRandomEnemyType();

		var entry = enemyPrefabs.FirstOrDefault(e => e.type == chosenType);
		if (entry.prefab == null)
		{
			Debug.LogWarning($"SpawnEnemiesManager: no hay prefab asignado para EnemyType {chosenType}");
			return;
		}

		Instantiate(entry.prefab, spawnPoint.position, spawnPoint.rotation);
	}


	private int GetCurrentEnemyCount()
	{
		if (maxEnemies <= 0 || string.IsNullOrEmpty(enemyTag))
		{
			return 0;
		}

		try
		{
			return GameObject.FindGameObjectsWithTag(enemyTag).Length;
		}
		catch
		{
			return 0;
		}
	}

	/// <summary>
	/// Devuelve el progreso normalizado entre 0 y 1 a través de todo el rango de dificultades.
	/// Ejemplo: Easy (index 0) + mitad del tiempo de stage -> 0.5/(maxIndex)
	/// </summary>
	public float GetProgressNormalized()
	{
		int maxIndex = System.Enum.GetValues(typeof(DifficultyType)).Length - 1;
		if (maxIndex <= 0) return 0f;

		float index = (int)currentDifficulty;
		float stageProgress = Mathf.Clamp01(CurrentStageElapsed / CurrentStageDuration);

		// Si ya estamos en la última dificultad, devolver 1
		if ((int)currentDifficulty >= maxIndex)
		{
			return 1f;
		}

		float total = (index + stageProgress) / (float)maxIndex;
		return Mathf.Clamp01(total);
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
