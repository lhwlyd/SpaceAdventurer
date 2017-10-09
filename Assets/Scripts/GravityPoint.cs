using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityPoint : MonoBehaviour {

    public float pullRadius;
    public float pullForce;
    public float minRadius;
    public float distanceForceMultiplier;

    public LayerMask LayersToPull;

    // Update is called once per frame
    void FixedUpdate () {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, pullRadius, LayersToPull);

        foreach( Collider2D obj in colliders){
            Rigidbody2D rb2d = obj.GetComponent<Rigidbody2D>();

            if( rb2d == null ){
                continue;
            }

            Vector2 direction = transform.position - obj.transform.position;


            // Why????

            if( direction.magnitude < minRadius){
                continue;
            }


            float distance = direction.sqrMagnitude * distanceForceMultiplier + 1; // the distance formula ?

            rb2d.AddForce( direction.normalized * (pullForce / distance) * rb2d.mass * Time.deltaTime); 
        }

	}
}
