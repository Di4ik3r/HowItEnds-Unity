using Assets.Scripts.DayNightCycle;
using Assets.Scripts.Game;
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
    public GameObject saveDialog;

    public Text errorText;

    public Text statusBarText;

    string fileName = "";

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
        AudioManager.instance.PlayMusic("MainMenu", 2);
        AudioManager.instance.PauseMusic("PauseMenu");

        IsGamePaused = false;
        SceneManager.LoadScene("Menu");
    }

    public void ShowSaveDialog()
    {        
        saveDialog.SetActive(true);       
    }

    public void CloseSaveDialog()
    {        
        saveDialog.SetActive(false);
    }

    public void FileNameInputTextChanged(string value)
    {
        if (value != "" && !value.Contains("/") && !value.Contains(":"))
        {
            fileName = value;
            errorText.text = "Name allowed!";
        }
        else
        {
            fileName = "";
            errorText.text = "Empty, '/' & ':' arent allowed!";
        }
    }

    public void SaveGame()
    {       
        if(fileName != "")
        {
            saveDialog.SetActive(false);
            IsGameSaved = true;
            GameService.SaveGame(new GameModel(MapHolder.getInstance(), TimeHolder.getInstance()), fileName);
            statusBarText.text = "Game saved!";
        }
    }

    public void Options()
    {
        AudioManager.instance.PlayMusic("MainMenu", 2);
        AudioManager.instance.PauseMusic("PauseMenu");

        IsGamePaused = false;
        OptionsBtnIsClicked = true;
        SceneManager.LoadScene("Menu");        
    }

    public void Quit()
    {
        if (PlayerPrefs.GetInt("autosave") == 1)
        {
            GameService.SaveGame(new GameModel(MapHolder.getInstance(), TimeHolder.getInstance()), "", true);
        }

        Application.Quit();
    }

    public void Resume()
    {
        AudioManager.instance.PauseMusic("PauseMenu");
        AudioManager.instance.PlayMusic("Game", 2);

        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        IsGamePaused = false;
    }

    private void Pause()
    {
        AudioManager.instance.PlayMusic("PauseMenu", 2);
        AudioManager.instance.PauseMusic("Game");

        statusBarText.text = "Game paused.";
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        IsGamePaused = true;
    }
}
