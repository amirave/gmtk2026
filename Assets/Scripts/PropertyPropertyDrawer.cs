using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Game
{
    [CustomPropertyDrawer(typeof(Property), true)]
    public class PropertyPropertyDrawer : PropertyDrawer
    {
        private static readonly Dictionary<string, Type> AllowedTypes = new()
        {
            { "Color Property", typeof(ColorProperty) },
            { "Shape Property", typeof(ShapeProperty) }
        };

        private static readonly Dictionary<Type, HashSet<string>> HiddenFieldsByType = new()
        {
            { typeof(ShapeProperty), new HashSet<string> { "isComposable" } }
        };

        private static bool IsHidden(SerializedProperty managedReference, SerializedProperty child)
        {
            if (managedReference.managedReferenceValue == null) return false;
            if (!HiddenFieldsByType.TryGetValue(managedReference.managedReferenceValue.GetType(), out var hidden)) return false;
            return hidden.Contains(child.name);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            float lineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;

            if (property.managedReferenceValue == null)
            {
                Rect buttonRect = new(position.x, position.y, position.width, lineHeight);
                if (EditorGUI.DropdownButton(buttonRect, new GUIContent("Select Property Type"), FocusType.Keyboard))
                {
                    GenericMenu menu = new();
                    foreach (var kvp in AllowedTypes)
                    {
                        string typeName = kvp.Key;
                        Type type = kvp.Value;
                        menu.AddItem(new GUIContent(typeName), false, () =>
                        {
                            property.managedReferenceValue = Activator.CreateInstance(type);
                            property.serializedObject.ApplyModifiedProperties();
                        });
                    }
                    menu.DropDown(buttonRect);
                }
            }
            else
            {
                string currentTypeName = property.managedReferenceFullTypename;
                string displayName = GetDisplayName(currentTypeName);

                Rect headerRect = new(position.x, position.y, position.width, lineHeight);
                Rect removeRect = new(position.x + position.width - lineHeight, position.y, lineHeight, lineHeight);

                EditorGUI.LabelField(headerRect, displayName, EditorStyles.boldLabel);

                if (GUI.Button(removeRect, "\u00D7"))
                {
                    property.managedReferenceValue = null;
                    property.serializedObject.ApplyModifiedProperties();
                    EditorGUI.EndProperty();
                    return;
                }

                float y = position.y + lineHeight + spacing;
                SerializedProperty child = property.Copy();
                SerializedProperty end = property.GetEndProperty();
                bool enterChildren = true;

                child.NextVisible(enterChildren);
                while (!SerializedProperty.EqualContents(child, end))
                {
                    if (!IsHidden(property, child))
                    {
                        float childHeight = EditorGUI.GetPropertyHeight(child, true);
                        Rect childRect = new(position.x, y, position.width, childHeight);
                        EditorGUI.PropertyField(childRect, child, true);
                        y += childHeight + spacing;
                    }
                    child.NextVisible(false);
                }
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float lineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;

            if (property.managedReferenceValue == null)
                return lineHeight;

            float height = lineHeight + spacing;
            SerializedProperty child = property.Copy();
            SerializedProperty end = property.GetEndProperty();
            bool enterChildren = true;

            child.NextVisible(enterChildren);
            while (!SerializedProperty.EqualContents(child, end))
            {
                if (!IsHidden(property, child))
                    height += EditorGUI.GetPropertyHeight(child, true) + spacing;
                child.NextVisible(false);
            }

            return height;
        }

        private static string GetDisplayName(string fullTypename)
        {
            string className = fullTypename.Split(',')[0];
            string shortName = className.Contains('.') ? className.Substring(className.LastIndexOf('.') + 1) : className;

            return shortName;
        }
    }
}
