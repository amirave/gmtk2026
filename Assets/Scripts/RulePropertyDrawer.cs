using UnityEditor;
using UnityEngine;

namespace Game
{
    [CustomPropertyDrawer(typeof(Rule))]
    public class RulePropertyDrawer : PropertyDrawer
    {
        private static readonly string[] FieldNames = { "property", "mode", "secondProperty" };
        private static readonly string[] FieldLabels = { "Property", "Mode", "Second Property" };

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            float lineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;

            var rule = property.managedReferenceValue as Rule;

            if (rule == null)
            {
                Rect buttonRect = new(position.x, position.y, position.width, lineHeight);
                if (GUI.Button(buttonRect, new GUIContent("Create Rule")))
                {
                    property.managedReferenceValue = new Rule();
                    property.serializedObject.ApplyModifiedProperties();
                }
            }
            else
            {
                Rect headerRect = new(position.x, position.y, position.width - lineHeight - spacing, lineHeight);
                Rect removeRect = new(position.x + position.width - lineHeight, position.y, lineHeight, lineHeight);

                property.isExpanded = EditorGUI.Foldout(headerRect, property.isExpanded, label, true);

                if (GUI.Button(removeRect, "\u00D7"))
                {
                    property.managedReferenceValue = null;
                    property.serializedObject.ApplyModifiedProperties();
                    EditorGUI.EndProperty();
                    return;
                }

                if (property.isExpanded)
                {
                    EditorGUI.indentLevel++;
                    float y = position.y + lineHeight + spacing;

                    for (int i = 0; i < FieldNames.Length; i++)
                    {
                        SerializedProperty field = property.FindPropertyRelative(FieldNames[i]);
                        if (field == null) continue;

                        float fieldHeight = EditorGUI.GetPropertyHeight(field, true);
                        Rect fieldRect = new(position.x, y, position.width, fieldHeight);
                        EditorGUI.PropertyField(fieldRect, field, new GUIContent(FieldLabels[i]), true);
                        y += fieldHeight + spacing;
                    }

                    EditorGUI.indentLevel--;
                }
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float lineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;

            var rule = property.managedReferenceValue as Rule;

            if (rule == null) return lineHeight;
            if (!property.isExpanded) return lineHeight;

            float height = lineHeight + spacing;

            foreach (string fieldName in FieldNames)
            {
                SerializedProperty field = property.FindPropertyRelative(fieldName);
                float fieldHeight = field != null ? EditorGUI.GetPropertyHeight(field, true) : lineHeight;
                height += fieldHeight + spacing;
            }

            return height;
        }
    }
}
