using Photon.Pun;
using UnityEngine;

public class DartProjectile : MonoBehaviourPun
{
    [SerializeField] private float _speed = 15f;    // 다트의 속도
    [SerializeField] private float _lifetime = 5f;  // 다트의 수명
    [SerializeField] private LayerMask _playerLayer;// 플레이어 레이어 설정
    private Vector3 _direction;                     // 다트의 이동 방향
    private Vector3 _startPosition;                 // 다트의 시작 위치
    private double _creationTime;                   // 다트 생성 시 서버 시간

    private bool _isInitialized = false;            // 초기화 여부

    private void Start()
    {
        // 소유자 여부와 관계없이 파괴는 모든 클라이언트에서 자동으로 발생
        Destroy(gameObject, _lifetime);
    }

    private void Update()
    {
        if (_isInitialized)
        {
            // 다트를 이동
            transform.position += _direction * _speed * Time.deltaTime;
        }
    }

    private void OnEnable()
    {
        if (!_isInitialized)
        {
            AdjustForLag(); // 지연 보상 적용
        }
    }

    /// <summary>
    /// 지연 보상을 적용하여 다트 위치를 보정합니다.
    /// </summary>
    private void AdjustForLag()
    {
        if (!photonView.IsMine)
        {
            // 서버 시간 기준으로 경과 시간 계산
            double elapsedTime = PhotonNetwork.Time - _creationTime;

            // 다트의 위치를 경과 시간에 따라 이동
            transform.position += _direction * _speed * (float)elapsedTime;
            Debug.Log($"지연 보상 적용 완료: {elapsedTime}s 경과, 새 위치: {transform.position}");
        }
    }

    /// <summary>
    /// 다트를 초기화합니다.
    /// </summary>
    /// <param name="startPosition">시작 위치</param>
    /// <param name="direction">이동 방향</param>
    /// <param name="creationTime">생성 시 서버 시간</param>
    [PunRPC]
    public void Initialize(Vector3 startPosition, Vector3 direction, double creationTime)
    {
        _startPosition = startPosition;
        _direction = direction.normalized;
        _creationTime = creationTime;
        transform.position = startPosition;
        transform.rotation = Quaternion.LookRotation(direction);
        _isInitialized = true;

        Debug.Log($"다트 초기화 완료: 시작 위치 {startPosition}, 방향 {direction}, 생성 시간 {creationTime}");
    }

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어 레이어와 충돌 시 무시
        if (((1 << other.gameObject.layer) & _playerLayer) != 0)
        {
            Debug.Log("플레이어와의 충돌 무시");
            return;
        }

        // 충돌 시 동작 처리
        WaterBomb waterBomb = other.GetComponent<WaterBomb>();
        if (waterBomb != null)
        {
            waterBomb.Interact(); // 물풍선 터뜨리기
        }

        Destroy(gameObject); // 다트 파괴
    }
}
