using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantWave : StateMachineBehaviour
{
    public float BendSmoothness = 0.3f;
    private Vector2 BendVelocity = Vector2.zero;

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Vector2 direction = new Vector2(animator.GetFloat("BendDirectionX"), animator.GetFloat("BendDirectionY"));
        direction = Vector2.SmoothDamp(direction, Vector2.zero, ref BendVelocity, BendSmoothness, Mathf.Infinity, Time.deltaTime);
        animator.SetFloat("BendDirectionX", direction.x);
        animator.SetFloat("BendDirectionY", direction.y);

        if(Input.GetKeyDown(KeyCode.L))
        {
            animator.SetFloat("BendDirectionX", 1);
            animator.SetFloat("BendDirectionY", 0);
        }
    }
}
