using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class World : MonoBehaviour
{
    /*Settings*/
    [Header("Day Night Cycle Settings")]
    public GameObject Sun;
    public List<Color> LightColors;
    //The current cycle progress
    [Range(0.0f, 1.0f)]
    public float CurrentTime = 0.47f;
    //Seconds in realtime it takes for a full day night cycle
    public float NightCycleTime = 20;
    public AnimationCurve IntensityCurve = AnimationCurve.Linear(0, 0, 1, 4);

    [Header("Weather Settings")]
    public List<Weather> Weathers = new List<Weather>();

    /*Info*/
    public static uint DayCount = 1;
    public static Weather CurrentWeather = new Weather();

    /*Data*/
    private float CycleTimer = 0;

    void Start()
    {
        CurrentWeather = Weathers[0];
        CycleTimer = CurrentTime * NightCycleTime;
    }

	void Update ()
    {
        DayCount += UpdateCycle();
        //CheckWeatherPercentages();
    }

    void OnNewDay()
    {
        PickWeather();
    }

    void OnNewNight()
    {

    }

    uint UpdateCycle()
    {
        uint day = 0;
        //Move forward in time
        CycleTimer += Time.deltaTime;
        if (CycleTimer >= NightCycleTime)
        {
            CycleTimer -= NightCycleTime;
            day += 1;
            OnNewDay();
        }

        if (Application.isPlaying) CurrentTime = CycleTimer / NightCycleTime;

        //Rotation
        transform.eulerAngles = Vector3.Lerp(new Vector3(0, 0, 0), new Vector3(0, 360 * 2, 0), CurrentTime);
        //Intensity
        Sun.GetComponent<Light>().intensity = IntensityCurve.Evaluate(CurrentTime);

        //Color
        if (LightColors.Count != 0)
        {
            float splitValue = 1.0f / LightColors.Count;
            int color1Index = Mathf.FloorToInt(CurrentTime / splitValue);
            int color2Index = color1Index + 1;
            if (color1Index >= LightColors.Count) color1Index -= LightColors.Count;
            if (color2Index >= LightColors.Count) color2Index -= LightColors.Count;

            Sun.GetComponent<Light>().color = Color.Lerp(LightColors[color1Index], LightColors[color2Index], (CurrentTime / splitValue) % 1);
        }
        return day;
    }

    void CheckWeatherPercentages()
    {
        float totalPercentage = 0;
        float weatherPercentage = 1 / (float)Weathers.Count;
        for (int i = 1; i < Weathers.Count; i++)
        {
            float successPercentage = 100 * (weatherPercentage * (1 / (float)Weathers[i].OneOutOf));
            totalPercentage += successPercentage;
            Debug.Log(Weathers[i].Name + ": " + successPercentage + "%");
        }
        Debug.Log(Weathers[0].Name + ": " + (100 - totalPercentage) + "%");
    }

    void PickWeather()
    {
        //Roll weather
        int weatherRoll = Random.Range((int)0, Weathers.Count);
        //Roll chance of success
        int success = Random.Range((int)0, Weathers[weatherRoll].OneOutOf);

        if (success == 0) CurrentWeather = Weathers[weatherRoll];
        else CurrentWeather = Weathers[0];
    }
}

