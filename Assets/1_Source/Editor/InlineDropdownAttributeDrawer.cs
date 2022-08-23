using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using System;
using UnityEditor;
using UnityEngine;

namespace TeamAlpha.Source.Editor
{
    public class InlineDropdownAttributeDrawer : OdinAttributeDrawer<InlineDropdownAttribute>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (Property.Children.Count > 1 == false || label == null)
            {
                CallNextDrawer(label);
                return;
            }

            EditorGUILayout.BeginHorizontal();
            Property.State.Expanded = EditorGUILayout.Foldout(Property.State.Expanded, label);
            Property.Children[0].Draw(GUIContent.none);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(2f);

            if (SirenixEditorGUI.BeginFadeGroup(this, Property.State.Expanded))
            {
                for (int i = 1; i < Property.Children.Count; i++)
                {
                    Property.Children[i].Draw();
                }
            }
            SirenixEditorGUI.EndFadeGroup();
        }
    }
}