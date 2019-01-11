using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cannonScript : MonoBehaviour {
    public float dispersion = 2;
    // Use this for initialization
    void Start () {
		
	}

    public int bullets = 0;

	// Update is called once per frame
	void Update () {
        if (/*Input.GetKey(KeyCode.Space) || */bullets > 0)
        {
            var dir = new Vector3(0,-1,0);

            dir = Quaternion.Euler(new Vector3(rand(), 0, 0)) * dir;//x
            dir = Quaternion.Euler(new Vector3(0, Random.value * 360.0f, 0)) * dir;//y

            dir = this.transform.rotation * dir;

            //dir = Quaternion.Euler(0, 0, 45) * dir;
            var q = new Q_SHOOT_PHOTON();
            q.direction = dir;
            q.position = transform.position;
            NetworkManager.instance.sendToAllComputers(q);
            PhotonEmitter.emitPhoton(transform.position, dir);
            bullets--;
            if (bullets < 0) bullets = 0;
        }
	}

    float rand()
    {
        float mean = 0;
        float u1 = 1.0f - Random.value; //uniform(0,1] random doubles
        float u2 = 1.0f - Random.value;
        float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2); //random normal(0,1)
        return mean + dispersion * randStdNormal; //random normal(mean,stdDev^2)
    }
}
