using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<ItemBase> inventory = new List<ItemBase>();

    public void AddItem(ItemBase item)
    {
        if (item.itemType == ItemBase.E_ITEMTYPE.ActiveItem)
        {
            if (inventory.Count < 1)
            {
                EquipItem(item);
            }
            else
            {
                Debug.Log("인벤토리가 가득 찼습니다.");
                ItemBase oldItem = inventory[0];
                inventory.RemoveAt(0);

                // 원래 있던 아이템을 네트워크 상에서 삭제
                if (oldItem.photonView != null && oldItem.photonView.IsMine)
                {
                    PhotonNetwork.Destroy(oldItem.gameObject);
                }
                else
                {
                    Destroy(oldItem.gameObject); // 로컬 복제본 제거
                }

                Debug.Log($"기존 아이템 {oldItem.itemName}이 삭제되었습니다.");
                EquipItem(item);
            }
        }
    }

    private void EquipItem(ItemBase item)
    {
        inventory.Add(item);
        item.transform.position = new Vector3(100, 0, 100);
        Debug.Log($"아이템 {item.itemName}이 인벤토리에 추가되었습니다.");
    }


    public void UseItem(int index)
    {
        if (index < inventory.Count)
        {
            inventory[index].ApplyEffect(this.gameObject);
            inventory.RemoveAt(index);
        }
    }
}
