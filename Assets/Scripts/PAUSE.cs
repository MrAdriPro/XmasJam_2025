using System.Collections;
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
    public void PlayerLose()
    {
        gameEnded = true;
        StartCoroutine(FadeInPanel(losePanel));
    }

    private IEnumerator FadeInPanel(GameObject panel)
    {
        yield return new WaitForSecondsRealtime(1.5f);

        panel.SetActive(true);
        CanvasGroup cg = panel.GetComponent<CanvasGroup>();

        float duration = 2.0f; 
        float currentTime = 0f;

        while (currentTime < duration)
        {
            currentTime += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(0f, 1f, currentTime / duration);
            yield return null;
        }

        cg.alpha = 1f;

        // 2. Ahora pausamos el juego y liberamos el ratón
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
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
        if (gameEnded) return;
        gameEnded = true;
        StartCoroutine(FadeInPanel(winPanel));
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
