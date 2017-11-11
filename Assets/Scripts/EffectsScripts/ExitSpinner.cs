using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitSpinner : MonoBehaviour
{

    private Activatable activator;

    private void Start()
    {
        activator = this.gameObject.GetComponent<Activatable>();
    }

    void FixedUpdate(){

        if (activator.active)
        this.transform.Rotate(new Vector3(0,0,-1));
    }


}
