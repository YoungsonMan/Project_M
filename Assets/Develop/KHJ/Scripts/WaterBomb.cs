using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBomb : MonoBehaviour
{
    [SerializeField] private float _lifeTime;

    private void OnEnable()
    {
        Destroy(gameObject, _lifeTime);
    }
}
