using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroying : MonoBehaviour {

    public int damageToPlayer;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Obstacle" || collision.gameObject.tag == "Torch" ||
           collision.gameObject.tag == "Enemy"){
            Destroy(collision.gameObject);
        }

        if(collision.gameObject.tag == "Player"){
            Invoke("LoseHp", damageToPlayer);
        }

    }
}
