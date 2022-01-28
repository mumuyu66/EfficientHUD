using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ActorHUD : MonoBehaviour
{
    public ActorHUD(int uuid, UIMeshDataBuffer meshSharedBuffer, Material mat)
    {

    }

    protected UIMeshDataBuffer mMeshSharedBuffer;
    protected int uuid;
    public void SetActorData(int uuid,UIMeshDataBuffer meshData)
    {
        this.uuid = uuid;
        mMeshSharedBuffer = meshData;
    }

    public void Start()
    {
        HUDImage[] images =  this.transform.GetComponents<HUDImage>();

    }

    public void OnEnable()
    { 
    }

    public void OnDisable()
    {
        
    }
}
