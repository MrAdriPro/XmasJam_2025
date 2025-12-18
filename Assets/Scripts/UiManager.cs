using UnityEngine;

public class UiManager : MonoBehaviour
{
    private bool isPaused;
    public GameObject pausePanel;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
            if (isPaused && Input.GetKeyDown(KeyCode.Escape))
            {
                pausePanel.SetActive(false);
                isPaused = false;
            }
        }
    }
    public void Pause()
    {
        isPaused = true;
        pausePanel.SetActive(true);
        Time.timeScale = 0;
    }
}
