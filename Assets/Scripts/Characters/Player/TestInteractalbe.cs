using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInteractalbe : Interactable
{
    public override void OnInteract()
    {
        print("INTERACT WITH " + gameObject.name);
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
