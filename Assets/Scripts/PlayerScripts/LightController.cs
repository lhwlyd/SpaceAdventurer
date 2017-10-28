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
	}

	// Late update is called afte update every time.
	public void UpdateLight ( int curHealth ) {
        //transform.position = playerControllerTransform.transform.position + offset;
        characterLight.spotAngle = (int)(0.5 * curHealth) + 90;
	}

}
