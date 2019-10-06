using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeFlag : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            GrabFlag(other.gameObject);
        }
    }


    private void GrabFlag(GameObject pGrabber)
    {
        Vector3 grabbedFlagPosition = pGrabber.transform.position;
        grabbedFlagPosition.y = grabbedFlagPosition.y + 4;
        this.transform.parent = pGrabber.transform;
        this.transform.position = grabbedFlagPosition;
    }
}
