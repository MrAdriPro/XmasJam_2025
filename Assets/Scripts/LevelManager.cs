using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    [SerializeField] private GameObject[] buttons;
    [SerializeField] private GameObject[] maxxedButtons;
    [SerializeField] private Transform[] buttonsPos;
    [SerializeField] private GameObject levelUpPanel;
    public bool levelUp;
    public Slider xpSlider;
    public TextMeshProUGUI textXP;

    public bool[] buttonsOff = new bool[5];

    public int currentLevel = 1;
    public int currentXP = 0;
    private int xpToNextLevel = 10; 

    private int buttonNum1;
    private int buttonNum2;
    private int buttonNum3;

    [SerializeField] private float lerpSpeed = 5f;
    private float targetXP;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        targetXP = currentXP;
        UpdateUI(true);
    }
    private void Update()
    {
        targetXP = Mathf.Lerp(targetXP, currentXP, Time.deltaTime * lerpSpeed);
        xpSlider.value = targetXP;
        textXP.text = currentXP + " / " + xpToNextLevel + " XP";
    }

    public void AddExperience(int amount)
    {
        currentXP += amount;
        print("Gained " + amount + " XP. Current XP: " + currentXP + "/" + xpToNextLevel);

        if (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        levelUp = true;
        currentLevel++;
        currentXP -= xpToNextLevel;
        targetXP = 0;
        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.5f);
        print("Leveled up to Level " + currentLevel + "! Next level at " + xpToNextLevel + " XP.");

        UpdateUI(true);
        UpgradeScreen();

        // UIManager.Instance.ShowLevelUpPanel();
    }
    private void UpdateUI(bool instant = false)
    {
        xpSlider.maxValue = xpToNextLevel;
        if (instant)
        {
            targetXP = currentXP;
        }
    }

    private void UpgradeScreen()
    {
        // Elige botón random
        RandomizeCard();

        // Desactiva los que hayas llegado al cap de la stat
        OffButtons();

        // Los pone en las posiciones correspondientes
        buttons[buttonNum1].transform.position = buttonsPos[0].position;
        buttons[buttonNum2].transform.position = buttonsPos[1].position;
        buttons[buttonNum3].transform.position = buttonsPos[2].position;

        

        // Los Activa
        buttons[buttonNum1].SetActive(true);
        buttons[buttonNum2].SetActive(true);
        buttons[buttonNum3].SetActive(true);
        
        
        AltPanel(true);

    }

    private void RandomizeCard()
    {
        // Elige botón random
        buttonNum1 = Random.Range(0,buttons.Length);

        
        for (int i = 0; i < 9999; i++)
        {
            buttonNum2 = Random.Range(0,buttons.Length);

            if(buttonNum2 != buttonNum1) break;
        }

        for (int i = 0; i < 9999; i++)
        {
            buttonNum3 = Random.Range(0,buttons.Length);

            if(buttonNum3 != buttonNum1 && buttonNum3 != buttonNum2) break;
        }

        print(buttonNum1);
        print(buttonNum2);
        print(buttonNum3);
    }

    public void AltPanel(bool activate)
    {
        levelUpPanel.SetActive(activate);

        if(activate)
        {
            print("activado");
            Time.timeScale = 0;
        }
        else
        {
            foreach (var button in buttons)
            {
                button.SetActive(false);
                Time.timeScale = 1;
                levelUp = false;
                UpdateUI();
            }

            
        }
    }

    private void OffButtons()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttonsOff[i] == true)
            {
                buttons[i] = maxxedButtons[i];
            }
        }
    }
}
