using Photon.Pun;
using UnityEngine;

public class DartProjectile : MonoBehaviourPun
{
    [SerializeField] private float _speed = 15f;    // 다트의 속도
    [SerializeField] private float _lifetime = 5f;  // 다트의 수명
    private Vector3 _initialDirection;              // 다트의 초기 방향
    private double _creationTime;                   // 다트 생성 시 서버 시간
    private Vector3 _startPosition;                 // 다트 시작 위치

    private void Start()
    {
        if (photonView.IsMine)
        {
            _initialDirection = transform.forward;
            _startPosition = transform.position;

            // 서버의 시간을 기록(별도의 RPC를 안쓰고 동기화 가능.)
            _creationTime = PhotonNetwork.Time;
            // 다트 일정 시간 후 자동 파괴
            Destroy(gameObject, _lifetime);
        }
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            // 초기 방향으로 다트를 이동
            transform.position += _initialDirection * _speed * Time.deltaTime;
        }
    }

    private void OnEnable()
    {
        if (!photonView.IsMine)
        {
            // 지연 보상 계산
            AdjustForLag();
        }
    }

    private void AdjustForLag()
    {
        // 생성 시점부터 현재까지 경과한 시간 계산
        double elapsedTime = PhotonNetwork.Time - _creationTime;

        // 다트의 위치를 경과 시간에 따라 보정
        transform.position = _startPosition + _initialDirection * _speed * (float)elapsedTime;

        Debug.Log($"Dart 지연 보상 적용. 경과 시간: {elapsedTime}s, 새 위치: {transform.position}");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine) return;
        if (other.gameObject.layer == LayerMask.NameToLayer("WaterBomb"))
        {
            // 물풍선과 충돌했을 경우
            WaterBomb waterBomb = other.GetComponent<WaterBomb>();
            if (waterBomb != null)
            {
                // 물풍선 터뜨리기
                waterBomb.Interact();

                // 다트를 파괴
                PhotonNetwork.Destroy(gameObject);
            }
        }
        else
        {
            Debug.Log("물풍선이 아니라서 동작이 안됩니다.");
        }
    }
}