using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activatable : MonoBehaviour {


    public bool active = false;
    private bool doingActive = false;

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
        if (doingActive)
        {
            yield return null;
        }
        else {
            doingActive = true;
			yield return new WaitForSeconds(5);
			this.active = false;
            doingActive = false;
			yield return null;
        }

    }
}

