using UnityEngine;
using System.Collections;

public class SetOnEndOfAnimation : StateMachineBehaviour
{
    [Header("The Type Of Edit (Only have one toggled)")]
    public bool IsBool = false;
    public bool IsInt = false;
    public bool IsFloat = false;

    [Header("The Value To Edit To (Only use the correct type)")]
    public bool BoolValue = false;
    public int IntValue = 0;
    public float FloatValue = 0;

    [Header("Name of the paremeter that is going to be edited")]
    public string ParameterName;

    private bool RunOnce = true;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        RunOnce = true;
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (/*animator.GetNextAnimatorStateInfo(0).fullPathHash != stateInfo.fullPathHash && */stateInfo.normalizedTime >= 1 /*&& RunOnce*/)
        {
            if (IsBool) animator.SetBool(ParameterName, BoolValue);
            if (IsInt) animator.SetInteger(ParameterName, IntValue);
            if (IsFloat) animator.SetFloat(ParameterName, FloatValue);
            RunOnce = false;
        }
    }
}