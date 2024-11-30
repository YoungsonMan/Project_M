using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DartItem : ItemBase
{
    [SerializeField] private GameObject _dartPrefab;  // 다트 프리팹

    private void Awake()
    {
        itemName = "다트 아이템";
        itemType = E_ITEMTYPE.ActiveItem;
    }

    public override void ApplyEffect(GameObject player)
    {
        // MuzzlePoint 가져오기
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController == null || playerController.muzzlePoint == null)
        {
            Debug.LogError("MuzzlePoint가 설정되지 않았습니다.");
            return;
        }

        // 다트를 MuzzlePoint에서 생성
        Vector3 spawnPosition = playerController.muzzlePoint.position;
        Quaternion spawnRotation = Quaternion.LookRotation(playerController.muzzlePoint.forward);

        // 다트를 네트워크 상에서 생성
        GameObject dart = PhotonNetwork.Instantiate($"Item/{_dartPrefab.name}", spawnPosition, spawnRotation);

        Debug.Log("다트 아이템 사용: 다트를 발사했습니다.");
    }
}
