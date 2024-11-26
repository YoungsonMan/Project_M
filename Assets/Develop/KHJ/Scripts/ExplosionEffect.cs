using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    [SerializeField] private float _lifeTime;

    private void OnEnable()
    {
        Deactivate();
    }

    private void OnDisable()
    {
        if (_deactiveCoroutine != null)
        {
            StopCoroutine(_deactiveCoroutine);
            _deactiveCoroutine = null;
        }
    }

    private void Deactivate() => StartCoroutine(DeactivateRoutine());

    private Coroutine _deactiveCoroutine;
    IEnumerator DeactivateRoutine()
    {
        yield return new WaitForSeconds(_lifeTime);

        gameObject.SetActive(false);
    }

}
