using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DartItem : ItemBase
{
    [SerializeField] private float _maxRange = 10f; // 다트가 닿을 수 있는 최대 거리
    [SerializeField] private LayerMask _waterBombLayer; // 물풍선만 감지하는 레이어


    private void Awake()
    {
        itemName = "다트 아이템";
        itemType = E_ITEMTYPE.ActiveItem;
    }

    public override void ApplyEffect(GameObject player)
    {
        RaycastHit hit;
        Vector3 playerPosition = player.transform.position;
        Vector3 forwardDirection = player.transform.forward;

        // 레이캐스트로 물풍선 감지
        if (Physics.Raycast(playerPosition + Vector3.up * 0.5f, forwardDirection, out hit, _maxRange, _waterBombLayer))
        {
            WaterBomb waterBomb = hit.collider.GetComponent<WaterBomb>();
            if (waterBomb != null)
            {
                // 물풍선 Interact 호출
                waterBomb.Interact();
                Debug.Log("다트 아이템 사용: 물풍선을 터뜨렸습니다.");
            }
            else
            {
                Debug.Log("다트 아이템 사용 실패: 물풍선을 찾지 못했습니다.");
            }
        }
        else
        {
            Debug.Log("다트 아이템 사용 실패: 범위 내에 물풍선이 없습니다.");
        }
    }
}
