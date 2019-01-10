using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Photon : MonoBehaviour {
    public Vector3d momentumD = new Vector3d(0, 0, 1);
    public Vector3d positionD;
    LineRenderer lr;

    public void setColor(Color col)
    {
        var tmp = lr.material.GetColor("_TintColor");
        lr.material.SetColor("_TintColor", col);
        lifeTime = -2;
    }

    public float lifeTime = 0;
    
    float maxLifeTime = 7f;

    public void synchPisition()
    {
        positionD = Vector3d.f_to_d(transform.position);
    }

    void Awake () {
        lr = GetComponent<LineRenderer>();
        positionD = Vector3d.f_to_d(transform.position);
    }
    
    public bool locker = false;

    public void Update()
    {
        float dt = 100.0f * 0.01f * 22 / 2 / 2 / 2 / 100;   //podstawa czasu musi być identyczna przy każdej iteracji
        lifeTime += Time.deltaTime;

        float al = 2-(lifeTime/maxLifeTime)*2;
        if (al > 1) al = 1;
        var tmp = lr.startColor;
        tmp.a = al;
        lr.startColor = tmp;
        lr.endColor = tmp;
        

        if (lifeTime > maxLifeTime) Destroy(this.gameObject);
        if (locker) return;
        if (transform.position.magnitude > 100) locker = true;

        Mass[] masses = FindObjectsOfType<Mass>();
        bool outLocker;
        
        for (int i = 0; i < 10; ++i)
        {
            if (locker)
            {
                this.transform.position = Vector3d.d_to_f(positionD);
                dodajPunkt();
                return;
            }

            Vector3d position1 = new Vector3d();
            Vector3d momentum1 = new Vector3d();
            PhotonPhysics.getNextPosition(positionD, momentumD, masses, dt, out position1, out momentum1, out outLocker);
            locker = outLocker;
            positionD = position1;
            momentumD = momentum1;
        }

        this.transform.position = Vector3d.d_to_f(positionD);
        dodajPunkt();
    }

    void dodajPunkt()
    {
        lr.positionCount++;
        lr.SetPosition(lr.positionCount-1, this.transform.position);        
    }
}
