using UnityEngine;


public class PlayerStatus : MonoBehaviour
{
    [SerializeField] public float speed;       // 플레이어 속도
    [SerializeField] public int power;         // 폭탄파워
    [SerializeField] public int bombCount;     // 폭탄수 

    public bool isBubble;                      // 물방울에 갇힌 상태

    // 플레이어 색상을 위한
    [SerializeField] public Color[] colors;
    // 팀넘버를 설정
    // -> 플레이어가 참조해서 캐릭터 색상을 설정?
    [SerializeField] public int teamNum;


    private void Awake()
    {
    }

    private void Update()
    {
        LimitSpeed();
    }

    public void LimitSpeed()
    {
        if(speed > 10)
        {
            speed = 10;
        }
    }

}
