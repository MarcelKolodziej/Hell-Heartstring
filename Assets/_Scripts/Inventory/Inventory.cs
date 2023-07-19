using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    Item[] contest;
    // Start is called before the first frame update
    void Start()
    {
        foreach (var item in contest) {
            print($"Has item: {item.GetName()}");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
