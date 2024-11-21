using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerStatus _status;

    [SerializeField] Rigidbody rigid;

    [SerializeField] Animator animator;

    private void Awake()
    {
        _status = GetComponent<PlayerStatus>();
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

        // (테스트) 플레이어 물풍선에 갇힘
        if(Input.GetKeyDown(KeyCode.Keypad0))
        {
            BindBubble();
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

    // 물풍선에 갇히는 기능
    public void BindBubble()
    {

    }

    // 풍선에 충돌을 감지하여 물풍선에 갇히게 한다.
    private void OnCollisionEnter(Collision collision)
    {
        BindBubble();
    }
}
