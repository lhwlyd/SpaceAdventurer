using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroying : Interactive {

    public int damageToPlayer;

    /*(
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
    */

    void Interact(GameObject inputGameobject){
		string thisTag = inputGameobject.tag;
		if (thisTag == "Obstacle" || thisTag == "Torch" || thisTag == "Enemy")
		{
			Destroy(inputGameobject);
		}

		if (thisTag == "Player")
		{
			inputGameobject.SendMessage("LoseHp", damageToPlayer);
		}
    }
}
