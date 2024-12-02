using Photon.Pun;
using System.Collections;
using UnityEngine;

public class Bubble : MonoBehaviourPun
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject bubble;

    private PlayerStatus _status;
    private Animator _animator;
    private WaterBombPlacer _placer;
    private Collider collider;

    private void Awake()
    {
        _status = player.GetComponent<PlayerStatus>();
        _animator = player.GetComponent<Animator>();
        _placer = player.GetComponent<WaterBombPlacer>();
    }

    private void OnEnable()
    {
        _animator.SetBool("isBubble", true);
        _placer.enabled = false;
        // n초 후 물방울 비활성화
        StartCoroutine(BubbleRoutine());
    }

    IEnumerator BubbleRoutine()
    {
        // 5초 뒤에 터지는 것으로 작성
        yield return new WaitForSeconds(5f);
        // 캐릭터 사망
        Dead();
    }

    private void OnTriggerEnter(Collider other)
    {
        // 충돌체 플레이어일 경우
        if (other.gameObject.layer == 3)
        {
            if (other.gameObject.GetComponent<PlayerStatus>().isBubble == true)
            {
                Debug.Log("버블 상태 플레이어 충돌");
                return;
            }

            // 충돌체와 플레이어의 색을 판단
            Color otherColor = other.gameObject.GetComponent<PlayerStatus>().color;
            Color playerColor = player.GetComponent<PlayerStatus>().color;

            // 팀이 방울을 터치할 경우
            if (playerColor == otherColor)
            {
                Debug.Log("같은 팀 충돌 확인");
                Save();
            }
            // 적이 방울을 터치할 경우
            // (임시) 하여튼 플레이어가 와서 터치하면 터짐
            if (playerColor != otherColor)
            {
                Dead();
            }
        }
    }

    [PunRPC]
    public void Save()
    {
        StopAllCoroutines();
        bubble.SetActive(false);
        _animator.SetBool("isBubble", false);
        _status.isBubble = false;
        _placer.enabled = true;
    }

    public void Dead()
    {
        GameManager.Instance.DecreaseTeammate(_status.teamNum);
        bubble.SetActive(false);
        _animator.SetBool("isDead", true);
        Destroy(player, 1f);
    }
}
