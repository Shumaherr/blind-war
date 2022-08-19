﻿using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FMODUnity
{
    [CustomPropertyDrawer(typeof(EventRefAttribute))]
    internal class EventRefDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var browseIcon = EditorGUIUtility.Load("FMOD/SearchIconBlack.png") as Texture;
            var openIcon = EditorGUIUtility.Load("FMOD/BrowserIcon.png") as Texture;
            var addIcon = EditorGUIUtility.Load("FMOD/AddIcon.png") as Texture;

            label = EditorGUI.BeginProperty(position, label, property);
            var pathProperty = property;

            var e = Event.current;
            if (e.type == EventType.DragPerform && position.Contains(e.mousePosition))
                if (DragAndDrop.objectReferences.Length > 0 &&
                    DragAndDrop.objectReferences[0] != null &&
                    DragAndDrop.objectReferences[0].GetType() == typeof(EditorEventRef))
                {
                    pathProperty.stringValue = ((EditorEventRef)DragAndDrop.objectReferences[0]).Path;
                    GUI.changed = true;
                    e.Use();
                }

            if (e.type == EventType.DragUpdated && position.Contains(e.mousePosition))
                if (DragAndDrop.objectReferences.Length > 0 &&
                    DragAndDrop.objectReferences[0] != null &&
                    DragAndDrop.objectReferences[0].GetType() == typeof(EditorEventRef))
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                    DragAndDrop.AcceptDrag();
                    e.Use();
                }

            var baseHeight = GUI.skin.textField.CalcSize(new GUIContent()).y;

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.padding.top = 1;
            buttonStyle.padding.bottom = 1;

            var addRect = new Rect(position.x + position.width - addIcon.width - 7, position.y, addIcon.width + 7,
                baseHeight);
            var openRect = new Rect(addRect.x - openIcon.width - 7, position.y, openIcon.width + 6, baseHeight);
            var searchRect = new Rect(openRect.x - browseIcon.width - 9, position.y, browseIcon.width + 8, baseHeight);
            var pathRect = new Rect(position.x, position.y, searchRect.x - position.x - 3, baseHeight);

            EditorGUI.PropertyField(pathRect, pathProperty, GUIContent.none);

            if (GUI.Button(searchRect, new GUIContent(browseIcon, "Search"), buttonStyle))
            {
                var eventBrowser = ScriptableObject.CreateInstance<EventBrowser>();

                eventBrowser.ChooseEvent(property);
                var windowRect = position;
                windowRect.position = GUIUtility.GUIToScreenPoint(windowRect.position);
                windowRect.height = openRect.height + 1;
                eventBrowser.ShowAsDropDown(windowRect, new Vector2(windowRect.width, 400));
            }

            if (GUI.Button(addRect, new GUIContent(addIcon, "Create New Event in Studio"), buttonStyle))
            {
                var addDropdown = ScriptableObject.CreateInstance<CreateEventPopup>();

                addDropdown.SelectEvent(property);
                var windowRect = position;
                windowRect.position = GUIUtility.GUIToScreenPoint(windowRect.position);
                windowRect.height = openRect.height + 1;
                addDropdown.ShowAsDropDown(windowRect, new Vector2(windowRect.width, 500));
            }

            if (GUI.Button(openRect, new GUIContent(openIcon, "Open In Browser"), buttonStyle) &&
                !string.IsNullOrEmpty(pathProperty.stringValue) &&
                EventManager.EventFromPath(pathProperty.stringValue) != null
            )
            {
                EventBrowser.ShowWindow();
                var eventBrowser = EditorWindow.GetWindow<EventBrowser>();
                eventBrowser.FrameEvent(pathProperty.stringValue);
            }

            if (!string.IsNullOrEmpty(pathProperty.stringValue) &&
                EventManager.EventFromPath(pathProperty.stringValue) != null)
            {
                var foldoutRect = new Rect(position.x + 10, position.y + baseHeight, position.width, baseHeight);
                property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, "Event Properties");
                if (property.isExpanded)
                {
                    var style = new GUIStyle(GUI.skin.label);
                    style.richText = true;
                    var eventRef = EventManager.EventFromPath(pathProperty.stringValue);
                    var width = style.CalcSize(new GUIContent("<b>Oneshot</b>")).x;
                    var labelRect = new Rect(position.x, position.y + baseHeight * 2, width, baseHeight);
                    var valueRect = new Rect(position.x + width + 10, position.y + baseHeight * 2, pathRect.width,
                        baseHeight);

                    if (pathProperty.stringValue.StartsWith("{"))
                    {
                        GUI.Label(labelRect, new GUIContent("<b>Path</b>"), style);
                        EditorGUI.SelectableLabel(valueRect, eventRef.Path);
                    }
                    else
                    {
                        GUI.Label(labelRect, new GUIContent("<b>GUID</b>"), style);
                        EditorGUI.SelectableLabel(valueRect, eventRef.Guid.ToString("b"));
                    }

                    labelRect.y += baseHeight;
                    valueRect.y += baseHeight;

                    GUI.Label(labelRect, new GUIContent("<b>Banks</b>"), style);
                    GUI.Label(valueRect, string.Join(", ", eventRef.Banks.Select(x => x.Name).ToArray()));
                    labelRect.y += baseHeight;
                    valueRect.y += baseHeight;

                    GUI.Label(labelRect, new GUIContent("<b>Panning</b>"), style);
                    GUI.Label(valueRect, eventRef.Is3D ? "3D" : "2D");
                    labelRect.y += baseHeight;
                    valueRect.y += baseHeight;

                    GUI.Label(labelRect, new GUIContent("<b>Stream</b>"), style);
                    GUI.Label(valueRect, eventRef.IsStream.ToString());
                    labelRect.y += baseHeight;
                    valueRect.y += baseHeight;

                    GUI.Label(labelRect, new GUIContent("<b>Oneshot</b>"), style);
                    GUI.Label(valueRect, eventRef.IsOneShot.ToString());
                    labelRect.y += baseHeight;
                    valueRect.y += baseHeight;
                }
            }
            else
            {
                var labelRect = new Rect(position.x, position.y + baseHeight, position.width, baseHeight);
                GUI.Label(labelRect,
                    new GUIContent("Event Not Found", EditorGUIUtility.Load("FMOD/NotFound.png") as Texture2D));
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var expanded = property.isExpanded && !string.IsNullOrEmpty(property.stringValue) &&
                           EventManager.EventFromPath(property.stringValue) != null;
            var baseHeight = GUI.skin.textField.CalcSize(new GUIContent()).y;
            return baseHeight * (expanded ? 7 : 2); // 6 lines of info
        }
    }
}