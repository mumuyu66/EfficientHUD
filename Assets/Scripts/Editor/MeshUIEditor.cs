using System.Linq;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.UI
{
    /// <summary>
    /// Editor class used to edit UI Graphics.
    /// </summary>

    [CustomEditor(typeof(MeshUI), false)]
    [CanEditMultipleObjects]
    public class MeshUIEditor : Editor
    {
        protected SerializedProperty m_Script;
        protected SerializedProperty m_Color;
        protected AnimBool m_ShowNativeSize;

        protected virtual void OnDisable()
        {
            Tools.hidden = false;
            m_ShowNativeSize.valueChanged.RemoveListener(Repaint);
        }

        protected virtual void OnEnable()
        {
            m_Script = serializedObject.FindProperty("m_Script");
            m_Color = serializedObject.FindProperty("m_Color");

            m_ShowNativeSize = new AnimBool(false);
            m_ShowNativeSize.valueChanged.AddListener(Repaint);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_Script);
            AppearanceControlsGUI();
            serializedObject.ApplyModifiedProperties();
        }

        protected void SetShowNativeSize(bool show, bool instant)
        {
            if (instant)
                m_ShowNativeSize.value = show;
            else
                m_ShowNativeSize.target = show;
        }

        protected void AppearanceControlsGUI()
        {
            EditorGUILayout.PropertyField(m_Color);
            MeshUI ui = target as MeshUI;
            if (ui == null) return;
            ui.SetAllDirty();
            ui.Rebuild(CanvasUpdate.PreRender);
        }
    }
}
