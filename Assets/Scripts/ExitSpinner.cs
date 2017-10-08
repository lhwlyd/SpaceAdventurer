using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitSpinner : MonoBehaviour
{
    
    void FixedUpdate(){
        this.transform.Rotate(new Vector3(0,0,-1));
    }


}
