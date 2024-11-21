using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedUpItem : ItemBase
{
    public float speedIncreaseAmount = 2.0f;    // 속도 증가량

    public override void ApplyEffect(GameObject player)
    {
        Debug.Log("스피드 아이템을 습득했습니다.");
        PlayerStatus playerStatus = player.GetComponent<PlayerStatus>();
        if (playerStatus)
        {
            Debug.Log("스피드가 증가 합니다.");
            //playerStatus.speed += speedIncreaseAmount;
        }
    }
}
