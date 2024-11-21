using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScripts : MonoBehaviour
{
    [SerializeField] private ProceduralDestruction pd;

    public void CubeDestroy()
    {
        pd.DestroyObject();
    }


}
