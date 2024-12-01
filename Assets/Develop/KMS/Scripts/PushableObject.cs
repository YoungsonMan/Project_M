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
    private PhotonView pushingPlayer = null;        // 현재 밀고 있는 플레이어
    private float contactTime = 0f;                 // 플레이어와의 접촉 시간
    private const float requiredContactTime = 0.5f; // 밀기 위한 최소 접촉 시간



    private void Awake()
    {
        // 초기 위치를 정수로 맞춤
        targetPosition = RoundToGrid(transform.position);
        transform.position = targetPosition;
    }

    private void Update()
    {
        // 밀고 있는 플레이어가 없으면 접촉 시간 초기화
        if (pushingPlayer == null)
        {
            contactTime = 0f;
        }
    }

    public void Push(PhotonView playerView, Vector3 direction)
    {
        // 이미 이동 중이면 무시
        if (isMoving) return;

        // 플레이어가 동일한 방향으로 계속 밀고 있는지 확인
        if (pushingPlayer == null || pushingPlayer != playerView)
        {
            pushingPlayer = playerView;
            contactTime = 0f; // 새로운 플레이어가 밀 경우 접촉 시간 초기화
        }
        contactTime += Time.deltaTime;

        // 최소 접촉 시간이 충족되었는지 확인
        if (contactTime >= requiredContactTime)
        {
            Vector3 potentialTarget = targetPosition + direction;

            if (CanMoveTo(potentialTarget))
            {
                targetPosition = potentialTarget;
                isMoving = true;
                contactTime = 0f; // 접촉 시간 초기화

                // RPC 호출로 모든 클라이언트에서 이동 동기화
                photonView.RPC(nameof(PushRPC), RpcTarget.All, targetPosition);
            }
        }
    }

    [PunRPC]
    private void PushRPC(Vector3 newTargetPosition)
    {
        // 이동 상태를 모든 클라이언트에서 동기화
        isMoving = true;

        StartCoroutine(MoveObject(newTargetPosition));
    }

    private IEnumerator MoveObject(Vector3 newTargetPosition)
    {
        while (Vector3.Distance(transform.position, newTargetPosition) > 0.01f)
        {
            // 위치를 이동
            Debug.Log("이동");
            transform.position = Vector3.MoveTowards(transform.position, newTargetPosition, _pushSpeed * Time.deltaTime);
            yield return null;
        }

        // 목표 위치로 고정
        transform.position = RoundToGrid(newTargetPosition);
        isMoving = false;
        pushingPlayer = null; // 밀기 완료 후 밀고 있는 플레이어 초기화
    }

    private bool CanMoveTo(Vector3 position)
    {
        // 충돌 체크 (예: 플레이어, 벽 등)
        // 해당 위치에 충돌체가 있는지 확인
        Collider[] colliders = Physics.OverlapSphere(position, 0.4f);
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Obstacle") 
                || collider.CompareTag("Box") 
                || collider.CompareTag("Player") 
                || collider.gameObject.layer == LayerMask.NameToLayer("WaterBomb"))
            {
                Debug.Log($"collider tag {collider.tag}");
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
