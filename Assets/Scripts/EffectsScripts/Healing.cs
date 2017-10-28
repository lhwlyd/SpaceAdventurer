using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healing : Interactive {

    public int healAmount;

    void Interact( GameObject inputGameobject ){
		string thisTag = inputGameobject.tag;
		if (thisTag == "Player" )
		{
            inputGameobject.SendMessage( "AddHp", healAmount );
            Destroy(this.gameObject);
		}
    }

}
