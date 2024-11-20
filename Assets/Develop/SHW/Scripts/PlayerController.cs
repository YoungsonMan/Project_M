using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float speed;       // 플레이어 속도
    [SerializeField] float power;       // 폭탄파워
    [SerializeField] int bombCount;     // 폭탄수 

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

        // 움직이지 않았을 시 
        if (moveDir == Vector3.zero)
            return;

        // 대각 입력 시 
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow))
        {
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow))
            {
                // TODO : 동시 입력시 현재 이동을 멈춤. 추후 이동하던 방향으로 계속 이동하는 쪽으로 
                return;
            }
        }

        transform.Translate(moveDir.normalized * speed * Time.deltaTime, Space.World);
        transform.forward = moveDir.normalized;
    }
}
