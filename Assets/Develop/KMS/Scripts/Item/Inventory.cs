using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Inventory : MonoBehaviourPun
{
    public List<ItemBase> inventory = new List<ItemBase>();

    // 아이템 변경 이벤트
    public event Action<bool, ItemBase> OnItemChanged;

    /// <summary>
    /// 아이템 습득하는 메서드.
    /// </summary>
    /// <param name="item"></param>
    public void AddItem(ItemBase item)
    {
        if (item.itemType == E_ITEMTYPE.ActiveItem)
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

                // 이전 아이템 삭제 동기화
                oldItem.photonView.RPC(nameof(ItemBase.Interact), RpcTarget.AllBuffered);

                Debug.Log($"기존 아이템 {oldItem.itemName}이 삭제되었습니다.");
                EquipItem(item);
            }
        }
    }

    /// <summary>
    /// 아이템을 인벤토리에 위치하도록 하는 메서드.
    /// </summary>
    /// <param name="item"></param>
    private void EquipItem(ItemBase item)
    {
        inventory.Add(item);
        // 아이템의 위치를 모든 클라이언트에서 이동
        if (PhotonNetwork.IsMasterClient)
        {
            item.photonView.RPC(nameof(ItemBase.MoveItemToInventory_RPC), RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.ActorNumber);
        }
        Debug.Log($"아이템 {item.itemName}이 인벤토리에 추가되었습니다.");

        // UI 갱신 이벤트 호출 (아이템 추가)
        OnItemChanged?.Invoke(true, item);
    }

    /// <summary>
    /// 해당 아이템을 사용하는 메서드.
    /// </summary>
    /// <param name="index"></param>
    public void UseItem(int index)
    {
        if (index < inventory.Count)
        {
            ItemBase item = inventory[index];

            // 아이템 사용
            inventory[index].ApplyEffect(this.gameObject);
            inventory.RemoveAt(index);

            // UI 갱신 이벤트 호출 (아이템 제거)
            OnItemChanged?.Invoke(false, null);

            // 방장에게 삭제 요청
            if (!PhotonNetwork.IsMasterClient)
            {
                photonView.RPC(nameof(RequestItemDestruction_RPC), RpcTarget.MasterClient, item.photonView.ViewID);
            }
            else
            {
                PhotonNetwork.Destroy(item.gameObject);
            }
        }
        else
        {
            Debug.Log($"인벤토리에 아이템이 없습니다.");
        }
    }

    /// <summary>
    /// 아이템을 사용시 마스터 클라이언트에게 삭제를 요청하는 메서드.
    /// </summary>
    /// <param name="itemViewID"></param>
    [PunRPC]
    public void RequestItemDestruction_RPC(int itemViewID)
    {
        PhotonView itemView = PhotonView.Find(itemViewID);
        if (itemView != null && itemView.IsMine)
        {
            PhotonNetwork.Destroy(itemView.gameObject);
        }
    }
}
