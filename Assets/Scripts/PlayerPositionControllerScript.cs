using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPositionControllerScript : MonoBehaviour {

    public Transform playerPositionController;

    public GameObject player;


	// Use this for initialization
	void Start () {
        player = GameObject.FindWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = player.transform.position;
	}
}
