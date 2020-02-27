using Assets.Scripts.DayNightCycle;
using Assets.Scripts.Map;
using Assets.Scripts.Services;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenuHolder;
    public GameObject optionsMenuHolder;
    public GameObject loadMenuHolder;
    public GameObject map;

    public Dropdown filePathDropdown;  

    void Start()
    {
        if (PauseMenu.OptionsBtnIsClicked)
        {
            Options();
            PauseMenu.OptionsBtnIsClicked = false;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(loadMenuHolder.activeSelf)
            {
                loadMenuHolder.SetActive(false);
            }
        }
    }

    private void FillFilePathDropdown()
    {

        DirectoryInfo directoryInfo = new DirectoryInfo(Application.persistentDataPath);
        FileInfo[] fileInfo = directoryInfo.GetFiles("*.save", SearchOption.AllDirectories);

        filePathDropdown.options.Clear();
        foreach (FileInfo file in fileInfo)
        {
            Dropdown.OptionData optionData = new Dropdown.OptionData(file.Name);
            filePathDropdown.options.Add(optionData);
        }
    }

    public void Play()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Game");
    }

    public void ShowLoadMenu()
    {
        FillFilePathDropdown();
        loadMenuHolder.SetActive(true);
    }

    public void LoadGame()
    {
        string fileName = filePathDropdown.options[filePathDropdown.value].text;
        GameService.LoadGame(fileName);

        OptionsMenu.IsGameLoaded = true;
        map.GetComponent<MapGenerator>().GenerateMap();

        loadMenuHolder.SetActive(false);
    }

    public void Options()
    {
        mainMenuHolder.SetActive(false);
        optionsMenuHolder.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
