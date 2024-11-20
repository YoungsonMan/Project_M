using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScripts : MonoBehaviour
{
    [SerializeField] private ProceduralDestruction pd;

    private void Start()
    {
    }

    public void CubeDestroy()
    {
        pd.DestroyObject();
    }
}
