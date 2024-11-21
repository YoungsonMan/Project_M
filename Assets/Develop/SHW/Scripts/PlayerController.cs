using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerStatus _status;

    [SerializeField] Rigidbody rigid;

    [SerializeField] Animator animator;

    public bool isBubble;
    [SerializeField] GameObject bubble;

    private void Awake()
    {
        _status = GetComponent<PlayerStatus>();
        isBubble = false;
        bubble.SetActive(false);
    }

    private void Start()
    {

    }

    private void Update()
    {
        // 플레이어 소유권자 일경우
        if (isBubble == false)
        {
            Move();
        }

        // 폭탄 설치
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetBoom();
        }

        // (테스트) 폭탄에 갇힐 경우
        if(Input.GetKeyDown(KeyCode.B))
        {
            BindBubble();
        }
        // 임시
        if(Input.GetKeyDown(KeyCode.V))
        {
            isBubble = false;
            bubble.SetActive(false);
        }
    }

    public void Move()
    {
        if(isBubble == true) return;

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

    public void BindBubble()
    {
        Debug.Log("물방울에 갇힘!");
        isBubble = true;

        // 이동 함수 실행 중 넘어왔을 경우의 초기화?
        rigid.velocity = Vector3.zero;
        animator.SetBool("Move",false);

        // 물방울 활성화
        bubble.SetActive(true);
        
        // 활성화 동안 스피드 감소
        // n초 후 물방울 비활성화
        // 캐릭터 사망
    }

    // 충돌 감지
    private void OnCollisionEnter(Collision collision)
    {
        // 바닥과 충돌해도 실행되서 임시로 주석처리
        // BindBubble();
    }
}
