using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool IsGamePaused = false;
    public static bool OptionsBtnIsClicked = false;
    public GameObject pauseMenu;

    public GameObject mainMenu;
    public GameObject optionsMenu;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsGamePaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void MainMenu()
    {
        IsGamePaused = false;
        SceneManager.LoadScene("Menu");
    }

    public void SaveGame()
    {

    }

    public void Options()
    {
        IsGamePaused = false;
        OptionsBtnIsClicked = true;
        SceneManager.LoadScene("Menu");        
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        IsGamePaused = false;
    }

    private void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        IsGamePaused = true;
    }
}
