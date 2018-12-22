using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Displays the slider's value as text
public class DisplaySliderValue : MonoBehaviour
{
    /*Settings*/
    public Slider TheSlider;

    /*Required Components*/
    private Text TheText;

    void Start()
    {
        TheText = GetComponent<Text>();
    }

	void Update ()
    {
        TheText.text = TheSlider.value.ToString();
    }
}
