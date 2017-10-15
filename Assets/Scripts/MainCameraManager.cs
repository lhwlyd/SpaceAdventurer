using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraManager : MonoBehaviour {

	public GameObject player;

	private Vector3 offset;

    public AudioClip[] bgms;

    public float dampTime = 0.1f;
    private Vector3 velocity = Vector3.zero;
    public Transform target;
    private Camera thisCamera;

    // Use this for initialization
    void Start()
    {
        //offset = new Vector3(0,0,-10);
        UpdatePlayer();

        this.GetComponent<AudioSource>().PlayOneShot(bgms[Random.Range(0, bgms.Length)]);
        this.GetComponent<AudioSource>().loop = true;
        thisCamera = this.GetComponent<Camera>();
    }
	

	void UpdatePlayer(){
		player = GameObject.FindGameObjectWithTag ("Player");
        target = player.transform;
	}

    void FixedUpdate()
    {
        if(target){
            Vector3 point = thisCamera.WorldToViewportPoint(target.position);
			Vector3 delta = target.position - thisCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z)); //(new Vector3(0.5, 0.5, point.z));
			Vector3 destination = transform.position + delta;
			transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
        }
    }

   
}
