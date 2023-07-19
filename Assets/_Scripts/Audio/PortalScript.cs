using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using Unity.VisualScripting;

[RequireComponent(typeof(StudioEventEmitter))]

public class PortalScript : MonoBehaviour
{
    private StudioEventEmitter emitter;

    private void Start()
        {
           emitter = AudioManager.instance.InitializeEventEmitter(FMODEvents.instance.portalIdleSounds, this.gameObject);
           print(emitter);
           //emitter.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position));
           emitter.Play();
        }

    //private void ActivatePortal()
    //{

    //}

    // private void OnCollisionEnter(Collision collision) {
    //     if (collision.gameObject.name == "Player") 
    //         emitter.Stop();
    // }

}
