using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour {
    public IPEndPoint ip = null;
    public Text ipText;
    // Use this for initialization
    void Start () {
        Application.runInBackground = true;
        wlaczSiec();
    }
	
	// Update is called once per frame
	void Update () {
        NetworkManager.instance.update();
        if (ip != null)
        {
            ipText.text = ip.ToString();
        } else
        {
            ipText.text = "brak";
        }
    }

    public void loadHTCvive()
    {
        SceneManager.LoadScene("HTCvive_test");
    }

    public void load3Dvision()
    {
        SceneManager.LoadScene("3Dvision_test");
    }

    public void startGry()
    {
        SceneManager.LoadScene("Board");
    }

    public void postawSerwer()
    {
        Debug.Log("R");
        NetworkManager.instance.runSerwer();
    }

    public void wlaczSiec()
    {
        Debug.Log("E");
        NetworkManager.instance.enableNetwork();
        }

    public void szukajSerwera()
    {
        Debug.Log("B");
        ip = null;
        NetworkManager.instance.sendBroadcast(new Q_SERVER_INFO_REQUEST());
    }

    public void dolaczDoPierwszegoLepszego()
    {
        if (ip != null)
        {
            Debug.Log("C");
            NetworkManager.instance.connectToSerwer(ip);
        }
    }

    public void testujPolaczenie()
    {
        Debug.Log("H");
        NetworkManager.instance.sendToServerToAll(new Q_HELLO { text = "komunikat" });
    }

    /*private void OnDestroy()
    {
        NetworkManager.instance.kill();
    }*/
}
