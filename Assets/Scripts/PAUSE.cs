using UnityEngine;
using UnityEngine.SceneManagement;

public class PAUSE : MonoBehaviour
{
    [Header("Paneles de Interfaz")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;

    private bool isPaused = false;
    private bool gameEnded = false;

    void Update()
    {
        if (gameEnded) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        pausePanel.SetActive(true);
        Time.timeScale = 0f; 
    }

    public void ResumeGame()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1f; 
    }

    public void PlayerWin()
    {
        gameEnded = true;
        Time.timeScale = 0f;
        winPanel.SetActive(true);
    }

    public void PlayerLose()
    {
        gameEnded = true;
        Time.timeScale = 0f;
        losePanel.SetActive(true);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0); 
    }
  

   
}
