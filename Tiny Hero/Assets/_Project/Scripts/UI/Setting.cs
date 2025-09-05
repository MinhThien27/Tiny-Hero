using UnityEngine;
using UnityEngine.SceneManagement;

public class Setting : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private string gameplaySceneName = "GamePlay";

    private bool isPaused = false;

    private void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        if (pausePanel != null)
            pausePanel.SetActive(isPaused);

        Time.timeScale = isPaused ? 0 : 1;
    }

    public void NewGame()
    {
        Time.timeScale = 1;

        SaveManager.Instance.ClearSaveData();

        SceneManager.LoadScene(gameplaySceneName);
    }

    public void ClosePanel()
    {
        TogglePause();
    }

    public void OnSettingsButton()
    {
        Debug.Log("Open Settings");
    }

    public void MainMenu()
    {
        Time.timeScale = 1;

        var player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            SaveManager.Instance.SaveGame();
        }

        SceneManager.LoadScene("MainMenu");
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Continue()
    {
        Time.timeScale = 1;
        SaveManager.Instance.ContinueGame();
    }
}
