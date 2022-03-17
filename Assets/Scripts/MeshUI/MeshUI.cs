﻿using System;
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
    public class MeshUI : UnityEngine.EventSystems.UIBehaviour
    {
        protected override void Start()
        {
            base.Start();
        }

        protected virtual void ReapplyDrivenProperties(RectTransform t)
        {
            Debug.Log("ReapplyDrivenProperties");
        }

        protected override void OnRectTransformDimensionsChange()
        {
            if (gameObject.activeInHierarchy)
            {
                SetVerticesDirty();
            }
        }

        protected override void OnBeforeTransformParentChanged()
        {
            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
            if (gameObject.activeInHierarchy)
            {
                SetVerticesDirty();
            }
        }

        protected override void OnTransformParentChanged()
        {
            base.OnTransformParentChanged();

            if (!IsActive())
                return;
        }

        public virtual void SetAllDirty()
        {
            SetVerticesDirty();
        }

        public MeshUIGroup UIGroup
        {
            get 
            {
                if (m_Group == null)
                {
                    CacheGroup();
                }
                return m_Group;
            }
        }

        public int MeshIndex = 0;
        public int SiblingIndex = 0;

        private MeshUIGroup m_Group;
        private void CacheGroup()
        {
            var list = ListPool<MeshUIGroup>.Get();
            gameObject.GetComponentsInParent(false, list);
            if (list.Count > 0)
            {
                // Find the first active and enabled MeshUIGroup.
                for (int i = 0; i < list.Count; ++i)
                {
                    if (list[i].isActiveAndEnabled)
                    {
                        m_Group = list[i];
                        break;
                    }
                }
            }
            else
                m_Group = null;
            ListPool<MeshUIGroup>.Release(list);
        }


#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            SetAllDirty();
        }
#endif

        protected bool InitBuffer = false;
        /// <summary>
        /// Mark the Graphic and the canvas as having been changed.
        /// </summary>
        protected override void OnEnable()
        {
            m_VertsDirty = true;
            SiblingIndex = rectTransform.GetSiblingIndex();
        }

        /// <summary>
        /// Clear references.
        /// </summary>
        protected override void OnDisable()
        {
            m_VertsDirty = true;
            if (meshBuffer != null && qmesh != null)
            {
                meshBuffer.CollapseQuad(qmesh.buffIndex, qmesh.indicesIndex);
            }
        }

        [NonSerialized] private RectTransform m_RectTransform;

        [NonSerialized] private bool m_VertsDirty;


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
        }

        public QuadMesh Quad { get { return qmesh; } }
        protected QuadMesh qmesh;
        protected UIMeshBuffer meshBuffer;
        public void SetUIMeshBuffer(UIMeshBuffer meshBuffer, QuadMesh qmesh)
        {
            this.meshBuffer = meshBuffer;
            this.qmesh = qmesh;
            MeshIndex = qmesh.id;
            SetAllDirty();
        }
       
        /// <summary>
        /// Update the renderer's vertices.
        /// </summary>
        protected virtual void UpdateGeometry()
        {
            DoLegacyMeshGeneration();
        }


        private void DoLegacyMeshGeneration()
        {
            OnPopulateMesh();
        }

        /// <summary>
        /// Fill the vertex buffer data.
        /// </summary>
        protected virtual void OnPopulateMesh()
        {
           
        }

        public Rect GetPixelAdjustedRect()
        {
            return rectTransform.rect; 
        }

        public virtual bool Rebuild(CanvasUpdate update)
        {
            if (!IsActive())
            {
                return m_VertsDirty;
            }
            bool dirty = false;
            switch (update)
            {
                case CanvasUpdate.PreRender:
                    if (m_VertsDirty)
                    {
                        dirty = true;
                        UpdateGeometry();
                        m_VertsDirty = false;
                    }
                    break;
            }
            return dirty;
        }        
    }
}

