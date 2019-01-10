using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;   //instancja
    public Camera camera;

    public Vector3 pos1;
    public Quaternion qa1;
    public Vector3 pos2;
    public Quaternion qa2;

    public cannonScript cannon;

    public GameObject p1;
    public GameObject p2;

    public float maxCannonOffset;
    public float maxCannonEotationOffset;

    GameObject HTC_pilot_position;

    Quaternion zeroRotation;

    void randomCannonPosition()
    {
        var rand = Random.Range(-maxCannonOffset, maxCannonOffset);
        var pos = cannon.transform.position;
        pos.z = rand;
        cannon.transform.position = pos;

        var rand2 = Random.Range(-maxCannonEotationOffset, maxCannonEotationOffset);
        cannon.transform.rotation = Quaternion.Euler(0,rand2,0) * zeroRotation;
    }

    // Use this for initialization
    void Start () {
        Application.runInBackground = true;
        instance = this.GetComponent<GameManager>();
        pos1 = p1.transform.position;
        qa1 = p1.transform.rotation;
        pos2 = p2.transform.position;
        qa2 = p2.transform.rotation;
        zeroRotation = cannon.transform.rotation;
    }

	
	// Update is called once per frame
	void Update () {
        float visibleOffset = 2f;
        //if ((p1.transform.position - camera.transform.position).magnitude < visibleOffset) p1.GetComponent<MeshRenderer>().enabled = false; else p1.GetComponent<MeshRenderer>().enabled = true;
        //if ((p2.transform.position - camera.transform.position).magnitude < visibleOffset) p2.GetComponent<MeshRenderer>().enabled = false; else p2.GetComponent<MeshRenderer>().enabled = true;

        if (NetworkManager.instance.getNetworkState() == NetworkState.NET_SERVER) {
            p1.GetComponent<MeshRenderer>().enabled = false;
            pos1 = camera.transform.position;
            qa1 = camera.transform.rotation;
            var q = new Q_SET_PLAYER_AVATAR();
            q.position = pos1;
            q.qa = qa1;
            q.type = 1;
            NetworkManager.instance.sendToAllComputers(q);
        } else p1.GetComponent<MeshRenderer>().enabled = true;
        if (NetworkManager.instance.getNetworkState() == NetworkState.NET_CLIENT) {
            p2.GetComponent<MeshRenderer>().enabled = false;
            pos2 = camera.transform.position;
            qa2 = camera.transform.rotation;
            var q = new Q_SET_PLAYER_AVATAR();
            q.position = pos2;
            q.qa = qa2;
            q.type = 2;
            NetworkManager.instance.sendToAllComputers(q);
        } else p2.GetComponent<MeshRenderer>().enabled = true;

        p1.transform.position = Vector3.Lerp(p1.transform.position, pos1, 20.0f * Time.deltaTime);
        p2.transform.position = Vector3.Lerp(p2.transform.position, pos2, 20.0f * Time.deltaTime);

        try { p1.transform.rotation = Quaternion.Lerp(p1.transform.rotation, qa1, 20.0f * Time.deltaTime); } catch { }
        try { p2.transform.rotation = Quaternion.Lerp(p2.transform.rotation, qa2, 20.0f * Time.deltaTime); } catch { }
        
        NetworkManager.instance.update();
        if (Input.GetKeyDown(KeyCode.H))
        {
            Debug.Log("H");
            NetworkManager.instance.sendToAllComputers(new Q_HELLO { text = "komunikat" });
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("R");
            randomCannonPosition();
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

    public void HTCbuttonClick()
    {
        placeBlackHole(HTC_pilot_position.transform.position, 0.6f);
    }

    public void HTCsetPilotPosition(Vector3 pilot)
    {
        HTC_pilot_position.transform.position = pilot;
    }
}
