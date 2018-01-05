using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISpinner : MonoBehaviour {

    private void FixedUpdate()
    {
        this.transform.Rotate(0,0,-1);
    }
}
