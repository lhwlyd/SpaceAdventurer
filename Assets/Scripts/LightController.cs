using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour {

    public Transform playerControllerTransform;
    public GameObject player;

	private Vector3 offset;

	private Light characterLight;

	// Use this for initialization
	void Start() {
		//offset = new Vector3(0,0,-3);
		characterLight = GetComponent<Light> ();
        //playerControllerTransform = GameObject.FindGameObjectWithTag("PlayerPositionController").transform;
        UpdatePlayer();
	}

	// Late update is called afte update every time.
	void Update () {
        //transform.position = playerControllerTransform.transform.position + offset;
		characterLight.spotAngle = (int)(0.2 * player.GetComponent<Player>().hp) + 120;
	}

	void UpdatePlayer(){
		player = GameObject.FindGameObjectWithTag ("Player");
	}
}
