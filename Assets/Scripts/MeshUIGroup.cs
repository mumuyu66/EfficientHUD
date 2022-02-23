using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using UnityEngine.UI.Collections;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
[ExecuteInEditMode]
public class MeshUIGroup : UnityEngine.EventSystems.UIBehaviour
{
    private IndexedSet<MeshUI> components = new IndexedSet<MeshUI>();
    public int quad_num = 400;
    public Sprite activeSprite;

    private MeshFilter meshFilter;
    private Mesh mesh;

    protected override void Awake()
    {
        base.Awake();
        buffer = new UIMeshBuffer();
        sharedMesh = new SharedQuadMesh(quad_num, buffer);

        Material mat = new Material(Shader.Find("UI/Default"));
        mat.SetTexture("_MainTex", activeSprite.texture);

        mesh = new Mesh();
        meshFilter = gameObject.GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
        }
        meshFilter.mesh = mesh;

        MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
        if (renderer == null)
        {
            renderer = gameObject.AddComponent<MeshRenderer>();
        }
        renderer.material = mat;
        renderer.sortingOrder = 1;
        renderer.receiveShadows = false;
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
        renderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
        
    }

    public bool Dirty = true;
    protected override void Start()
    {
        base.Start();
        Canvas.willRenderCanvases += PerformUpdate;
    }

    protected override void OnDestroy()
    {
        Canvas.willRenderCanvases -= PerformUpdate;
        base.OnDestroy();
    }

    private void PerformUpdate()
    {
        if (Dirty)
        {
            MeshUI ui;
            for (int i = 0; i < components.Count; i++)
            {
                ui = components[i];
                if (ui.IsActive())
                {
                    ui.Rebuild(CanvasUpdate.PreRender);
                }
            }
            buffer.FillMesh(mesh);
            Dirty = false;
        }
    }

    protected void ReapplyDrivenProperties(RectTransform driven)
    {
        Debug.Log(string.Format("ReapplyDrivenProperties name = {0}",driven.name));
    }

    private UIMeshBuffer buffer;
    private SharedQuadMesh sharedMesh;
    [NonSerialized] private Canvas m_Canvas;
    [NonSerialized] private CanvasRenderer canvasRenderer;
    public Canvas canvas
    {
        get
        {
            if (m_Canvas == null)
                CacheCanvas();
            return m_Canvas;
        }
    }

    private void CacheCanvas()
    {
        var list = ListPool<Canvas>.Get();
        gameObject.GetComponentsInParent(false, list);
        if (list.Count > 0)
        {
            // Find the first active and enabled canvas.
            for (int i = 0; i < list.Count; ++i)
            {
                if (list[i].isActiveAndEnabled)
                {
                    m_Canvas = list[i];
                    break;
                }
            }
        }
        else
            m_Canvas = null;
        ListPool<Canvas>.Release(list);
    }

    public void AddMeshUI(MeshUI ui)
    {
        if (sharedMesh == null)
        {
            return;
        }
        if (components.AddUnique(ui))
        {
            ui.SetUIMeshBuffer(buffer, sharedMesh.Get());
        }
    }

    public void RemoveMeshUI(MeshUI ui,QuadMesh m)
    {
        if (m != null)
        {
            components.Remove(ui);
            buffer.CollapseQuad(m.buffIndex,m.indicesIndex);
            mesh.Clear();
            buffer.FillMesh(mesh);
        }
            
    }
}
