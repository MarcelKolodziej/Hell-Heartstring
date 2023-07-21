using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Drawing.Printing;
using PlasticPipe.PlasticProtocol.Messages;
using Unity.VisualScripting;
using Codice.Client.BaseCommands.CheckIn;
using UnityEngine.UIElements;
using UnityEditor.Rendering;
using System;

namespace FPS.Dialogue.Editor
{
    public class DialogueEditor : EditorWindow {
        Dialogue selectedDialogue = null;
        GUIStyle nodeStyle;
        DialogueNode draggingNode = null;
        Vector2 draggingOffset;

        [MenuItem("Window/Dialogue Editor")]
        public static void ShowEditorWindow() {
            GetWindow(typeof(DialogueEditor), false, "Dialog Editor");

        }

        [OnOpenAssetAttribute(1)]
        public static bool OnOpenAsset(int instanceID, int line) {
            Dialogue dialogue = EditorUtility.InstanceIDToObject(instanceID) as Dialogue;
            if (dialogue != null) {
                ShowEditorWindow();
                return true;
            }
            return false;

        }

        private void OnEnable() {
            Selection.selectionChanged += OnSelectionChanged;

            nodeStyle = new GUIStyle();
            nodeStyle.normal.background = EditorGUIUtility.Load("node0") as Texture2D;
            nodeStyle.normal.textColor = Color.white;
            nodeStyle.padding = new RectOffset(20, 20, 20, 20);
            nodeStyle.border = new RectOffset(12, 12, 12, 12);
        }

        private void OnSelectionChanged() {
            Debug.Log("On Selection Changed");
            Dialogue newDialogue = Selection.activeObject as Dialogue;
            if (newDialogue != null) {
                selectedDialogue = newDialogue;
                Repaint();
            }
        }
        void OnGUI() {
            if (selectedDialogue == null) {
                EditorGUILayout.LabelField("There's no dialogue ");
            } else {
                ProcessEvents();
                foreach (DialogueNode node in selectedDialogue.GetAllNodes()) {
                    OnGUInode(node);
                }
            }

        }
        private void ProcessEvents()  // mouse events for dragging nodes
        {
            if (Event.current.type == EventType.MouseDown && draggingNode == null)
            {
                draggingNode = GetNodeAtPoint(Event.current.mousePosition);
                if (draggingNode != null) {
                    draggingOffset = (draggingNode.rect.position - Event.current.mousePosition);

                }
            }
             else if (Event.current.type == EventType.MouseDrag && draggingNode != null)  // handle draggin
            {
                Undo.RecordObject(selectedDialogue, "Move Dialogue node");
                draggingNode.rect.position = Event.current.mousePosition + draggingOffset;
                GUI.changed = true; // more accurate then repaint()
            } 
            else if (Event.current.type == EventType.MouseUp && draggingNode != null)
            {
                draggingNode = null;
            }
        }
        private void OnGUInode(DialogueNode node)
        {
            GUILayout.BeginArea(node.rect, nodeStyle);
            EditorGUI.BeginChangeCheck(); // before field changes data

            EditorGUILayout.LabelField("Node: ", EditorStyles.whiteLabel);
            string newText = EditorGUILayout.TextField(node.text);
            string newUniqueID = EditorGUILayout.TextField(node.uniqueID);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(selectedDialogue, "Update Dialogue Text");

                node.text = newText;
                node.uniqueID = newUniqueID;
            }
            GUILayout.EndArea();
        }
        private DialogueNode GetNodeAtPoint(Vector2 point) {
            DialogueNode foundNode = null;
            // Loop over all nodes
            foreach ( DialogueNode node in selectedDialogue.GetAllNodes())
            {
                if (node.rect.Contains(point)) 
                    foundNode =  node;
            }
            // Return the correct node.
            return foundNode;
        }
    }

}
