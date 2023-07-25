using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS.Dialogue 
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue")]
    public class Dialogue : ScriptableObject
    {
        [SerializeField]
        List<DialogueNode> nodes = new List<DialogueNode>();

        Dictionary<string, DialogueNode> nodeLookup =   new Dictionary<string, DialogueNode>();

#if UNITY_EDITOR
        private void Awake() {
            if (nodes.Count == 0) {
                nodes.Add(new DialogueNode());
            }
        }

        private void OnValidate() {
            nodeLookup.Clear();
            foreach (DialogueNode node in GetAllNodes()) {
                nodeLookup[node.uniqueID] = node;
            }
        }
#endif
        public IEnumerable<DialogueNode> GetAllNodes() { return nodes; }

        public DialogueNode GetRootNode() { return nodes[0]; }

        public IEnumerable<DialogueNode> GetAllNodes(DialogueNode parendNode)
        {
            List<DialogueNode> result = new List<DialogueNode>();
            foreach (string childID in parendNode.children)
            {
               if  (nodeLookup.ContainsKey(childID)) {
                    yield return nodeLookup[childID];
                }
            }
        }
    }
}
