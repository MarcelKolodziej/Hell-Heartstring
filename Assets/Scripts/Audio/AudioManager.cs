using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Runtime.CompilerServices;
using System.Diagnostics.Tracing;

public class AudioManager : MonoBehaviour
{
    private List<EventInstance> eventInstances;

    private List<StudioEventEmitter> eventEmitters;
   public static AudioManager instance { get; private set;  }

   private void Awake() 
   {
        if (instance != null) 
        {
            Debug.LogError("You have more than one AudioManager!");
        }

        instance = this;

        eventInstances= new List<EventInstance>();
        eventEmitters = new List<StudioEventEmitter>();

    }

       public void PlayeOneShot(EventReference sound, Vector3 worldPos) 
        {
            RuntimeManager.PlayOneShot(sound, worldPos);
        }

    public EventInstance CreateEventInstance(EventReference eventReference) 
        {
            EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
            eventInstances.Add(eventInstance);
            return eventInstance;
        }

    public StudioEventEmitter InitializeEventEmitter(EventReference eventReference, GameObject emitterGameObject) 
        {
            StudioEventEmitter emitter = emitterGameObject.GetComponent<StudioEventEmitter>();
            emitter.EventReference = eventReference;
            eventEmitters.Add(emitter);
            return emitter;
        }
    private void CleanUp()
        {
        foreach (EventInstance eventInstace in eventInstances)
         {
            eventInstace.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstace.release();
        }
         foreach (StudioEventEmitter emitter in eventEmitters) 
         {
            emitter.Stop();
        }
    }
       private void OnDestroy()
        {
            CleanUp();
        }

}
