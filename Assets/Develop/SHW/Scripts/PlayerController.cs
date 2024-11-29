using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviourPun, IExplosionInteractable
{
    private PlayerStatus _status;           // 플레이어 스탯 가져옴

    [SerializeField] Rigidbody rigid;       // 이동을 위한 rigidbody

    [SerializeField] Animator animator;     // 플레이어 애니메이션 실행

    [SerializeField] GameObject bubble;     // 물줄기에 맏고 갇힐 물방울

    [SerializeField] GameObject arrow;      // 플레이어를 가르킬 화살표

    // 물방울 안에 갇혔을 때 속도 느리게 설정
    private float bubbleSpeed = 0.5f;



    // 색상 변경용 
    [SerializeField] public Color color;
    [SerializeField] Renderer bodyRenderer;

    private void Awake()
    {
        
        _status = GetComponent<PlayerStatus>();
        _status.isBubble = false;
        bubble.SetActive(false);
    }
    private void Start()
    {
        // 색상 설정
        photonView.RPC("SetColor", RpcTarget.All);
        GameManager.Instance.IncreaseTeammate(_status.teamNum);
    }

    private void Update()
    {
        if (photonView.IsMine == false)
        {
            arrow.SetActive(false);
            return;
        }

        Move();


    }

    public void Move()
    {
        Vector3 moveDir = new Vector3();
        moveDir.x = Input.GetAxisRaw("Horizontal");
        moveDir.z = Input.GetAxisRaw("Vertical");

        moveDirection = new Vector3(moveDir.x, 0, moveDir.z).normalized;

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
        if (_status.isBubble == true)
        {
            rigid.velocity = moveDir.normalized * bubbleSpeed;
        }
        else
        {
            rigid.velocity = moveDir.normalized * _status.speed;
        }

        // 입력이 없어도 방향을 유지
        if (moveDir.magnitude > 0.1)
        {
            transform.forward = moveDir;
        }
    }

    [PunRPC]
    public void SetColor()
    {
        for (int i = 0; i < bodyRenderer.materials.Length; i++)
        {
            // 플레이어 teamNum 세팅이 끝난다면 num2 자리에 _status.teamNum을 넣으면 됩니다.
            // Debug.Log($"하운) TeamNum {_status.teamNum}");
            bodyRenderer.materials[i].color = _status.colors[_status.teamNum];
            // 팀 인식을 위해 캐릭터 색상 설정
            color = _status.colors[_status.teamNum];
        }

    }
    public bool Interact()
    {
        photonView.RPC("BubbledRPC", RpcTarget.All);

        return true;
    }

    [PunRPC]
    public void BubbledRPC()
    {
        _status.isBubble = true;
        bubble.SetActive(true);
    }

    private Vector3 moveDirection;
    private void OnCollisionStay(Collision collision)
    {
        if (!photonView.IsMine) return;

        // 밀기 가능한 오브젝트와 충돌 시
        PushableObject pushable = collision.gameObject.GetComponent<PushableObject>();
        if (pushable != null)
        {
            // 접촉 + 플레이어 방향 =  게이지
            // 미접촉 = 노게이지
            // 접촉 + 플레이어 방향 x = 노게이지

            // 충돌 지점과 충돌 방향 계산
            Vector3 collisionDirection = (collision.transform.position - transform.position).normalized;
            
            // 이동 방향과 충돌 방향의 내적적용.
            float dotProduct = Vector3.Dot(moveDirection, collisionDirection);

            if (dotProduct > 0.5f) // 방향 유사도가 높을 경우 (1에 가까울수록 방향이 일치)
            {
                // 오브젝트 밀기
                pushable.Push(photonView, moveDirection);
            }
            else
            {
                Debug.Log($"방향이 일치하지 않아 오브젝트를 밀 수 없습니다.  {dotProduct}");
            }
        }
    }
}

