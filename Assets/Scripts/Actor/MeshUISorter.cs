using System;
using System.Collections.Generic;

using UnityEngine.UI;
using UnityEngine;

public class MeshUISorter:MonoBehaviour
{
    protected bool InitBuffer = false;

    public int index;
    public MeshUIGroup UIGroup;
    public MeshUI[] sorter;

    protected virtual void Start()
    {
        Register();
    }

    private void Register()
    {
        if (UIGroup && !InitBuffer)
        {
            InitBuffer = true;
            foreach (var ui in sorter)
            {
                if (ui != null)
                {
                    UIGroup.AddMeshUI(ui);
                }
            }
        }
    }

    private void ReleaseMesh()
    {
        if (UIGroup && InitBuffer)
        {
            InitBuffer = false;
            foreach (var ui in sorter)
            {
                if (ui != null)
                {
                    UIGroup.RemoveMeshUI(ui);
                }
            }
        }
    }

    private void Update()
    {
        if (UIGroup)
        {
            foreach (var ui in sorter)
            {
                ui.SetAllDirty();
            }
        }
    }

    protected virtual void OnEnable()
    {
        Register();
    }

    protected virtual void OnDisable()
    {
        ReleaseMesh();
    }

    protected virtual void OnDestroy()
    {
        ReleaseMesh();
    }
}