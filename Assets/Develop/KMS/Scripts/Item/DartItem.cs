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

        // 다트의 생성 위치와 방향
        Vector3 spawnPosition = playerController.muzzlePoint.position;
        Vector3 spawnDirection = playerController.muzzlePoint.forward;
        Debug.Log($"spawnDirection {spawnDirection}");

        // Y축 회전값 계산
        float yRotation = Mathf.Atan2(spawnDirection.x, spawnDirection.z) * Mathf.Rad2Deg; // Z축 기준 각도 계산
        Quaternion spawnRotation = Quaternion.Euler(0, yRotation, 0); // X, Z축은 0으로 고정하고 Y축만 회전
        Debug.Log($"yRotation {yRotation}");

        // 네트워크 상에서 다트를 생성
        GameObject dart = PhotonNetwork.Instantiate($"Item/{_dartPrefab.name}", spawnPosition, spawnRotation);

        // 모든 클라이언트에서 다트 초기화
        PhotonView dartPhotonView = dart.GetComponent<PhotonView>();
        dartPhotonView.RPC(nameof(DartProjectile.Initialize), RpcTarget.AllBuffered, spawnPosition, spawnDirection, PhotonNetwork.Time);

        Debug.Log("다트 아이템 사용: 다트를 발사했습니다.");
    }
}
