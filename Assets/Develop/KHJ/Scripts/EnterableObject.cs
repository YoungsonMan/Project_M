using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterableObject : MonoBehaviourPun
{
    [SerializeField] List<GameObject> _innerObjs;

    private void Awake()
    {
        _innerObjs = new List<GameObject>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Transform root = GetRoot(other.transform);
        List<Renderer> renderers = root.GetComponent<RendererCache>().Cache;

        _innerObjs.Add(root.gameObject);

        if (renderers != null)
            Conceal(renderers);
    }

    private void OnTriggerExit(Collider other)
    {
        Transform root = GetRoot(other.transform);
        List<Renderer> renderers = root.GetComponent<RendererCache>().Cache;

        _innerObjs.Remove(root.gameObject);

        if (renderers != null)
            Reveal(renderers);
    }

    private void OnDestroy()
    {
        if (_innerObjs.Count == 0)
            return;

        foreach (GameObject obj in _innerObjs)
        {
            List<Renderer> renderers = obj.GetComponent<RendererCache>().Cache;
            if (renderers != null)
                Reveal(renderers);

            IExplosionInteractable interactable = obj.GetComponent<IExplosionInteractable>();
            interactable?.Interact();
        }
    }

    private Transform GetRoot(Transform target)
    {
        Transform curTransform = target;
        while (curTransform.parent != null)
        {
            curTransform = curTransform.parent;
        }

        return curTransform;
    }

    private void Conceal(List<Renderer> renderers)
    {
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = false;
        }
    }

    private void Reveal(List<Renderer> renderers)
    {
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = true;
        }
    }
}
