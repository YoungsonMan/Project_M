using UnityEngine;

public enum Team { Red, Green, Blue, Yello, Cyan, White, Black, Gray }

public class PlayerStatus : MonoBehaviour
{
    [SerializeField] public float speed;       // ÇÃ·¹ÀÌ¾î ¼Óµµ
    [SerializeField] public int power;         // ÆøÅºÆÄ¿ö
    [SerializeField] public int bombCount;     // ÆøÅº¼ö 

    public bool isBubble;                      // ¹°¹æ¿ï¿¡ °¤Èù »óÅÂ

    // ÇÃ·¹ÀÌ¾î »ö»óÀ» À§ÇÑ

    [SerializeField] public Team curTeam;
    [SerializeField] public Color[] colors;


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
