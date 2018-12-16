using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class losowyEmiter : MonoBehaviour {
    float time = 0;
    public float okres = 1;

	void Update () {
        time += Time.deltaTime;
        if (time > okres)
        {
            time = 0;
            var losowyKierunek = Random.onUnitSphere;
            PhotonEmitter.emitPhoton(transform.position, losowyKierunek);
        }
	}
}
