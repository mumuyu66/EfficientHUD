using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.UI
{
    /// <summary>
    /// Editor class used to edit UI Images.
    /// </summary>
    [CustomEditor(typeof(MeshImage), true)]
    [CanEditMultipleObjects]
    public class MeshImageEditor : MeshUIEditor
    {
        SerializedProperty m_activeSprite;
        GUIContent m_SpriteContent;
        protected override void OnEnable()
        {
            base.OnEnable();
            m_activeSprite = serializedObject.FindProperty("activeSprite");
            m_SpriteContent = new GUIContent("Source Image");

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_activeSprite, m_SpriteContent);
            AppearanceControlsGUI();
            serializedObject.ApplyModifiedProperties();
        }

        protected void SpriteGUI()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_activeSprite, m_SpriteContent);
            if (EditorGUI.EndChangeCheck())
            {
                var newSprite = m_activeSprite.objectReferenceValue as Sprite;
                if (newSprite)
                {
                    MeshImage ui = target as MeshImage;
                    ui.SetAllDirty();
                    ui.Rebuild(CanvasUpdate.PreRender);
                }
            }
        }

        /// <summary>
        /// Draw the Image preview.
        /// </summary>
        public override void OnPreviewGUI(Rect rect, GUIStyle background)
        {
            MeshImage image = target as MeshImage;
            if (image == null) return;

            Sprite sf = image.activeSprite;
            if (sf == null) return;

            SpriteDrawUtility.DrawSprite(sf, rect, image.color);
        }

        /// <summary>
        /// Info String drawn at the bottom of the Preview
        /// </summary>

        public override string GetInfoString()
        {
            RawImage rawImage = target as RawImage;

            // Image size Text
            string text = string.Format("RawImage Size: {0}x{1}",
                    Mathf.RoundToInt(Mathf.Abs(rawImage.rectTransform.rect.width)),
                    Mathf.RoundToInt(Mathf.Abs(rawImage.rectTransform.rect.height)));

            return text;
        }
    }
}
