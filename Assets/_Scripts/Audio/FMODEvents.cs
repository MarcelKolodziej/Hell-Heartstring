using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMODEvents : MonoBehaviour
{
    [field: Header("Footsteps SFX")]
    [field: SerializeField] public EventReference footStepsSounds {  get; private set; }
    [field: SerializeField] public EventReference portalIdleSounds {  get; private set; }

    public static FMODEvents instance { get; private set; }
    private void Awake() {
           if (instance != null) 
           {
                Debug.LogError("You have more then one FmodsEvent instance in project!");
            }
           instance = this;
    }
}
