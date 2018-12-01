using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void loadHTCvive()
    {
        SceneManager.LoadScene("HTCvive_test");
    }

    public void load3Dvision()
    {
        SceneManager.LoadScene("3Dvision_test");
    }
}
