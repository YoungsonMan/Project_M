using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float speed;       // 플레이어 속도
    [SerializeField] float power;       // 폭탄파워
    [SerializeField] int bombCount;     // 폭탄수 

    [SerializeField] Animator animator;

    private void Start()
    {

    }

    private void Update()
    {
        // 플레이어 소유권자 일경우
        Move();
        // TODO : 폭탄 설치
    }

    public void Move()
    {
        Vector3 moveDir = new Vector3();
        moveDir.x = Input.GetAxisRaw("Horizontal");
        moveDir.z = Input.GetAxisRaw("Vertical");

        if (moveDir.x != 0 || moveDir.z != 0)
        {
            animator.SetBool("Move", true);
        }
        else
        {
            animator.SetBool("Move", false);
        }

        // 움직이지 않았을 시 
        if (moveDir == Vector3.zero)
            return;


        // 동시 입력 시 다른 방향 움직임 제한
        if(moveDir.x !=0)
        {
            moveDir.z = 0;
        }
        else if(moveDir.z != 0) 
        {
            moveDir.x = 0;
        }

        transform.Translate(moveDir.normalized * speed * Time.deltaTime, Space.World);
        transform.forward = moveDir.normalized;
    }
}
