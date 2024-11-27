using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;

public class PlayerController : MonoBehaviourPun, IExplosionInteractable
{
    private PlayerStatus _status;           // 플레이어 스탯 가져옴

    [SerializeField] Rigidbody rigid;       // 이동을 위한 rigidbody

    [SerializeField] Animator animator;     // 플레이어 애니메이션 실행

    [SerializeField] GameObject bubble;     // 물줄기에 맏고 갇힐 물방울

    [SerializeField] int playerNumber;

    // 물방울 안에 갇혔을 때 속도 느리게 설정
    private float bubbleSpeed = 0.5f;

    // 색상 변경용 
    [SerializeField] public Color color;
    [SerializeField] Renderer bodyRenderer;
    public int testNum = 0;

    private void Awake()
    {
        playerNumber = PhotonNetwork.LocalPlayer.ActorNumber - 1;
        _status = GetComponent<PlayerStatus>();
        _status.isBubble = false;
        bubble.SetActive(false);

        // SetColor();
        photonView.RPC("SetColor", RpcTarget.All);
    }

    private void Update()
    {
        if (photonView.IsMine == false)
            return;

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
        // test) 팀에 따른 색 변경
        // 임시) 짝수팀 홀수 팀
        int num = photonView.Owner.GetPlayerNumber();
        int num2 = num % 2;

        for (int i = 0; i < bodyRenderer.materials.Length; i++)
        {
            bodyRenderer.materials[i].color = _status.colors[num2];
            //bodyRenderer.materials[i].color = color;
        }

    }

    // (테스트) 숫자를 입력할 경우 팀과 색상 설정
    public void SetTeamColor(int num)
    {
        switch (num)
        {
            case 0:
                color = _status.colors[0];
                break;
            case 1:
                color = _status.colors[1];
                break;
            case 2:
                color = _status.colors[2];
                break;
            case 3:
                color = _status.colors[3];
                break;
            case 4:
                color = _status.colors[4];
                break;
            case 5:
                color = _status.colors[5];
                break;
            case 6:
                color = _status.colors[6];
                break;
            case 7:
                color = _status.colors[7];
                break;
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
}
