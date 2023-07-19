using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Notes_Interactable : Interactable
{
    [SerializeField] private FirstPersonController player;
    [SerializeField] private GameObject noteTextUI;
    public AudioSource diana_VoiceSound;

    public override void OnInteract()
    {
        print("INTERACT WITH " + gameObject.name);
        noteTextUI.SetActive(true);
        player.canMove = false;
        diana_VoiceSound.Play();


    }
    public override void OnFocus()
    {
        print("LOOKING AT " + gameObject.name);

    }
    public override void OnLoseFocus()
    {
        print("STOPPED LOOKING AT " + gameObject.name);
    }
}
