using Photon.Pun;
using UnityEngine;

public class DartProjectile : MonoBehaviourPun
{
    [SerializeField] private float _speed = 15f;     // 다트의 속도
    [SerializeField] private float _lifetime = 5f;  // 다트의 수명
    private Vector3 _initialDirection;

    private void Start()
    {
        if (photonView.IsMine)
        {
            _initialDirection = transform.forward;
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

    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine) return;

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
}