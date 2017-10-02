using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radar : MonoBehaviour {

    public List<GameObject> trackedObjects;
    List<GameObject> radarObjects;
    public GameObject radarPrefab;
    List<GameObject> borderObjects;

    public float switchDistance = 10f;

    public Transform helpTransform;

    // Awake is important! Awake is called when before/when this class is refed elsewhere
    void Awake () {

        // Reserving this list for future use. Not serving any purpose for now.
        trackedObjects = new List<GameObject>();
        radarObjects = new List<GameObject>();
        borderObjects = new List<GameObject>();
    }
	
	// Update is called once per frame
	void Update () {
        if( radarObjects == null || borderObjects == null){
            return;
        }
        for (int i = 0; i < radarObjects.Count; i++){
            if(Vector2.Distance(radarObjects[i].transform.position, transform.position) > switchDistance){
                //switch this object to the border obj
                helpTransform.LookAt(radarObjects[i].transform);
                borderObjects[i].transform.position = transform.position + switchDistance * helpTransform.forward;

                borderObjects[i].layer = LayerMask.NameToLayer("MinimapLayer");
                radarObjects[i].layer = LayerMask.NameToLayer("Invisible");
            } else {
                //switch this obj to regular obj
                borderObjects[i].layer = LayerMask.NameToLayer("Invisible");
				radarObjects[i].layer = LayerMask.NameToLayer("MinimapLayer");

			}
        }
	}

    void CreateRadarObjects( GameObject o ){
		/*radarObjects = new List<GameObject>();
        foreach( GameObject o in trackedObjects){
            GameObject k = Instantiate(radarPrefab, o.transform.position, Quaternion.identity) as GameObject;
            radarObjects.Add(k);

            GameObject j = Instantiate(radarPrefab, o.transform.position, Quaternion.identity) as GameObject;
            borderObjects.Add(j);
        }
        */

		GameObject k = Instantiate(radarPrefab, o.transform.position, Quaternion.identity) as GameObject;
		radarObjects.Add(k);

		GameObject j = Instantiate(radarPrefab, o.transform.position, Quaternion.identity) as GameObject;
		borderObjects.Add(j);
    }

    public void AddToTrackedObjects(GameObject obj){
        trackedObjects.Add(obj);

        CreateRadarObjects(obj);
    }
}
