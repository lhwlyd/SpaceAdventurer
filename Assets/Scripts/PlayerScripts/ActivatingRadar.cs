using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
 * This is used to activate all objects with Update functions if and only if
 * they are within a certain range near the player. The purpose of doing so 
 * is to cut the unnecessary calculations off and potentially help the 
 * enemies perform better( the can rest at where they are originally!)
 * 
 * @Author: Roy Luo
 */
public class ActivatingRadar : MonoBehaviour {

    public float activatingRadius;

    public LayerMask LayersToActivate;

    // Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, activatingRadius, LayersToActivate);

        foreach( Collider2D obj in colliders ){
            Activatable activator;
            if( (activator = obj.GetComponent<Activatable>()) != null){
                activator.SendMessage("Activate");
            }
        }




	}



}
