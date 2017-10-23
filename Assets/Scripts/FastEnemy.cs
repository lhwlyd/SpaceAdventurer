using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * This class represents a fast enemy.
 * This enemy will locate the player then charge at the player in a linear fashion. If this enemy misses the player, this enemy will 
 * re-adjust itself to linearly charge at the player again.
 */
public class FastEnemy : Enemy {
    public GameObject explosion;
    public bool hasCharged;

    /**
     * Movement logic: Get player's position (Do not keep updating the player's position. Only 
     * reset target once after done charging.)
     * Move towards that position linearly (1 direction/ axis)
     * Stop once at the player's position
     */ 
    protected override void FixedUpdate() {
        if (GameManager.instance.doingSetup)
            return;
    }

    protected override void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Player") {
            GameObject thisExplosion = Instantiate(explosion, this.gameObject.transform.position, Quaternion.identity) as GameObject;
            Destroy(thisExplosion, 2f);
        }
    }
}
