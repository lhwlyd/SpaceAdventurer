using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DumbEnemy : Enemy {

    public GameObject explosion;

    // Basic functions inherited from the Enemy class

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if( collision.gameObject.tag == "Player"){
			GameObject thisExplosion = Instantiate(explosion, this.gameObject.transform.position, Quaternion.identity) as GameObject;
			Destroy(thisExplosion, 1f);
        }

    }
}
