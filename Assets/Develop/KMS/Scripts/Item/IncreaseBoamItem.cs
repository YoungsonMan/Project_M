using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseBoamItem : ItemBase
{
    public int balloonCountIncrease = 1;  // 추가될 물풍선 개수

    private void Awake()
    {
        itemName = "물풍선 증가 아이템";
        itemType = E_ITEMTYPE.InstantItem;
    }

    public override void ApplyEffect(GameObject player)
    {
        Debug.Log("풍선의 갯수 증가 아이템을 습득했습니다.");
        PlayerStatus playerStatus = player.GetComponent<PlayerStatus>();
        if (playerStatus)
        {
            Debug.Log("풍선의 갯수가 증가 합니다.");
            playerStatus.bombCount += balloonCountIncrease;
        }
    }
}
