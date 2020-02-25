using Assets.Scripts.Map;
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
    public GameObject map;

    public InputField widthField;
    public InputField lenghtField;

    private MapHolder Map = MapHolder.getInstance();

    public void Play()
    {
        SceneManager.LoadScene("Game");
    }

    public void SetWidth()
    {        
        Map.Width = int.Parse(widthField.text);
        map.GetComponent<MapGenerator>().GenerateMap();                          
    }

    public void SetLenght()
    {        
        Map.Lenght = int.Parse(lenghtField.text);
        map.GetComponent<MapGenerator>().GenerateMap();
    }

    public void SetHighDifference(float value)
    {        
        Map.HeightDifference = value;
        map.GetComponent<MapGenerator>().GenerateMap();
    }

    public void SetDecorationPercent(float value)
    {
        Map.DecorationPercent = value;
        map.GetComponent<MapGenerator>().GenerateMap();
    }

    public void SetFoodPercent(float value)
    {
        Map.FoodPercent = value;
        map.GetComponent<MapGenerator>().GenerateMap();
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
