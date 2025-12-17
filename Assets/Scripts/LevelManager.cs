using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    [SerializeField] private GameObject[] buttons;
    [SerializeField] private Transform[] buttonsPos;

    public int currentLevel = 1;
    public int currentXP = 0;
    private int xpToNextLevel = 10; 

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
        currentLevel++;
        currentXP -= xpToNextLevel;
        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.5f);
        print("Leveled up to Level " + currentLevel + "! Next level at " + xpToNextLevel + " XP.");


        // Elige botón random
        int num1 = Random.Range(0,buttons.Length);
        int num2 = Random.Range(0,buttons.Length);
        int num3 = Random.Range(0,buttons.Length);

        // Los pone en las posiciones correspondientes
        buttons[num1].transform.position = buttonsPos[0].position;
        buttons[num2].transform.position = buttonsPos[1].position;
        buttons[num3].transform.position = buttonsPos[2].position;
        
        // Los Activa
        buttons[num1].SetActive(true);
        buttons[num2].SetActive(true);
        buttons[num3].SetActive(true);


        Time.timeScale = 0;
        // UIManager.Instance.ShowLevelUpPanel();
    }
}
