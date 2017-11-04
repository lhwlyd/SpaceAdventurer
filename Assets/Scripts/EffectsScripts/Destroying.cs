using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroying : Interactive {

    void Interact(GameObject inputGameobject){
		string thisTag = inputGameobject.tag;
		if (thisTag == "Obstacle" || thisTag == "Torch" || thisTag == "Enemy")
		{
			Destroy(inputGameobject);
		}

        /* Moved to a new effect called hurting. This being here will cause the 
         * screen to flash red when we don't want it to.
         *
		if (thisTag == "Player")
		{
			inputGameobject.SendMessage("LoseHp", damageToPlayer);
		}
		*/
    }
}
