using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mass : MonoBehaviour {
    public float _mass;
    public float promien;

    public float rs { get; private set; }
    public float rf { get; private set; }

    public float mass
    {
        get { return _mass; }
        set {
            _mass = value;
            rs = 2 * _mass * (float)PhotonPhysics.G / (float)(PhotonPhysics.c * PhotonPhysics.c);
            rf = 3 * _mass * (float)PhotonPhysics.G / (float)(PhotonPhysics.c * PhotonPhysics.c);
        }
    }

    // Use this for initialization
    void Awake () {
        mass = _mass;//return;
    }

	// Update is called once per frame
	void Update () {
        promien = rs;
        this.transform.localScale = new Vector3(promien*2, promien*2, promien*2);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        float radius = (float)((3.0 * PhotonPhysics.G * mass) / (PhotonPhysics.c * PhotonPhysics.c));
        Gizmos.DrawWireSphere(transform.position, radius);
        Gizmos.color = Color.red;
        radius = (float)((2.0 * PhotonPhysics.G * mass) / (PhotonPhysics.c * PhotonPhysics.c));        
        //Gizmos.DrawWireSphere(transform.position, radius);
        Gizmos.DrawWireSphere(transform.position, radius);
        /*Gizmos.color = Color.yellow;
        radius = (float)((1.0 * Photon.G * mass) / (Photon.c * Photon.c));
        Gizmos.DrawWireSphere(transform.position, radius);
        promien = radius;
        radius *= 2.0f/3.0f;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);*/
    }
}
