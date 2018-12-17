using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;   //instancja
    public Camera camera;
    // Use this for initialization
    void Start () {
        Application.runInBackground = true;
        instance = this.GetComponent<GameManager>();
    }
	
	// Update is called once per frame
	void Update () {
        NetworkManager.instance.update();
        if (Input.GetKeyDown(KeyCode.H))
        {
            Debug.Log("H");
            NetworkManager.instance.sendToAllComputers(new Q_HELLO { text = "komunikat" });
        }

        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            Transform objectHit = hit.transform;
            if (objectHit.name == "Plane")
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Debug.Log(hit.point);
                    float r = 0.6f;
                    placeBlackHole(hit.point, r);

                    var q = new Q_SPAWN_BLACKHOLE();
                    q.position = hit.point;
                    q.radius = r;
                    NetworkManager.instance.sendToAllComputers(q);
                }
            }
        }
        
    }

    static int blackHoleIndex = 0;
    public void placeBlackHole(Vector3 position, float r)
    {
        position.y = 0;
        GameObject f = (GameObject)Instantiate(Resources.Load("czarnaDziura"));
        f.name = "czarnaDziura" + blackHoleIndex++;
        var ms = f.GetComponent<Mass>();
        f.transform.position = position;
        ms.promien = r;
    }
}
