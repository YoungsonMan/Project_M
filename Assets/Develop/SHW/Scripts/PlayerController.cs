using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerStatus _status;

    [SerializeField] Rigidbody rigid;

    [SerializeField] Animator animator;

    // public bool isBubble;
    [SerializeField] GameObject bubble;

    private void Awake()
    {
        _status = GetComponent<PlayerStatus>();
        _status.isBubble = false;
        bubble.SetActive(false);
    }

    private void Start()
    {

    }

    private void Update()
    {
        // 플레이어 소유권자 일경우
        Move();

        // 폭탄 설치
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetBoom();
        }
    }

    public void Move()
    {
        Vector3 moveDir = new Vector3();
        moveDir.x = Input.GetAxisRaw("Horizontal");
        moveDir.z = Input.GetAxisRaw("Vertical");

        // 이동시 애니메이션 출력
        if (moveDir.x != 0 || moveDir.z != 0)
        {
            animator.SetBool("Move", true);
        }
        else
        {
            animator.SetBool("Move", false);
        }

        // 계속 이동 방지
        if (moveDir.magnitude < 0.1)
        {
            rigid.velocity = Vector3.zero;
        }

        // 동시 입력 시 다른 방향 움직임 제한
        if (moveDir.x != 0)
        {
            moveDir.z = 0;
        }
        else if (moveDir.z != 0)
        {
            moveDir.x = 0;
        }

        // 리지드 바디로 이동
        rigid.velocity = moveDir.normalized * _status.speed;
        transform.forward = moveDir;
    }

    public void SetBoom()
    {
        // TODO : 폭탄 설치
    }

    // 충돌 감지
    private void OnCollisionEnter(Collision collision)
    {
        // 물줄기에 닿았을 경우
        if (collision.gameObject.name == "test")
        {
            Debug.Log("물방울에 갇힘!");
            _status.isBubble = true;

            // 이동 함수 실행 중 넘어왔을 경우의 초기화?
            //rigid.velocity = Vector3.zero;
            //animator.SetBool("Move", false);

            // 갇혔을때 느리게 이동을 위한 

            bubble.SetActive(true);
        }

    }
}
