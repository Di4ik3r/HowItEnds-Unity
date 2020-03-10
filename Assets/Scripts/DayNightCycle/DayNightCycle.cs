using Assets.Scripts.DayNightCycle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DayNightCycle : MonoBehaviour
{
    [Header("Time")]
    [SerializeField]
    private float _timeScale = 100f;
    [SerializeField]
    public bool pause = false;
    [SerializeField]
    public AnimationCurve timeCurve;
    private float timeCurveNormalization;

    [Header("Sun Light")]
    [SerializeField]
    private Transform dailyRotation;
    [SerializeField]
    private Light sun;
    private float intensity;
    [SerializeField]
    public float sunBaseIntensity = 1f;
    [SerializeField]
    public float sunVariation = 1.5f;
    [SerializeField]
    public Gradient sunColor = null;

    [Header("Seasonal Variables")]
    [SerializeField]
    private Transform sunSeasonalRotation;
    [SerializeField]
    [Range(-45f, 45f)]
    private float maxSeasonalTilt;

    [Header("Modules")]
    private List<DNModuleBase> moduleList = new List<DNModuleBase>();

    [Header("Output")]
    public Text clockText;
    public Text dayNumberText;
    public Text yearNumberText;

    private TimeHolder timeHolder = TimeHolder.Instance;

    void Start()
    {
        dayNumberText.text = "Day number: " + timeHolder.DayNumber;
        yearNumberText.text = "Year number: " + timeHolder.YearNumber;
        NormalTimeCurve();

        CreatureManager.Instance.CheckDeath(2);
    }

    void Update()
    {
        if (!PauseMenu.IsGamePaused)
        {
            UpdateTimeScale();
            UpdateTime();
        }

        AdjustSunRotation();
        SunIntensity();
        AdjustSunColor();
        UpdateClock();
        UpdateModules(); //will update modules each frame
    }

    private void UpdateTimeScale()
    {
        _timeScale = 24 / (timeHolder.DayLength / 60);
        _timeScale *= timeCurve.Evaluate(timeHolder.ElapsedTime / (timeHolder.DayLength * 60));//changes timescale based on time curve
        _timeScale /= timeCurveNormalization; //keeps day length at target value
    }

    private void NormalTimeCurve()
    {
        float stepSize = 0.01f;
        int numberSteps = Mathf.FloorToInt(1f / stepSize);
        float curveTotal = 0;

        for (int i = 0; i < numberSteps; i++)
        {
            curveTotal += timeCurve.Evaluate(i * stepSize);
        }

        timeCurveNormalization = curveTotal / numberSteps; //keeps day length at target value
    }

    private void UpdateTime()
    {
        timeHolder.TimeOfDay += Time.deltaTime * _timeScale / 86400; // seconds in a day
        timeHolder.ElapsedTime += Time.deltaTime;
        if (timeHolder.TimeOfDay > 1) //new day!!
        {
            timeHolder.ElapsedTime = 0;
            timeHolder.DayNumber++;
            dayNumberText.text = "Day number: " + timeHolder.DayNumber;
            timeHolder.TimeOfDay -= 1;
            CreatureManager.Instance.CheckDeath(timeHolder.DayNumber);

            if (timeHolder.DayNumber > timeHolder.YearLength) //new year!
            {
                timeHolder.YearNumber++;
                yearNumberText.text = "Year number: " + timeHolder.YearNumber;
                timeHolder.DayNumber = 0;
            }
        }
    }

    private void UpdateClock()
    {
        float time = timeHolder.ElapsedTime / (timeHolder.DayLength * 60);
        float hour = Mathf.FloorToInt(time * 24);
        float minute = Mathf.FloorToInt(((time * 24) - hour) * 60);

        string hourString;
        string minuteString;

        if (!timeHolder.Use24Hours && hour > 12)
            hour -= 12;

        if (hour < 10)
            hourString = "0" + hour.ToString();
        else
            hourString = hour.ToString();

        if (minute < 10)
            minuteString = "0" + minute.ToString();
        else
            minuteString = minute.ToString();

        if (timeHolder.Use24Hours)
            clockText.text = "Time: " + hourString + " : " + minuteString;
        else if (time > 0.5f)
            clockText.text = "Time: " + hourString + " : " + minuteString + " pm";
        else
            clockText.text = "Time: " + hourString + " : " + minuteString + " am";
    }

    //rotates the sun daily (and seasonally soon too);
    private void AdjustSunRotation()
    {
        float sunAngle = timeHolder.TimeOfDay * 360f;
        dailyRotation.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, sunAngle));

        float seasonalAngle = -maxSeasonalTilt * Mathf.Cos(timeHolder.DayNumber / timeHolder.YearLength * 2f * Mathf.PI);
        sunSeasonalRotation.localRotation = Quaternion.Euler(new Vector3(seasonalAngle, 0f, 0f));
    }
    
    private void SunIntensity()
    {
        intensity = Vector3.Dot(sun.transform.forward, Vector3.down);
        intensity = Mathf.Clamp01(intensity);

        sun.intensity = intensity * sunVariation + sunBaseIntensity;
    }

    private void AdjustSunColor()
    {
        sun.color = sunColor.Evaluate(intensity);
    }

    public void AddModule(DNModuleBase module)
    {
        moduleList.Add(module);
    }

    public void RemoveModule(DNModuleBase module)
    {
        moduleList.Remove(module);
    }

    ////update each module based on current sun intensity
    private void UpdateModules()
    {
        foreach (DNModuleBase module in moduleList)
        {
            module.UpdateModule(intensity);
        }
    }
}
