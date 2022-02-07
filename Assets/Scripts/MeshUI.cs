using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEngine.UI
{
    internal static class SetPropertyUtility
    {
        public static bool SetColor(ref Color currentValue, Color newValue)
        {
            if (currentValue.r == newValue.r && currentValue.g == newValue.g && currentValue.b == newValue.b && currentValue.a == newValue.a)
                return false;

            currentValue = newValue;
            return true;
        }

        public static bool SetStruct<T>(ref T currentValue, T newValue) where T : struct
        {
            if (EqualityComparer<T>.Default.Equals(currentValue, newValue))
                return false;

            currentValue = newValue;
            return true;
        }

        public static bool SetClass<T>(ref T currentValue, T newValue) where T : class
        {
            if ((currentValue == null && newValue == null) || (currentValue != null && currentValue.Equals(newValue)))
                return false;

            currentValue = newValue;
            return true;
        }
    }

    [RequireComponent(typeof(RectTransform))]
    [ExecuteInEditMode]
    public class MeshUI : UnityEngine.EventSystems.UIBehaviour, ICanvasElement
    {
        public virtual void Rebuild(CanvasUpdate update)
        {
            if (canvasRenderer.cull)
                return;

            switch (update)
            {
                case CanvasUpdate.PreRender:
                    if (m_VertsDirty)
                    {
                        UpdateGeometry();
                        m_VertsDirty = false;
                    }
                    break;
            }
        }

        public virtual void GraphicUpdateComplete()
        {
        }

        public virtual void LayoutComplete()
        {
        }


        [NonSerialized] private RectTransform m_RectTransform;
        [NonSerialized] private CanvasRenderer m_CanvasRender;
        [NonSerialized] private Canvas m_Canvas;

        [NonSerialized] private bool m_VertsDirty;
        [NonSerialized] private bool m_MaterialDirty;


        [SerializeField] private Color m_Color = Color.white;
        public virtual Color color { get { return m_Color; } set { if (SetPropertyUtility.SetColor(ref m_Color, value)) SetVerticesDirty(); } }

        /// <summary>
        /// Transform gets cached for speed.
        /// </summary>
        public RectTransform rectTransform
        {
            get { return m_RectTransform ?? (m_RectTransform = GetComponent<RectTransform>()); }
        }

        public virtual void SetVerticesDirty()
        {
            if (!IsActive())
                return;

            m_VertsDirty = true;
            CanvasUpdateRegistry.RegisterCanvasElementForGraphicRebuild(this);
        }

        /// <summary>
        /// UI Renderer component.
        /// </summary>
        public CanvasRenderer canvasRenderer
        {
            get
            {
                return m_CanvasRender;
            }
            set
            {
                m_CanvasRender = value;
            }
        }

        public Canvas canvas
        {
            get
            {
                return m_Canvas;
            }
            set
            {
                m_Canvas = value;
            }
        }

        private UIMeshBuffer meshBuffer;
        private int indicesIndex, vindex;
        public void SetUIMeshBuffer(UIMeshBuffer meshBuffer,int indicesIndex,int vindex)
        {
            this.meshBuffer = meshBuffer;
            this.indicesIndex = indicesIndex;
            this.vindex = vindex;
        }
       
        /// <summary>
        /// Update the renderer's vertices.
        /// </summary>
        protected virtual void UpdateGeometry()
        {
            DoLegacyMeshGeneration();
        }

        /// <summary>
        /// Update the renderer's material.
        /// </summary>
        protected virtual void UpdateMaterial()
        {
            if (!IsActive())
                return;

            canvasRenderer.materialCount = 1;
            // canvasRenderer.SetMaterial(materialForRendering, 0);
            // canvasRenderer.SetTexture(mainTexture);
        }

        private void DoLegacyMeshGeneration()
        {
          //  OnPopulateMesh();
        }

        /// <summary>
        /// Fill the vertex buffer data.
        /// </summary>
        protected virtual void OnPopulateMesh(UIMeshBuffer mesh)
        {
            var r = GetPixelAdjustedRect();
            var v = new Vector4(r.x, r.y, r.x + r.width, r.y + r.height);
            mesh.UpdataColor(indicesIndex,color);
            mesh.UpdataUV(indicesIndex, new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0));
            mesh.UpdataVertices(indicesIndex, new Vector3(v.x, v.y), new Vector3(v.x, v.w), new Vector3(v.z, v.w), new Vector3(v.z, v.y));
            mesh.FillQuad(indicesIndex, vindex);
        }

        protected virtual void BuildMash(UIMeshBuffer mesh)
        {
            var r = GetPixelAdjustedRect();
            var v = new Vector4(r.x, r.y, r.x + r.width, r.y + r.height);

            vindex = mesh.AddQuad(new Vector3(v.x, v.y), new Vector3(v.x, v.w), new Vector3(v.z, v.w), new Vector3(v.z, v.y),
                new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0),
                color,
                out indicesIndex
                );
        }

        public Rect GetPixelAdjustedRect()
        {
            if (!canvas || canvas.renderMode == RenderMode.WorldSpace || canvas.scaleFactor == 0.0f || !canvas.pixelPerfect)
                return rectTransform.rect;
            else
                return RectTransformUtility.PixelAdjustRect(rectTransform, canvas);
        }
    }
}

