using UnityEditor;
using UnityEngine;

namespace Game
{
    [CustomPropertyDrawer(typeof(Rule))]
    public class RulePropertyDrawer : PropertyDrawer
    {
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

                    SerializedProperty propertyField = property.FindPropertyRelative("property");
                    if (propertyField != null)
                    {
                        float childHeight = EditorGUI.GetPropertyHeight(propertyField, true);
                        Rect childRect = new(position.x, y, position.width, childHeight);
                        EditorGUI.PropertyField(childRect, propertyField, new GUIContent("Property"), true);
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

            if (rule == null)
                return lineHeight;

            if (!property.isExpanded)
                return lineHeight;

            SerializedProperty propertyField = property.FindPropertyRelative("property");
            float childHeight = propertyField != null ? EditorGUI.GetPropertyHeight(propertyField, true) : lineHeight;

            return lineHeight + spacing + childHeight + spacing;
        }
    }
}
