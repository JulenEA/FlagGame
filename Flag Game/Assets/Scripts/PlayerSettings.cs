using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSettings : MonoBehaviour
{

    //Team: Indica a qué equipo pertenece el jugador
    public int playerTeam = 1;
    //Jugador
    public float playerSpeed = 6f;
    public float coolDownTime = 0.3f;
    //Bala
    public float bulletVelocity = 500f;
    public float bulletDestroyingTime = 3f;
    //Manejar la muerte
    private bool IsDead = false;


    //Team
    public int GetPlayerTeam()
    {
        return playerTeam;
    }

    //Player
    public float GetPlayerSpeed()
    {
        return playerSpeed;
    }

    public float GetCoolDownTime()
    {
        return coolDownTime;
    }


    //Bala
    public float GetBulletVelocity()
    {
        return bulletVelocity;
    }

    public float GetBulletDestroyingTime()
    {
        return bulletDestroyingTime;
    }

    public void KillPlayer()
    {
        IsDead = true;
    }

    public bool IsPlayerDead()
    {
        return IsDead;
    }

}
