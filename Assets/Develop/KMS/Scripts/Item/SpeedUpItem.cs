using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedUpItem : ItemBase
{
    public float speedIncreaseAmount = 2.0f;    // 속도 증가량

    public override void ApplyEffect(GameObject player)
    {
        PlayerStatus playerStatus = player.GetComponent<PlayerStatus>();
        if (playerStatus)
        {
            //playerStatus.speed += speedIncreaseAmount;
        }
    }
}
