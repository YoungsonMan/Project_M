using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpItem : ItemBase
{
    public int powerIncrease = 1;  // 물풍선 파워(폭파범위)

    private void Awake()
    {
        itemName = "물풍선 파워 증가 아이템";
        itemType = E_ITEMTYPE.InstantItem;
    }

    public override void ApplyEffect(GameObject player)
    {
        Debug.Log("파워 아이템을 습득했습니다.");
        PlayerStatus playerStatus = player.GetComponent<PlayerStatus>();
        if (playerStatus)
        {
            Debug.Log("파워가 증가 합니다.");
            playerStatus.power += powerIncrease;
        }
    }
}
