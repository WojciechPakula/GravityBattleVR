using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraScript : MonoBehaviour {

	float cameraSpeed = 25.0f; //podstawowa prędkość ruchu kamery - 25 to taki sweet spot, ewentualnie dać mniej, nie więcej
	float cameraSensitivity = 0.25f; //czułość kamery przy użyciu myszki
	private bool isRotating;
	private Vector3 lastMousePos = new Vector3(255,255,255);

    private Vector3 pivot;
    private Vector3 pivotMouse;

    void Start () {
        pivot = this.transform.position;
    }
	
	void Update () {
        pivotMouse = Vector3.Lerp(pivotMouse, Input.mousePosition, Time.deltaTime * 10f);
        
        if (Input.GetMouseButtonDown(1)) {
			lastMousePos = pivotMouse;
			isRotating = true;
		}
		
		if (!Input.GetMouseButton(1)) isRotating = false;
		
		//obsługa myszki
		if (isRotating) {
			lastMousePos = pivotMouse - lastMousePos;
			lastMousePos = new Vector3(-lastMousePos.y * cameraSensitivity, lastMousePos.x * cameraSensitivity, 0);
            lastMousePos = new Vector3(transform.eulerAngles.x + lastMousePos.x, transform.eulerAngles.y + lastMousePos.y, 0);
            transform.eulerAngles = lastMousePos;
            lastMousePos = pivotMouse;
		}

		//obsługa klawiatury
		float f = 0.0f;
		Vector3 p = GetBaseInput();
		p = p * cameraSpeed;
		p = p * Time.deltaTime;
        p = this.transform.rotation * p;
        pivot += p;
        this.transform.position = Vector3.Lerp(this.transform.position, pivot, Time.deltaTime*10f);
    }

    private Vector3 GetBaseInput() {
		Vector3 p_Velocity = new Vector3();
		if (Input.GetKey (KeyCode.W)) {
			p_Velocity += new Vector3(0, 0, 1);
		}
		if (Input.GetKey (KeyCode.S)) {
			p_Velocity += new Vector3(0, 0, -1);
		}
		if (Input.GetKey (KeyCode.A)) {
			p_Velocity += new Vector3(-1, 0, 0);
		}
		if (Input.GetKey (KeyCode.D)) {
			p_Velocity += new Vector3(1, 0, 0);
		}
		return p_Velocity;
	}
}
