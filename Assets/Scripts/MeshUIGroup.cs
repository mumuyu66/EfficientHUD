using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(CanvasRenderer))]
[RequireComponent(typeof(RectTransform))]
[ExecuteInEditMode]
class MeshUIGroup : UnityEngine.EventSystems.UIBehaviour
{
    protected List<MeshUI> components = new List<MeshUI>();
    protected override void Start()
    {
    }

    [NonSerialized] private Canvas m_Canvas;
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

    public void AddUI(MeshUI ui)
    { 
    }

    public void RemoveUI(MeshUI ui)
    {
        
    }
}
