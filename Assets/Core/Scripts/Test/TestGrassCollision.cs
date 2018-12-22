using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGrassCollision : MonoBehaviour
{
    public float Radius = 1;

    //void FixedUpdate()
    //{
    //    Collider[] hitColliders = Physics.OverlapSphere(transform.position, Radius); //To do: add layermask
    //    for (int i = 0; i < hitColliders.Length; i++)
    //    {
    //        Vector3 entityPosition = new Vector3(transform.position.x, 0, transform.position.z);
    //        Vector3 plantPosition = new Vector3(hitColliders[i].transform.position.x, 0, hitColliders[i].transform.position.z);
    //        float distance = Vector3.Distance(entityPosition, plantPosition);
    //        Vector3 direction = (plantPosition - entityPosition).normalized;
    //        Animator plantAnimator = hitColliders[i].GetComponent<Animator>();
    //        float clampedPower = Mathf.Clamp(1 - (distance / Radius), 0, 1) * 2;
    //        if (plantAnimator != null) //Remove when layer mask is made
    //        {
    //            plantAnimator.SetFloat("BendDirectionX", direction.x * clampedPower);
    //            plantAnimator.SetFloat("BendDirectionY", direction.z * clampedPower);
    //        }
    //    }
    //}

    void OnTriggerStay(Collider other)
    {
        //Collider[] hitColliders = Physics.OverlapSphere(transform.position, Radius); //To do: add layermask
        //for (int i = 0; i < hitColliders.Length; i++)
        //{
        //    Vector3 entityPosition = new Vector3(transform.position.x, 0, transform.position.z);
        //    Vector3 plantPosition = new Vector3(hitColliders[i].transform.position.x, 0, hitColliders[i].transform.position.z);
        //    float distance = Vector3.Distance(entityPosition, plantPosition);
        //    Vector3 direction = (plantPosition - entityPosition).normalized;
        //    Animator plantAnimator = hitColliders[i].GetComponent<Animator>();
        //    float clampedPower = Mathf.Clamp(1 - (distance / Radius), 0, 1);
        //    if (plantAnimator != null) //Remove when layer mask is made
        //    {
        //        plantAnimator.SetFloat("BendDirectionX", direction.x * clampedPower);
        //        plantAnimator.SetFloat("BendDirectionY", direction.z * clampedPower);
        //    }
        //}

        Vector3 entityPosition = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 plantPosition = new Vector3(other.transform.position.x, 0, other.transform.position.z);
        float distance = Vector3.Distance(entityPosition, plantPosition);
        Vector3 direction = (plantPosition - entityPosition).normalized;
        Animator plantAnimator = other.GetComponent<Animator>();
        float clampedPower = Mathf.Clamp(1 - (distance / Radius), 0, 1) * (2 * (1 + Radius));
        if (plantAnimator != null) //Remove when layer mask is made
        {
            plantAnimator.SetFloat("BendDirectionX", direction.x * clampedPower);
            plantAnimator.SetFloat("BendDirectionY", direction.z * clampedPower);
        }
    }

    //void OnDrawGizmos()
    //{
    //    Gizmos.DrawWireSphere(transform.position, Radius);
    //}
}
