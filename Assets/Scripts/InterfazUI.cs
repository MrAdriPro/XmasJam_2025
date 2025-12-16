using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class DifficultyUI : MonoBehaviour
{
	[Header("References")]
	public SpawnEnemiesManager spawnManager;
	public RectTransform trackRect;
	public RectTransform pointer;
	public RectTransform labelsParent;
	public GameObject labelPrefab;

	[Header("Labels (manual)")]
	[Tooltip("Si añades cadenas aquí, se usarán como etiquetas en el UI en lugar de los nombres del enum.")]
	public List<string> customLabels = new List<string>();

	[Header("Styling")]
	public Color normalColor = Color.white;
	public Color highlightColor = Color.yellow;
	public float labelYOffset = 18f;
	public float pointerSmoothSpeed = 12f;

	[Header("Sizing (labels)")]
	public int labelFontSize = 14;          // tamaño de fuente fijo para consistencia
	public int tmpFontSizeMin = 10;
	public int tmpFontSizeMax = 28;
	public float labelWidthPadding = 0.9f;  // porcentaje del spacing usado como ancho máximo

	private List<Graphic> labelGraphics = new List<Graphic>();
	private int difficultiesCount = 0;
	private float pointerTargetX = 0f;

    [Obsolete]
    private void Start()
	{
		if (trackRect == null || pointer == null || labelPrefab == null || labelsParent == null || spawnManager == null)
		{
			Debug.LogWarning("DifficultyUI: faltan referencias en el inspector.");
			return;
		}

		Canvas.ForceUpdateCanvases();

		for (int i = labelsParent.childCount - 1; i >= 0; i--)
		{
			DestroyImmediate(labelsParent.GetChild(i).gameObject);
		}

		// Elegir nombres: si customLabels tiene elementos se usan, si no se usan los del enum
		string[] names;
		if (customLabels != null && customLabels.Count > 0)
		{
			names = customLabels.ToArray();
		}
		else
		{
			names = Enum.GetNames(typeof(DifficultyType));
		}

		difficultiesCount = names.Length;
		labelGraphics.Clear();

		// calcular spacing y ancho máximo por etiqueta para evitar solapamientos
		float trackWidth = Mathf.Max(1f, trackRect.rect.width);
		float spacing = (difficultiesCount > 1) ? (trackWidth / (difficultiesCount - 1)) : trackWidth;
		float labelMaxWidth = Mathf.Max(40f, spacing * labelWidthPadding); // mínimo 40px

		for (int i = 0; i < difficultiesCount; i++)
		{
			var go = Instantiate(labelPrefab, labelsParent);
			go.name = "DifficultyLabel_" + i;

			var uguiText = go.GetComponent<Text>();
			var tmpText = go.GetComponent<TextMeshProUGUI>();
			if (uguiText == null && tmpText == null)
			{
				Debug.LogWarning("DifficultyUI: labelPrefab necesita un componente Text o TextMeshProUGUI. Prefab destruido.");
				DestroyImmediate(go);
				continue;
			}

			string label = (names[i] ?? string.Empty).ToUpperInvariant();

			var rt = go.GetComponent<RectTransform>();
			Vector2 size = rt.sizeDelta;
			if (Mathf.Approximately(size.y, 0f)) size.y = 18f;
			rt.sizeDelta = new Vector2(labelMaxWidth, size.y);

			if (tmpText != null)
			{
				// TMP: tamaño fijo y ellipsis
				tmpText.text = label;
				tmpText.alignment = TextAlignmentOptions.Center;
				tmpText.color = normalColor;
				tmpText.enableWordWrapping = false;
				tmpText.overflowMode = TextOverflowModes.Ellipsis;
				tmpText.enableAutoSizing = false; 
				tmpText.fontSize = labelFontSize;
				labelGraphics.Add(tmpText);
			}
			else 
			{
				uguiText.text = label;
				uguiText.alignment = TextAnchor.MiddleCenter;
				uguiText.color = normalColor;
				uguiText.resizeTextForBestFit = false;
				uguiText.fontSize = labelFontSize;
				uguiText.horizontalOverflow = HorizontalWrapMode.Overflow;
				uguiText.verticalOverflow = VerticalWrapMode.Truncate;

				Canvas.ForceUpdateCanvases();
				if (uguiText.preferredWidth > labelMaxWidth)
				{
					string original = label;
					int len = original.Length;
					while (len > 0)
					{
						string trial = original.Substring(0, len) + "...";
						uguiText.text = trial;
						Canvas.ForceUpdateCanvases();
						if (uguiText.preferredWidth <= labelMaxWidth) break;
						len--;
					}
				}

				labelGraphics.Add(uguiText);
			}

			// posicionar a lo largo de la barra
			float t = (difficultiesCount == 1) ? 0f : (float)i / (difficultiesCount - 1);
			float halfWidth = trackWidth * 0.5f;
			rt.pivot = new Vector2(0.5f, 0.5f);
			rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
			rt.anchoredPosition = new Vector2(Mathf.Lerp(-halfWidth, halfWidth, t), labelYOffset);
		}
	}

	private void Update()
	{
		if (spawnManager == null || trackRect == null || pointer == null || labelGraphics.Count == 0) return;

		float normalized = spawnManager.GetProgressNormalized();

		float halfWidth = trackRect.rect.width * 0.5f;
		pointerTargetX = Mathf.Lerp(-halfWidth, halfWidth, normalized);

		Vector2 current = pointer.anchoredPosition;
		current.x = Mathf.Lerp(current.x, pointerTargetX, Time.deltaTime * pointerSmoothSpeed);
		pointer.anchoredPosition = current;

		// Resaltar la dificultad real usando el valor del SpawnEnemiesManager (no el redondeo del progreso)
		int activeIndex = 0;
		try
		{
			activeIndex = (int)spawnManager.CurrentDifficulty;
		}
		catch
		{
			int maxIndex = Mathf.Max(1, labelGraphics.Count - 1);
			float scaled = normalized * maxIndex;
			activeIndex = Mathf.Clamp(Mathf.RoundToInt(scaled), 0, labelGraphics.Count - 1);
		}

		// Si customLabels tiene menos elementos que el enum, clamp para evitar OOB
		activeIndex = Mathf.Clamp(activeIndex, 0, labelGraphics.Count - 1);

		for (int i = 0; i < labelGraphics.Count; i++)
		{
			if (labelGraphics[i] == null) continue;

			// Color por defecto / activo
			var color = (i == activeIndex) ? highlightColor : normalColor;

			// Última etiqueta en rojo solo si es la activa
			if (i == labelGraphics.Count - 1 && i == activeIndex)
			{
				color = Color.red;
			}

			labelGraphics[i].color = color;
		}
	}
}
