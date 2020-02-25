using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public GameObject mainMenuHolder;
    public GameObject optionsMenuHolder;
    public GameObject backMap;

    public InputField widthField;
    public InputField lenghtField;

    private MapGenerator mapGenerator;

    public void Play()
    {
        SceneManager.LoadScene("Game");
    }

    public void SetWidth()
    {
        print("Width " + int.Parse(widthField.text));
        //backMap.GetComponent()
        mapGenerator.GenerateMap();
    }

    public void SetLenght()
    {
        print("Lenght " + int.Parse(lenghtField.text));
    }

    public void SetHighDifference(float value)
    {
        print("High difference " + value);
    }

    public void SetDecorationPercent(float value)
    {
        print("Decaration percent " + value);
    }

    public void SetFoodPercent(float value)
    {
        print("Food percent " + value);
        mapGenerator.GenerateMap();
    }

    public void Options()
    {
        print("Stone clicked");
        print(mainMenuHolder.name);
        print(optionsMenuHolder.name);
        mainMenuHolder.SetActive(false);
        optionsMenuHolder.SetActive(true);
    }

    public void MainMenu()
    {
        mainMenuHolder.SetActive(true);
        optionsMenuHolder.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
