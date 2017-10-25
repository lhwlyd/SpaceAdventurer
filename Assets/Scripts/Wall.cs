using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{

    public int playerDamage = 1;

    private int wallHp = 3;

    public void TakeDamage()
    {
        wallHp -= playerDamage;
        if (wallHp <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
		if (collision.gameObject.GetComponent<Interactive>() != null)
		{
			collision.gameObject.SendMessage("Interact", this.gameObject);
		}
    }

}
