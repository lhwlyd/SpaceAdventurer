using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activatable : MonoBehaviour {


    public bool active = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if( collision.name == "ActiveRange"){
			this.active = true;
		}
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name == "ActiveRange")
		{
			this.active = true;
		}    
    }

    public IEnumerator SetActive( bool active ){
        this.active = active;
        //yield return new WaitForSeconds(3);
        //this.active = false;
        yield return null;
    }
}

