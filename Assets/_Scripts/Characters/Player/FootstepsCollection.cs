using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Footsteps collection", menuName = "Create New Footsteps Collection")]

public class FootstepsCollection : ScriptableObject 
{ 
    public List<AudioClip> FootstepSounds = new List<AudioClip>();
    public AudioClip jumpSound;
    public AudioClip landSound;
}
