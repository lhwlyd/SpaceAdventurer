using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MovingObjects {

	public int attackDamage = 1;
	public float restartLevelDelay = 1.75f;

	private Animator animator;

    public Rigidbody2D rb2d;

    public float speed;

    public GameObject rocketFire;

    public AudioSource audioSource;

    public PlayerHealth healthManager;

    public PlayerGears playerGearManager;

    public bool stopped = false;

    private bool interacted = false;

	// Use this for initialization
	protected override void Start () {
        healthManager = this.GetComponent<PlayerHealth>();

        playerGearManager = this.GetComponent<PlayerGears>();

		animator = GetComponent<Animator> ();

        rb2d = this.GetComponent<Rigidbody2D>();

		base.Start ();
	}

	private void OnDisable(){
        GameManager.instance.playerHealth = healthManager.currentHealth;
	}

    // Update is called once per frame
    void FixedUpdate()
    {
        if ( stopped || GameManager.instance.doingSetup) {
            return;
        }

		// Prevent multiple interactions due to the collider being circle/capsule
		interacted = false;


		float moveHorizontal = Input.GetAxis("Horizontal");

        //Store the current vertical input in the float moveVertical.
        float moveVertical = Input.GetAxis("Vertical");


		if (moveHorizontal < 0f)
		{
            this.animator.SetBool("FacingRight", false);

            // Also change the backpack if it was using left backpack
            transform.Find("LeftBackpackTransform").GetComponent<TrailRenderer>().time = 0.5f;
			transform.Find("RightBackpackTransform").GetComponent<TrailRenderer>().time = 0;

		}
		
        if (moveHorizontal > 0f){
            this.animator.SetBool("FacingRight", true);
			transform.Find("RightBackpackTransform").GetComponent<TrailRenderer>().time = 0.5f;
			transform.Find("LeftBackpackTransform").GetComponent<TrailRenderer>().time = 0;

	    }
		

        // Rotate
        this.rb2d.AddTorque( moveHorizontal * -1f, ForceMode2D.Force);

        // Physical implementation: using torque instead of adjusting angle directly.
        if (moveVertical > 0f)
        {
            this.animator.SetBool("Moving", true);
            this.rb2d.AddForce(this.transform.up * speed);

        } else {
            
            this.animator.SetBool("Moving", false);
        }

		if (Input.GetKeyDown("space") )
		{
			FireRocket();
		}

        if( Input.GetKeyDown("r") ){
            playerGearManager.LaunchProjectile( this.gameObject.transform, this.gameObject.transform.rotation );
        }

	}
    /**
     * Create a rocket fire at opposite to the moving direction of the player.
     */
    private void FireRocket(){
        GameObject fire = Instantiate(rocketFire, new Vector3(this.transform.position.x, this.transform.position.y, 0f), this.transform.rotation) as GameObject;
		rb2d.AddForce(100 * speed * this.transform.up);
		fire.transform.Rotate(0,0,180);
        fire.transform.SetParent(this.transform);
        Destroy(fire, 1f);
    }

	private void Restart(){

		SceneManager.LoadScene ("Main");
	}


    // Use this method to hurt the player
	public void LoseHp( int loss ){

        healthManager.TakeDamage(loss);

		CheckIfGameOver ();
	}

	// Use this method to hurt the player
	public void AddHp(int increment)
	{
        healthManager.AddHp( increment );
	}

    /**
     * Currently checking : exit / food(torch)
     */
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.GetComponent<Interactive>() != null && !interacted)
		{
            interacted = true;
			collision.gameObject.SendMessage("Interact", this.gameObject);
		}


        if (collision.tag == "Exit")
        {
            Invoke("Restart", restartLevelDelay);

			enabled = false;

			MoveToExit(collision.gameObject.transform);


        }

    }

    void MoveToExit(Transform exitTransform ){
        // MoveOverSeconds(this.gameObject, exitTransform.position, restartLevelDelay);
        this.rb2d.velocity = Vector2.zero;
        this.stopped = true;
    }

    /** Helper method to move an object over some seconds.
    * might be useful to immitate gravitational work. Haven't found out how should it work yet. Need to check back later
    */
	public IEnumerator MoveOverSeconds(GameObject objectToMove, Vector3 end, float seconds)
	{
		float elapsedTime = 0;
		Vector3 startingPos = objectToMove.transform.position;
		while (elapsedTime < seconds)
		{
			objectToMove.transform.position = Vector3.Lerp(startingPos, end, (elapsedTime / seconds));
			elapsedTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		objectToMove.transform.position = end;
	}


    /**
     * Always call this every time player's lose hp
     */
	private void CheckIfGameOver(){
        if( healthManager.currentHealth <= 0){
            //this.enabled = false;
            this.gameObject.SetActive(false);
            GameManager.instance.GameOver ();
		}
	}



}
