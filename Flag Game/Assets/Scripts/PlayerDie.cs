using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDie : MonoBehaviourPunCallbacks
{
    void OnDestroy()
    {
        Debug.Log("OnDestroy method called");
        TakeFlag takeFlagComponent = GetComponentInChildren<TakeFlag>();
        if (takeFlagComponent != null)
        {
            Debug.Log("Has flag");
            takeFlagComponent.transform.parent = null;
            Vector3 newFlagPosition = takeFlagComponent.transform.position;
            newFlagPosition.y = 0.3f;
            takeFlagComponent.transform.position = newFlagPosition;
        }

    }

}
