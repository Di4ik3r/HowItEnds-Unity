using Assets.Scripts.DayNightCycle;
using Assets.Scripts.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    //Map controls
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

    //Time controls
    public InputField dayLengthField;
    public InputField yearLengthField;
    public Toggle use24HoursToggle;

    //Creatures controls
    public static int GroundCreaturesCount = 0;
    public static int WaterCreaturesCount = 0;

    //UI controls
    public GameObject mainMenuHolder;
    public GameObject optionsMenuHolder;
    public GameObject map;

    //Holders
    private MapHolder mapHolder = MapHolder.getInstance();
    private TimeHolder timeHolder = TimeHolder.getInstance();    

    //Game controls
    public Toggle autosaveToggle;
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    public static bool IsGameLoaded = false;

    void Awake()
    {
        PrefillControls();
    }

    void Update()
    {
        if (IsGameLoaded)
        {
            PrefillControls();
            IsGameLoaded = false;
        }
    }

    public void PrefillControls()
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

        autosaveToggle.isOn = PlayerPrefs.GetInt("autosave") == 1 ? true : false;
        masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVol");
        musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVol");
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("SfxVol");
    }

    public void MainMenu()
    {
        mainMenuHolder.SetActive(true);
        optionsMenuHolder.SetActive(false);
    }

    public void SetMasterVolume(float value)
    {
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Master);
    }

    public void SetMusicVolume(float value)
    {
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Music);
    }

    public void SetSfxVolume(float value)
    {
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Sfx);
    }

    public void SetGroundCreaturesCount(float value)
    {
        GroundCreaturesCount = (int)value;
    }

    public void SetWaterCreaturesCount(float value)
    {
        WaterCreaturesCount = (int)value;
    }

    public void SetUseAutosave()
    {
        if (autosaveToggle.isOn)
        {
            PlayerPrefs.SetInt("autosave", 1);
        }
        else
        {
            PlayerPrefs.SetInt("autosave", 0);
        }        
    }

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
        if (octavesField.text != "")
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
}
