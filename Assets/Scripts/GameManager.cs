using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;   //instancja
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
    }
}
