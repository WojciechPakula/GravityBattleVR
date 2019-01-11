using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour {
    public int score = 0;
    void OnTriggerEnter(Collider other)
    {
        try
        {
            var ph = other.gameObject.GetComponent<Photon>();
            ph.setColor(Color.white);
            score++;
        } catch
        {

        }
    }

    void Update()
    {
        
    }
    void Start()
    {
        
    }
}
