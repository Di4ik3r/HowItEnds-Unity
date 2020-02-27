using Assets.Scripts.DayNightCycle;
using Assets.Scripts.Map;
using Assets.Scripts.Services;
using Microsoft.Win32;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class PauseMenu : MonoBehaviour
{
    public static bool IsGamePaused = false;
    private bool IsGameSaved = false;
    public static bool OptionsBtnIsClicked = false;
    public GameObject pauseMenu;

    public GameObject mainMenu;
    public GameObject optionsMenu;

    public Text statusBarText;

    void Update()
    {
        if (IsGameSaved)
        {
            statusBarText.text = "";
            Resume();
            IsGameSaved = false;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsGamePaused)
            {
                statusBarText.text = "";
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
        IsGameSaved = true;
        GameService.SaveGame(new GameModel(MapHolder.getInstance(), TimeHolder.getInstance()));
        statusBarText.text = "Game saved!";
    }

    public void Options()
    {
        IsGamePaused = false;
        OptionsBtnIsClicked = true;
        SceneManager.LoadScene("Menu");        
    }

    public void Quit()
    {
        GameService.SaveGame(new GameModel(MapHolder.getInstance(), TimeHolder.getInstance()), true);
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
        statusBarText.text = "Game paused.";
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        IsGamePaused = true;
    }
}
