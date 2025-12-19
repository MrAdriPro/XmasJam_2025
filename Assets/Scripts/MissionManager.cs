using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MissionManager : MonoBehaviour
{
    [Header("Referencias de Scripts")]
    public PlayerCarry player;
    public Cradle cradle;
    public PAUSE pauseScript; // Arrastra aquí el objeto que tiene tu script PAUSE

    [Header("Colores")]
    public Color colorIncompleto = Color.black;
    public Color colorCompletado = Color.green;

    [Header("UI Madera")]
    public Toggle toggleMadera;
    public TextMeshProUGUI textoMadera;

    [Header("UI Otros Objetos")]
    public Toggle toggleFaro;
    public GameObject objetoFaroEnCuna;

    public Toggle toggleCuna;
    public GameObject objetoCunaEnCuna;

    private bool victoriaLlamada = false;

    void Start()
    {
        toggleMadera.interactable = false;
        toggleFaro.interactable = false;
        toggleCuna.interactable = false;
    }

    void Update()
    {
        textoMadera.text = "Madera " + player.carriedWood + "/" + player.maxWood;
        ActualizarToggle(toggleMadera, cradle.woodDelivered);

        ActualizarToggle(toggleFaro, objetoFaroEnCuna.activeSelf);
        ActualizarToggle(toggleCuna, objetoCunaEnCuna.activeSelf);


        if (toggleMadera.isOn && toggleFaro.isOn && toggleCuna.isOn && !victoriaLlamada)
        {
            victoriaLlamada = true;
            ManejarVictoria();
        }
    }

    void ActualizarToggle(Toggle tg, bool estaCompletado)
    {
        tg.isOn = estaCompletado;
        if (tg.graphic != null)
        {
            tg.graphic.color = estaCompletado ? colorCompletado : colorIncompleto;
        }
    }

    void ManejarVictoria()
    {
        if (pauseScript != null)
        {
            pauseScript.PlayerWin();
        }
        else
        {
            Debug.LogError("No se ha encontrado la referencia al script PAUSE en el MissionManager");
        }
    }
}