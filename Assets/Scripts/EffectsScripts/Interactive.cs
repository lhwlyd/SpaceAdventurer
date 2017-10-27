using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactive : MonoBehaviour{

    /* Inherit this for interactive behaviours on some objects in game:
     * e.g. Food can have a healing effect, enemy can have a hurtful effect,
     * explosion can hurt while push you away ... etc
     */
    void Interact(GameObject obj){}
}
