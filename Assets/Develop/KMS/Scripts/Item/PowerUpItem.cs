using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpItem : ItemBase
{
    public int powerIncrease = 1;  // ¹°Ç³¼± ÆÄ¿ö(ÆøÆÄ¹üÀ§)

    public override void ApplyEffect(GameObject player)
    {
        PlayerStatus playerStatus = player.GetComponent<PlayerStatus>();
        if (playerStatus)
        {
            //playerStatus.power += powerIncrease;
        }
    }
}
