using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PushableObject : MonoBehaviourPun
{
    [SerializeField] private float _pushSpeed = 3f; // 미는 속도

    private Vector3 targetPosition;                 // 목표 위치
    private bool isMoving = false;                  // 이동 중인지 여부

    private ProceduralDestruction destructionScript;

    private void Awake()
    {
        // 초기 위치를 정수로 맞춤
        targetPosition = RoundToGrid(transform.position);
        transform.position = targetPosition;
    }

    public void Push(Vector3 direction)
    {
        if (!isMoving)
        {
            Vector3 potentialTarget = targetPosition + direction;

            // 이동 가능 여부 체크
            if (CanMoveTo(potentialTarget))
            {
                targetPosition = potentialTarget;
                isMoving = true;

                // RPC 호출로 모든 클라이언트에서 이동 동기화
                photonView.RPC(nameof(PushRPC), RpcTarget.All, targetPosition);
            }
        }
    }

    [PunRPC]
    private void PushRPC(Vector3 newTargetPosition)
    {
        StartCoroutine(MoveObject(newTargetPosition));
    }

    private IEnumerator MoveObject(Vector3 newTargetPosition)
    {
        while (Vector3.Distance(transform.position, newTargetPosition) > 0.01f)
        {
            // 위치를 이동
            transform.position = Vector3.MoveTowards(transform.position, newTargetPosition, _pushSpeed * Time.deltaTime);
            yield return null;
        }

        // 목표 위치로 고정
        transform.position = RoundToGrid(newTargetPosition);
        isMoving = false;
    }

    private bool CanMoveTo(Vector3 position)
    {
        // 충돌 체크 (예: 플레이어, 벽 등)
        // 해당 위치에 충돌체가 있는지 확인
        Collider[] colliders = Physics.OverlapSphere(position, 0.4f);
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Obstacle") || collider.CompareTag("Box") || collider.CompareTag("Player"))
            {
                // 이동 불가능
                return false;
            }
        }
        return true;
    }

    private Vector3 RoundToGrid(Vector3 position)
    {
        // 위치를 정수 좌표로 맞춤
        return new Vector3(Mathf.Round(position.x), position.y, Mathf.Round(position.z));
    }
}
