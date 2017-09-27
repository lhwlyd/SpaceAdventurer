using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraManager : MonoBehaviour {

	public GameObject player;

	private Vector3 offset;

	// Use this for initialization
	void Start() {
		offset = new Vector3(0,0,-10);
		UpdatePlayer ();
	}
	
	// Late update is called afte update every time.
	void Update () {
		transform.position = player.transform.position + offset;
	}

	void UpdatePlayer(){
		player = GameObject.FindGameObjectWithTag ("Player");
	}
}
