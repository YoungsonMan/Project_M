using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    private void OnEnable()
    {
        Destroy(gameObject, 0.2f);
    }
}
