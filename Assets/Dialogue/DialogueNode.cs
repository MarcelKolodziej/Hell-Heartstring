using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS.Dialogue {

    [Serializable]
    public class DialogueNode {
        public string uniqueID;
        public string name;
        public string text;
        public string[] children;

    }
}