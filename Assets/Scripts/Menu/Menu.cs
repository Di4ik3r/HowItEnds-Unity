using Assets.Scripts.DayNightCycle;
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
    public InputField seedField;
    public InputField noiseScaleField;
    public InputField octavesField;
    public InputField offsetXField;
    public InputField offsetYField;
    public Slider highDifferenceSlider;
    public Slider foodPercentSlider;
    public Slider decorationPercentSlider;

    public InputField dayLengthField;
    public InputField yearLengthField;
    public Toggle use24HoursToggle;

    private MapHolder mapHolder = MapHolder.getInstance();
    private TimeHolder timeHolder = TimeHolder.getInstance();

    void Awake()
    {
        widthField.text = mapHolder.Width.ToString();
        lenghtField.text = mapHolder.Lenght.ToString();
        seedField.text = mapHolder.Seed.ToString();
        noiseScaleField.text = mapHolder.NoiseScale.ToString();
        octavesField.text = mapHolder.Octaves.ToString();
        offsetXField.text = mapHolder.OffsetX.ToString();
        offsetYField.text = mapHolder.OffsetY.ToString();
        highDifferenceSlider.value = mapHolder.HeightDifference;
        foodPercentSlider.value = mapHolder.FoodPercent;
        decorationPercentSlider.value = mapHolder.DecorationPercent;

        dayLengthField.text = timeHolder.DayLength.ToString();
        yearLengthField.text = timeHolder.YearLength.ToString();
        use24HoursToggle.isOn = timeHolder.Use24Hours;
    }

    public void Play()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Game");
    }

    public void SetGroundCreaturesCount(){}

    public void SetWaterCreaturesCount(){}

    public void SetUse24Hours()
    {
        timeHolder.Use24Hours = use24HoursToggle.isOn;
    }

    public void SetDayLength()
    {
        if (dayLengthField.text != "")
        {
            timeHolder.DayLength = int.Parse(dayLengthField.text);            
        }
    }

    public void SetYearLength()
    {
        if (yearLengthField.text != "")
        {
            timeHolder.YearLength = int.Parse(yearLengthField.text);
        }
    }

    public void SetWidth()
    {
        if (widthField.text != "")
        {
            mapHolder.Width = int.Parse(widthField.text);
            map.GetComponent<MapGenerator>().GenerateMap();                          
        }
    }

    public void SetLenght()
    {
        if (lenghtField.text != "")
        {
            mapHolder.Lenght = int.Parse(lenghtField.text);
            map.GetComponent<MapGenerator>().GenerateMap();
        }
    }

    public void SetSeed()
    {
        if (seedField.text != "")
        {
            mapHolder.Seed = int.Parse(seedField.text);
            map.GetComponent<MapGenerator>().GenerateMap();
        }
    }

    public void SetOctaves()
    {       
        if(octavesField.text != "")
        {
            mapHolder.Octaves = int.Parse(octavesField.text);
            map.GetComponent<MapGenerator>().GenerateMap();
        }
    }

    public void SetNoiseScale()
    {
        if (noiseScaleField.text != "")
        {
            mapHolder.NoiseScale = int.Parse(noiseScaleField.text);
            map.GetComponent<MapGenerator>().GenerateMap();
        }
    }

    public void SetOffsetX()
    {
        if (offsetXField.text != "")
        {
            mapHolder.OffsetX = int.Parse(offsetXField.text);
            map.GetComponent<MapGenerator>().GenerateMap();
        }
    }

    public void SetOffsetY()
    {
        if (offsetYField.text != "")
        {
            mapHolder.OffsetY = int.Parse(offsetYField.text);
            map.GetComponent<MapGenerator>().GenerateMap();
        }
    }

    public void SetHighDifference(float value)
    {
        mapHolder.HeightDifference = value;
        map.GetComponent<MapGenerator>().GenerateMap();
    }

    public void SetDecorationPercent(float value)
    {
        mapHolder.DecorationPercent = value;
        map.GetComponent<MapGenerator>().GenerateMap();
    }

    public void SetFoodPercent(float value)
    {
        mapHolder.FoodPercent = value;
        map.GetComponent<MapGenerator>().GenerateMap();
    }

    public void Options()
    {
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
