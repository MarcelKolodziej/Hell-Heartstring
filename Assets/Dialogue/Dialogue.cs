using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS.Dialogue 
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue")]
    public class Dialogue : ScriptableObject {
        [SerializeField]
        DialogueNode[] nodes;
    }

}
