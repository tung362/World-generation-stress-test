using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputTarget : MonoBehaviour
{
    //public MenuButtonOptions TargetToTrack;
    public int TextID = 0;

    /*Required components*/
    private InputField TheInputField;

    void Start()
    {
        TheInputField = GetComponent<InputField>();
        if (TheInputField == null) return;
        //TheInputField.text = TargetToTrack.Texts[TextID];
    }

    void Update()
    {
        UpdateText();
    }

    void UpdateText()
    {
        if (TheInputField == null) return;
        //TargetToTrack.Texts[TextID] = TheInputField.text;
    }
}
