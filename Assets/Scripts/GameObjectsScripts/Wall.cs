using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
		if (collision.gameObject.GetComponent<Interactive>() != null)
		{
			collision.gameObject.SendMessage("Interact", this.gameObject);
		}
    }

}
