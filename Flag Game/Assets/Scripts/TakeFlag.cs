using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeFlag : MonoBehaviourPunCallbacks
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            if (!PhotonNetwork.IsConnected)
            {
                GrabFlag(other.gameObject.transform);
            }
            else
            {
                other.gameObject.GetPhotonView().RPC("GrabFlagRPC", RpcTarget.All, new object[] { other.gameObject.GetPhotonView().ViewID });
            }
        }
    }

    
    private void GrabFlag(Transform pGrabber)
    {
        Vector3 grabbedFlagPosition = pGrabber.position;
        grabbedFlagPosition.y = grabbedFlagPosition.y + 4;
        this.transform.parent = pGrabber;
        this.transform.position = grabbedFlagPosition;
    }

    [PunRPC]
    public void GrabFlagRPC(int pViewId)
    {
        Transform tGrabber = PhotonView.Find(pViewId).transform;
        GrabFlag(tGrabber);
    }
}
