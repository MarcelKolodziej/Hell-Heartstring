using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAnimator : MonoBehaviour
{
    public Vector3 movement;
    public GameObject lookAt;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.Translate(movement);
        if (lookAt != null) 
        {
            transform.LookAt(lookAt.transform.position);
        }
    }
}
