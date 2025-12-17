using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

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


        //Time.timeScale = 0;
        // UIManager.Instance.ShowLevelUpPanel();
    }
}
