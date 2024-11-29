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
            bubble.StopAllCoroutines();
            bubble.bubble.SetActive(false);
            bubble.player.GetComponent<Animator>().SetBool("isBubble", false);
            bubble.player.GetComponent<PlayerStatus>().isBubble = false;
            bubble.player.GetComponent<WaterBombPlacer>().enabled = true;
        }
        else
        {
            Debug.Log("바늘 사용 실패: 물방울 상태가 아님.");
        }
    }
}
