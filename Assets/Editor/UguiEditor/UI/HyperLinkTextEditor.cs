using UnityEngine;
using UnityEngine.UI;
namespace UnityEditor.UI
{
    // TODO REVIEW
    // Have material live under text
    // move stencil mask into effects *make an efects top level element like there is
    // paragraph and character

    /// <summary>
    /// Editor class used to edit UI Labels.
    /// </summary>

    [CanEditMultipleObjects]
    public class HyperLinkTextEditor : GraphicEditor
    {
        SerializedProperty m_Text;
        SerializedProperty m_FontData;
        SerializedProperty m_hyperlinkColor;
        SerializedProperty m_parseEmoji;
        protected override void OnEnable()
        {
            base.OnEnable();
            m_Text = serializedObject.FindProperty("m_Text");
            m_FontData = serializedObject.FindProperty("m_FontData");
            m_hyperlinkColor= serializedObject.FindProperty("hyperlinkColor");
            m_parseEmoji = serializedObject.FindProperty("parseEmoji");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_Text);
            EditorGUILayout.PropertyField(m_FontData);
            EditorGUILayout.PropertyField(m_hyperlinkColor);
            EditorGUILayout.PropertyField(m_parseEmoji);

            EditorGUILayout.PropertyField(m_Color);
            EditorGUILayout.PropertyField(m_Material);
            EditorGUILayout.PropertyField(m_RaycastTarget);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
