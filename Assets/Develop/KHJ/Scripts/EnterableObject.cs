using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterableObject : MonoBehaviourPun
{
    [SerializeField] List<GameObject> _innerObjs;

    private int _waterbombLayer;

    private void Awake()
    {
        _innerObjs = new List<GameObject>();
        _waterbombLayer = LayerMask.NameToLayer("WaterBomb");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Something Entered");
        RendererCache rendererCache = GetRenderCache(other.transform);
        if (rendererCache == null)
            return;

        _innerObjs.Add(rendererCache.gameObject);

        List<Renderer> renderers = rendererCache.Cache;

        if (renderers != null)
            Conceal(renderers);
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Something Exited");
        RendererCache rendererCache = GetRenderCache(other.transform);
        if (rendererCache == null)
            return;

        _innerObjs.Remove(rendererCache.gameObject);

        List<Renderer> renderers = rendererCache.Cache;

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
            if (renderers != null && obj.layer != _waterbombLayer)
                Reveal(renderers);

            IExplosionInteractable interactable = obj.GetComponent<IExplosionInteractable>();
            interactable?.Interact();
        }
    }

    private RendererCache GetRenderCache(Transform target)
    {
        RendererCache rendererCache = null;
        Transform curTransform = target;

        while (curTransform != null)
        {
            rendererCache = curTransform.GetComponent<RendererCache>();
            Debug.Log($"Finding RendererCache.. {curTransform.name}: {rendererCache}");
            if (rendererCache != null)
                return rendererCache;

            curTransform = curTransform.parent;
        }

        return null;
    }

    private void Conceal(List<Renderer> renderers)
    {
        foreach (Renderer renderer in renderers)
        {
            if (renderer != null)
                renderer.enabled = false;
        }
    }

    private void Reveal(List<Renderer> renderers)
    {
        foreach (Renderer renderer in renderers)
        {
            if (renderer != null)
                renderer.enabled = true;
        }
    }
}
