using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.U2D;

public class ActorHUD : MonoBehaviour
{

    public void Start()
    {
        ActorUIMesh mesh = ActorUIMeshProvider.Instance.Get();
        Mesh _mesh = new Mesh();
        MeshFilter filter = this.gameObject.AddComponent<MeshFilter>();
        filter.mesh = _mesh;

        MeshRenderer renderer = this.gameObject.AddComponent<MeshRenderer>();
        renderer.sortingOrder = 100;
        renderer.receiveShadows = false;
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
        renderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
        
        HUDImage[] images =  this.transform.GetComponentsInChildren<HUDImage>();
        for (int i = 0; i < images.Length; i++)
        {
            HUDImage img = images[i];
            img.SetActorHUDMesh(i,mesh);
            img.OnFillMesh(mesh);
            img.OnRenderer(renderer);
        }
        mesh.FillMesh(_mesh);
    }

    public void OnEnable()
    { 
    }

    public void OnDisable()
    {
        
    }
}
