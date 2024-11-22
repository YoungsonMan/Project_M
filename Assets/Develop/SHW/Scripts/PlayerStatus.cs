using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    [SerializeField] public float speed;       // ÇÃ·¹ÀÌ¾î ¼Óµµ
    [SerializeField] public float power;       // ÆøÅºÆÄ¿ö
    [SerializeField] public int bombCount;     // ÆøÅº¼ö 

    public bool isBubble;

    private float preSpeed;

    private void Awake()
    {
       //preSpeed = speed;
    }

    private void Update()
    {
        Debug.Log($"{speed}");
        //if (isBubble == false)
        //{
        //    speed = preSpeed;
        //}
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if(collision.gameObject.name == "test")
        //{
        //    preSpeed = speed;
        //    speed = 0.5f;
        //}
    }
}
