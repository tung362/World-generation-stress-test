using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class FadeInOut : MonoBehaviour
{
    /*Settings*/
    public enum FadeColors { None, FadeIn, FadeOut }
    public FadeColors StartColor;
    public bool FadeIn = true;
    public Color FadeInColor;
    public Color FadeOutColor;
    public float Duration = 1;

    /*Data*/
    private bool RunOnce = true;
    private float CurrentTimer = 0;


    /*Callable functions*/
    public UnityEvent OnFadeIn;
    public UnityEvent OnFadeOut;

    /*Required components*/
    private Image TheImage;
    private Text TheText;

    void Awake()
    {
        TheImage = GetComponent<Image>();
        TheText = GetComponent<Text>();

        switch (StartColor)
        {
            case FadeColors.None:
                break;
            case FadeColors.FadeIn:
                if (TheImage != null) TheImage.color = FadeInColor;
                if (TheText != null) TheText.color = FadeInColor;
                break;
            case FadeColors.FadeOut:
                if (TheImage != null) TheImage.color = FadeOutColor;
                if (TheText != null) TheText.color = FadeOutColor;
                break;
            default:
                break;
        }
    }

    void Start()
    {
        if (FadeIn) RunOnce = true;
        else RunOnce = false;
    }

    void Update()
    {
        FadeUpdate();
    }

    void FadeUpdate()
    {
        if (FadeIn)
        {
            if (RunOnce)
            {
                CurrentTimer += Time.deltaTime;
                if (CurrentTimer > Duration) CurrentTimer = Duration;
                float progress = CurrentTimer / Duration;

                if(TheText != null)
                {
                    if (TheText.color != FadeInColor) TheText.color = Color.Lerp(FadeOutColor, FadeInColor, progress);

                    if (TheText.color == FadeInColor)
                    {
                        OnFadeIn.Invoke();
                        CurrentTimer = 0;
                        RunOnce = false;
                    }
                }

                if (TheImage != null)
                {
                    if (TheImage.color != FadeInColor) TheImage.color = Color.Lerp(FadeOutColor, FadeInColor, progress);

                    if (TheImage.color == FadeInColor)
                    {
                        OnFadeIn.Invoke();
                        CurrentTimer = 0;
                        RunOnce = false;
                    }
                }
            }
        }
        else
        {
            if (!RunOnce)
            {
                CurrentTimer += Time.deltaTime;
                if (CurrentTimer > Duration) CurrentTimer = Duration;
                float progress = CurrentTimer / Duration;

                if (TheText != null)
                {
                    if (TheText.color != FadeOutColor) TheText.color = Color.Lerp(FadeInColor, FadeOutColor, progress);

                    if (TheText.color == FadeOutColor)
                    {
                        OnFadeOut.Invoke();
                        CurrentTimer = 0;
                        RunOnce = true;
                    }
                }

                if (TheImage != null)
                {
                    if (TheImage.color != FadeOutColor) TheImage.color = Color.Lerp(FadeInColor, FadeOutColor, progress);

                    if (TheImage.color == FadeOutColor)
                    {
                        OnFadeOut.Invoke();
                        CurrentTimer = 0;
                        RunOnce = true;
                    }
                }
            }
        }
    }

    public void Toggle()
    {
        CurrentTimer = 0;
        FadeIn = !FadeIn;
        if (FadeIn) RunOnce = true;
        else RunOnce = false;
    }

    public void ForceToggleFadeIn()
    {
        CurrentTimer = 0;
        FadeIn = true;
        RunOnce = true;
    }

    public void ForceToggleFadeOut()
    {
        CurrentTimer = 0;
        FadeIn = false;
        RunOnce = false;
    }
}
