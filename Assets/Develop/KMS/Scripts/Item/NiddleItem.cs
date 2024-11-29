using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NiddleItem : ItemBase
{
    private void Awake()
    {
        itemName = "바늘 아이템";
        itemType = E_ITEMTYPE.ActiveItem;
    }

    public override void ApplyEffect(GameObject player)
    {
        Bubble bubble = player.GetComponentInChildren<Bubble>();
        if (bubble != null)
        {
            // Bubble의 Save 함수 호출
            bubble.photonView.RPC(nameof(bubble.Save), RpcTarget.All);
            Debug.Log("바늘 아이템 사용: 물방울 상태에서 탈출");
        }
        else
        {
            Debug.Log("바늘 사용 실패: 물방울 상태가 아님.");
        }
    }
}
