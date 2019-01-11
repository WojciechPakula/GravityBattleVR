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

    public TextMesh textScoreA;
    public TextMesh textScoreB;
    public TextMesh textPlayer;
    public TextMesh textRound;

    public Gate gateA;
    public Gate gateB;

    void randomCannonPosition()
    {
        var rand = Random.Range(-maxCannonOffset, maxCannonOffset);
        var pos = cannon.transform.position;
        pos.z = rand;

        var rand2 = Random.Range(-maxCannonEotationOffset, maxCannonEotationOffset);
        Quaternion rot = Quaternion.Euler(0,rand2,0) * zeroRotation;

        var q = new Q_SET_CANNON_POSITION();
        q.qa = rot;
        q.position = pos;
        NetworkManager.instance.sendToAllComputers(q);

        setCannonPosition(pos, rot);
    }

    public void setCannonPosition(Vector3 pos, Quaternion rot)
    {
        cannon.transform.position = pos;
        cannon.transform.rotation = rot;
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
        gameInit();
    }

    void removeBlackHoles()
    {
        var m = FindObjectsOfType<Mass>();
        foreach (var mass in m)
        {
            Destroy(mass.gameObject);
        }
    }

    public int blackHolesPerTurn;
    public int turnsPerRound;
    int turn;
    void gameInit()
    {
        turn = 0;
        playerId = 0;
        setPlayerText();
        availableBlackHoles = blackHolesPerTurn;
        removeBlackHoles();
    }

    public void nextTurn()
    {
        availableBlackHoles = blackHolesPerTurn;
        turn++;
        if (turn == turnsPerRound*2)
        {
            turn = 0;
            round++;
            removeBlackHoles();
        }
        randomCannonPosition();
        setPlayerText();
    }

    void endTurn()
    {
        turnTrigger = false;
        nextTurn();
    }

    int swap=0;
    int playerId = 0;
    int round = 1;
    int availableBlackHoles = 0;
    void setPlayerText()
    {
        if ((turn+round) % 2 == 1)
        {
            textPlayer.text = "A";
            textPlayer.color = Color.red;
            playerId = 0;
        } else
        {
            textPlayer.text = "B";
            textPlayer.color = Color.blue;
            playerId = 1;
        }
    }

    int bulletStreamDelay = 0;
    bool turnTrigger = false;
    public void startCannon()
    {
        if (turnTrigger == true) return;
        cannon.bullets = 100;   //wystrzeliwuje 100 pocisków
        bulletStreamDelay = 100;
        turnTrigger = true;
        var q = new Q_RUN_ROUND();
        NetworkManager.instance.sendToAllComputers(q);
    }

    public void startCannonRemote()
    {
        if (turnTrigger == true) return;
        bulletStreamDelay = 100;
        turnTrigger = true;
    }


    // Update is called once per frame
    void Update () {
        if (bulletStreamDelay > 0) bulletStreamDelay--;
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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (NetworkManager.instance.getNetworkState() == NetworkState.NET_SERVER && playerId == 0)
                startCannon();
            else if (NetworkManager.instance.getNetworkState() == NetworkState.NET_CLIENT && playerId == 1)
                startCannon();
            else if (NetworkManager.instance.getNetworkState() == NetworkState.NET_DISABLED || NetworkManager.instance.getNetworkState() == NetworkState.NET_ENABLED)
                startCannon();
        }

        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            Transform objectHit = hit.transform;
            if (objectHit.name == "GhostPlane")
            {
                if (Input.GetMouseButtonDown(0) && turnTrigger == false)
                {
                    bool fail = true;
                    if (NetworkManager.instance.getNetworkState() == NetworkState.NET_SERVER && playerId == 0)
                        fail = false;
                    else if (NetworkManager.instance.getNetworkState() == NetworkState.NET_CLIENT && playerId == 1)
                        fail = false;
                    else if (NetworkManager.instance.getNetworkState() == NetworkState.NET_DISABLED || NetworkManager.instance.getNetworkState() == NetworkState.NET_ENABLED)
                        fail = false;

                    if (availableBlackHoles > 0 && !fail)
                    {
                        availableBlackHoles--;
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

        var photons = FindObjectsOfType<Photon>();
        if (photons.Length == 0 && bulletStreamDelay == 0 && turnTrigger == true)
        {
            endTurn();
        }

        textScoreA.text = gateA.score.ToString();
        textScoreB.text = gateB.score.ToString();
        textRound.text = round.ToString();
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
