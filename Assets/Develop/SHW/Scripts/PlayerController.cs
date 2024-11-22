using UnityEngine;

public class PlayerController : MonoBehaviour, IExplosionInteractable
{
    private PlayerStatus _status;           // 플레이어 스탯 가져옴

    [SerializeField] Rigidbody rigid;       // 이동을 위한 rigidbody

    [SerializeField] Animator animator;     // 플레이어 애니메이션 실행

    [SerializeField] GameObject bubble;     // 물줄기에 맏고 갇힐 물방울

    // 물방울 안에 갇혔을 때 속도 느리게 설정
    private float bubbleSpeed = 0.5f;

    private void Awake()
    {
        _status = GetComponent<PlayerStatus>();
        _status.isBubble = false;
        bubble.SetActive(false);
    }

    private void Update()
    {
        // TODO : 플레이어 소유권자일 경우의 조건문 추가 필요
        Move();
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
        //if (moveDir.magnitude < 0.1)
        //{
        //    rigid.velocity = Vector3.zero;
        //}

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
        if (_status.isBubble == true)
        {
            rigid.velocity = moveDir.normalized * bubbleSpeed;
        }
        else
        {
            rigid.velocity = moveDir.normalized * _status.speed;
        }
        transform.forward = moveDir;
    }

    public bool Interact()
    {
        // Debug.Log("물방울에 갇힘!");

        _status.isBubble = true;
        bubble.SetActive(true);

        return false;
    }
}
