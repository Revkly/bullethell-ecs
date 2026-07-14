using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    public GameObject pausePanel;
    private bool isPaused = false;

    void Start()
    {
        // Pastikan game berjalan normal saat mulai
        Time.timeScale = 1f;
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    void Update()
    {
        // Tombol Esc di keyboard untuk Pause/Resume
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        if (pausePanel != null) pausePanel.SetActive(true);
        Time.timeScale = 0f; // Hentikan waktu (termasuk ECS)
    }

    public void ResumeGame()
    {
        isPaused = false;
        if (pausePanel != null) pausePanel.SetActive(false);
        Time.timeScale = 1f; // Jalankan waktu kembali
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f; // WAJIB! Kembalikan waktu normal sebelum ganti scene
        ECSWorldResetter.ResetAndLoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Debug.Log("Keluar dari Game!");
        Application.Quit();
    }
}
