using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGears : MonoBehaviour {

    // A launchable rocket!
    public int projectileLeft;
    public GameObject[] LaunchableProjectile;

    // At most three gears, use a stack to decide who gets projected first.
    public Stack<GameObject> arsenal;

    void Start()
    {
        projectileLeft = 1;
        arsenal = new Stack<GameObject>();

        // Initialize with one random launchable item.
        arsenal.Push(LaunchableProjectile[Random.Range(0, LaunchableProjectile.Length)]);
    }

    // Launch a projectile, can be something fun e.g. food / flip-flops / a small asteroid etc
    public void LaunchProjectile(Transform fireLocation, Quaternion playerFacing){
        if( projectileLeft <= 0 ){
            return;
        }

        projectileLeft--;

        // Launch the projectile by poping out the last item on the stack.
        GameObject launchedProjectile = Instantiate(arsenal.Pop(), fireLocation.position, playerFacing) as GameObject;
        launchedProjectile.gameObject.GetComponent<Rigidbody2D>().AddForce(300 * transform.up );
        Destroy(launchedProjectile.gameObject, 5f);
    }


}
