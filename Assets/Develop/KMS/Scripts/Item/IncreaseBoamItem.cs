using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseBoamItem : ItemBase
{
    public int balloonCountIncrease = 1;  // 추가될 물풍선 개수

    public override void ApplyEffect(GameObject player)
    {
        PlayerStatus playerStatus = player.GetComponent<PlayerStatus>();
        if (playerStatus)
        {
            //playerStatus.bombCount += balloonCountIncrease;
        }
    }
}
