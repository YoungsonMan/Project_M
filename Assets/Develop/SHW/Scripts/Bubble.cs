using Photon.Pun;
using System.Collections;
using UnityEngine;

public class Bubble : MonoBehaviourPun
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject bubble;     

    private PlayerStatus _status;           // 플레이어 스탯
    private Animator _animator;             // 애니메이션 출력
    private WaterBombPlacer _placer;        // 물풍선 설치

    private void Awake()
    {
        _status = player.GetComponent<PlayerStatus>();
        _animator = player.GetComponent<Animator>();
        _placer = player.GetComponent<WaterBombPlacer>();
    }

    private void OnEnable()
    {
        SoundManager.Instance.PlaySFX(SoundManager.E_SFX.BOMB_LOCKED);

        _animator.SetBool("isBubble", true);
        _placer.enabled = false;
        // n초 후 자동 사망
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
        SoundManager.Instance.PlaySFX(SoundManager.E_SFX.BOMB_DEAD);
        GameManager.Instance.DecreaseTeammate(_status.teamNum);
        bubble.SetActive(false);
        _animator.SetBool("isDead", true);
        Destroy(player, 1f);
    }
}
