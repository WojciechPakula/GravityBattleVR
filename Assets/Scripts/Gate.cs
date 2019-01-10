using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour {
    void OnTriggerEnter(Collider other)
    {
        var ph = other.gameObject.GetComponent<Photon>();
        ph.setColor(Color.white);
    }

    void Update()
    {
        
    }
    void Start()
    {
        
    }
}
