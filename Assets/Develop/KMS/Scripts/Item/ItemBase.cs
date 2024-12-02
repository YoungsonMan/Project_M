using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemBase : MonoBehaviourPun, IExplosionInteractable
{
    public E_ITEMTYPE itemType;     // 현재 아이템의 타입

    public string itemName;         // 아이템 이름
    public bool isPickup = false;   // 아이템이 픽업되었는지 여부

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"{other.name}이 아이템을 습득했습니다.");
            PhotonView playerView = other.GetComponent<PhotonView>();

            if (playerView != null && playerView.IsMine)
            {
                // 아이템 획득 사운드
                SoundManager.Instance.PlaySFX(SoundManager.E_SFX.GET_ITEM);

                // 즉각 사용 아이템
                if (itemType == E_ITEMTYPE.InstantItem)
                {
                    photonView.RPC(nameof(OnPickedUp_RPC), RpcTarget.AllBuffered, playerView.ViewID);
                }
                // 동작 필요 아이템(인벤토리에 추가)
                else if (itemType == E_ITEMTYPE.ActiveItem)
                {
                    playerView.GetComponent<Inventory>().AddItem(this);

                    // 아이템을 모든 클라이언트에서 이동시킴
                    photonView.RPC(nameof(MoveItemToInventory_RPC), RpcTarget.AllBuffered, playerView.ViewID);
                }
                //photonView.RPC(nameof(OnPickedUp_RPC), RpcTarget.AllBuffered, playerView.ViewID);
            }
        }
    }

    /// <summary>
    /// 아이템의 효과를 플레이어에 적용하는 메서드 (상속받아 구현)
    /// </summary>
    public abstract void ApplyEffect(GameObject player);

    /// <summary>
    /// 아이템이 픽업된 후 추가적으로 처리할 동작
    /// </summary>
    [PunRPC]
    protected virtual void OnPickedUp_RPC(int playerViewID)
    {
        PhotonView playerPhotonView = PhotonView.Find(playerViewID);
        if (playerPhotonView != null)
        {
            GameObject player = playerPhotonView.gameObject;
            ApplyEffect(player);
        }

        // 모든 클라이언트에서 동기화된 아이템 삭제
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    /// <summary>
    /// 아이템을 인벤토리로 이동
    /// </summary>
    [PunRPC]
    public virtual void MoveItemToInventory_RPC(int playerViewID)
    {
        if (PhotonView.Find(playerViewID) != null)
        {
            // 아이템 위치 이동
            transform.position = new Vector3(100, 0, 100);
        }
    }

    /// <summary>
    /// 물줄기에 의해 아이템이 제거되는 처리
    /// </summary>
    protected virtual void OnHitByWaterStream()
    {
        Debug.Log($"{itemName}이(가) 물줄기에 의해 제거되었습니다.");

        if (photonView.IsMine)
            PhotonNetwork.Destroy(gameObject);
    }

    public virtual bool Interact()
    {
        OnHitByWaterStream();
        return true;
    }
}
