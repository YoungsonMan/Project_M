using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    [SerializeField] public float speed;       // ÇÃ·¹ÀÌ¾î ¼Óµµ
    [SerializeField] public int power;         // ÆøÅºÆÄ¿ö
    [SerializeField] public int bombCount;     // ÆøÅº¼ö 

    public bool isBubble;                      // ¹°¹æ¿ï¿¡ °¤Èù »óÅÂ
}
