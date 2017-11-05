using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGears : MonoBehaviour {

    // We only want one gear manager. Implement this later together with the 
    // static player.
    //public static PlayerGears instance = null;

	// A launchable rocket!
	public int projectileLeft;
    public GameObject[] launchableProjectiles;
    public GameObject[] projectileUI;

    // At most three gears, use a stack to decide who gets projected first.
    public Stack<GameObject> arsenal;

    private Transform PUITransform; // UI location

    void Awake()
    {
        /*
		if (instance == null)
		{
			instance = this;
		}
		else if (instance != this)
		{
			Destroy(gameObject);
		}

        DontDestroyOnLoad(gameObject);

        */ 
  		arsenal = new Stack<GameObject>();
		PUITransform = GameObject.Find("ProjectilesUI").transform;
		projectileLeft = 0;
        ReloadRandom(2);

	}

    private void ReloadRandom(int amount){

        // Initialize with one random launchable item.
        for (int i = 0; i < amount; i++){
			PickUpProjectile(Random.Range(0, launchableProjectiles.Length));
		}
		
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

        // Also remove the corresponding UI at the bottom left corner.
        Destroy(PUITransform.GetChild(PUITransform.childCount - 1).gameObject);
    }


    public void PickUpProjectile( int projectileNumber ){
        if(projectileLeft >= 3){
            return;
        }

        projectileLeft++;
        arsenal.Push( launchableProjectiles[projectileNumber] );
        GameObject newUI = Instantiate(projectileUI[projectileNumber], new Vector3(0,0,0), Quaternion.identity) as GameObject;

		// Adjust the position to be sitting at the left bottom corner of the main canvas.
		newUI.transform.SetParent(PUITransform.transform);
        newUI.transform.position = new Vector2(PUITransform.position.x + projectileLeft * 50, PUITransform.position.y + 25 );
	}


}
