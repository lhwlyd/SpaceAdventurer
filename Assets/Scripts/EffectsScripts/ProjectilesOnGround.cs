using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilesOnGround : Interactive {

    public int projectileTypeNumber;

	void Interact(GameObject inputGameobject)
	{
		string thisTag = inputGameobject.tag;
		if (thisTag == "Player")
		{
            // I minus 1 to the type numer here so in the inspector use the type number starting from 1, not 0
            inputGameobject.gameObject.GetComponent<PlayerGears>().PickUpProjectile(projectileTypeNumber - 1);
			Destroy(this.gameObject);
		}
	}
}
