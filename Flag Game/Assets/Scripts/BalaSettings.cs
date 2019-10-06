using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalaSettings : MonoBehaviour
{
    private int balaTeam;

    public int GetBalaTeam()
    {
        return balaTeam;
    }

    public void SetBalaTeam(int pTeam)
    {
        balaTeam = pTeam;
    }
}
