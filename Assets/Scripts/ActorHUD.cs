using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ActorHUD : MonoBehaviour
{
    public ActorHUD(int uuid, UIMeshBuffer meshSharedBuffer, Material mat)
    {

    }

    public void Start()
    {
        ActorUIMesh mesh = ActorUIMeshProvider.Instance.Get();
        HUDImage[] images =  this.transform.GetComponents<HUDImage>();
        for (int i = 0; i < images.Length; i++)
        {
            images[i].SetActorHUDMesh(i,mesh);
        }
    }

    public void OnEnable()
    { 
    }

    public void OnDisable()
    {
        
    }
}
