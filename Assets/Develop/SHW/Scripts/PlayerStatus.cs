using UnityEngine;

public enum Team { Red, Green, Blue, Yello, Cyan, White, Black, Gray }

public class PlayerStatus : MonoBehaviour
{
    [SerializeField] public float speed;       // 플레이어 속도
    [SerializeField] public int power;         // 폭탄파워
    [SerializeField] public int bombCount;     // 폭탄수 

    public bool isBubble;                      // 물방울에 갇힌 상태

    // 플레이어 색상을 위한

    [SerializeField] public Team curTeam;
    [SerializeField] public Color[] colors;


    private void Awake()
    {
    }

    //public void SetTeamColor()
    //{
    //    switch (curTeam)
    //    {
    //        case Team.Green:
    //            colors[0] = Color.green;
    //            break;
    //        case Team.Red:
    //            colors[1] = Color.red;
    //            break;
    //        case Team.Blue:
    //            colors[2] = Color.blue;
    //            break;
    //        case Team.Yello:
    //            colors[3] = Color.yellow;
    //            break;
    //        case Team.White:
    //            colors[4] = Color.white;
    //            break;
    //        case Team.Black:
    //            colors[5] = Color.black;
    //            break;
    //        case Team.Gray:
    //            colors[6] = Color.grey;
    //            break;
    //        case Team.Cyan:
    //            colors[7] = Color.cyan;
    //            break;
    //    }
    //}
}
