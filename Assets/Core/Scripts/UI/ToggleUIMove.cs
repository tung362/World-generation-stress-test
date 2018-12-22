using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Move UI back and forth
public class ToggleUIMove : MonoBehaviour
{
    public bool Toggle = false;
    public Vector3 FalseTarget = Vector3.zero;
    public Vector3 TrueTarget = Vector3.zero;
    public float Speed = 1000;

    private RectTransform TheRectTransform;

    void Start()
    {
        TheRectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (!Toggle) TheRectTransform.anchoredPosition = Vector3.MoveTowards(TheRectTransform.anchoredPosition, FalseTarget, Speed * Time.deltaTime);
        else TheRectTransform.anchoredPosition = Vector3.MoveTowards(TheRectTransform.anchoredPosition, TrueTarget, Speed * Time.deltaTime);
    }

    public void ToggleTrueFalse()
    {
        Toggle = !Toggle;
    }
}
