using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurting : Interactive {


	public int damageToPlayer;

	void Interact(GameObject inputGameobject)
	{
		string thisTag = inputGameobject.tag;

        if (thisTag == "Player")
        {
            inputGameobject.SendMessage("LoseHp", damageToPlayer);
        }
	}
}
