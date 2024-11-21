using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemBase : MonoBehaviour
{
    public string itemName;         // 아이템 이름
    public bool isPickup = false;   // 아이템이 픽업되었는지 여부

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어와 충돌시
        if (other.CompareTag("Player"))
        {
            isPickup = true;
            ApplyEffect(other.gameObject);
            OnPickedUp();
        }
        // 물줄기와 충돌시
        else if (other.CompareTag("WaterStream"))   // 임의로 현재 입력한 tag
        {
            OnHitByWaterStream();
        }
    }

    /// <summary>
    /// 아이템의 효과를 플레이어에 적용하는 메서드 (상속받아 구현)
    /// </summary>
    public abstract void ApplyEffect(GameObject player);

    /// <summary>
    /// 아이템이 픽업된 후 추가적으로 처리할 동작
    /// </summary>
    protected virtual void OnPickedUp()
    {
        // 픽업 후 아이템 오브젝트 제거
        Destroy(gameObject);
    }

    /// <summary>
    /// 물줄기에 의해 아이템이 제거되는 처리
    /// </summary>
    protected virtual void OnHitByWaterStream()
    {
        Debug.Log($"{itemName}이(가) 물줄기에 의해 제거되었습니다.");
        Destroy(gameObject);
    }
}
