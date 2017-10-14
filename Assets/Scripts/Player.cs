using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MovingObjects {

	public int attackDamage = 1;
    [HideInInspector] public int healthPerFood = 10;
	public float restartLevelDelay = 1.75f;

	private Animator animator;
	[HideInInspector]public int hp;

    public Rigidbody2D rb2d;

    //UI control
    public Text foodText;

    public float speed;

    public GameObject rocketFire;

    private float inhaleTime;

	// Use this for initialization
	protected override void Start () {
		animator = GetComponent<Animator> ();

		hp = GameManager.instance.playerHealth;

		foodText = GameObject.Find ("FoodText").GetComponent<Text>();

		foodText.text = "Oxygen Left " + hp + " %";

        rb2d = this.GetComponent<Rigidbody2D>();

		base.Start ();
	}

	private void OnDisable(){
		GameManager.instance.playerHealth = hp;
	}

    // Update is called once per frame
    void FixedUpdate()
    {
        /*
		if (!GameManager.instance.playersTurn) {
			return;
		}

		int horizontal = 0;
		int vertical = 0;

		horizontal = (int)Input.GetAxisRaw ("Horizontal");
		vertical = (int)Input.GetAxisRaw ("Vertical");

		if (horizontal != 0)
			vertical = 0;

		if(horizontal!=0 || vertical != 0){
            AttemptMove<Wall> (horizontal, vertical);
		}
		*/


        /**
         *  Deprecate this moving method (inputs add force directly) since it doesn't feel physical
        //Store the current horizontal input in the float moveHorizontal.
        float moveHorizontal = Input.GetAxis("Horizontal");

        //Store the current vertical input in the float moveVertical.
        float moveVertical = Input.GetAxis("Vertical");

        if ( moveHorizontal < 0f ){
            this.GetComponent<SpriteRenderer>().sprite = leftSpaceMan;
        } else {
			if (moveHorizontal > 0f)
			this.GetComponent<SpriteRenderer>().sprite = rightSpaceMan;
		}

        if (moveHorizontal - Mathf.Abs(moveVertical) < Mathf.Epsilon)
        {
            moveHorizontal *= 0.71f;
            moveVertical *= 0.71f;
        }



        //Use the two store floats to create a new Vector2 variable movement.
        Vector2 velocity = speed * new Vector2(moveHorizontal, moveVertical);

        */

        /*
		//Call the AddForce function of our Rigidbody2D rb2d supplying movement multiplied by speed to move our player.
        rb2d.MovePosition(rb2d.position + velocity * Time.deltaTime);





        float angle = Mathf.Atan2(velocity.x, velocity.y) * Mathf.Rad2Deg;
        // Below is a lazy implementation. Not physical
        // this.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -angle));

      


        // Check for Rocket input
        if( Input.GetKeyDown("space") && velocity.magnitude > 0){
			FireRocket(velocity);
			rb2d.AddForce( 30 * velocity );
        } else {
			rb2d.AddForce(velocity);
		}

        */

        inhaleTime += Time.fixedDeltaTime;
        if( inhaleTime >= 3f ){
            inhaleTime = 0;
            hp -= 1;
			foodText.text = " Oxygen Left : " + hp + " %";

		}


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

	}
    /**
     * Create a rocket fire at opposite to the moving direction of the player.
     */
    private void FireRocket(){
        this.LoseHp(1);

        GameObject fire = Instantiate(rocketFire, new Vector3(this.transform.position.x, this.transform.position.y, 0f), this.transform.rotation) as GameObject;
		rb2d.AddForce(100 * speed * this.transform.up);
		fire.transform.Rotate(0,0,180);
        fire.transform.SetParent(this.transform);
        Destroy(fire, 1f);
    }

	protected override void OnCantMove<T> (T component){
        Wall hitWall = component as Wall;

        hitWall.TakeDamage();
	}

	private void Restart(){

		SceneManager.LoadScene (0);
	}


    // Use this method to hurt the player
	public void LoseHp( int loss ){
		hp -= loss;

		foodText.text = "-" + loss  + " Oxygen Left : " + hp + " %";

		CheckIfGameOver ();
	}

    /**
     * Currently checking : exit / food(torch)
     */
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Exit")
        {
            Invoke("Restart", restartLevelDelay);

			enabled = false;

			MoveToExit(other.gameObject.transform);


        }
        else if (other.tag == "Torch")
        {
            int healthAdded = healthPerFood;
            if (hp >= 90)
            {
                hp += (int)(healthPerFood / 4);
                healthAdded = (int)(healthPerFood / 4);
            }
            else
            {
                if (hp >= 50)
                {
                    healthPerFood += (int)(healthPerFood / 2);
					healthAdded = (int)(healthPerFood / 2);

				} else {
					hp += healthPerFood;
				}
            } 

            foodText.text = "+" + healthAdded + " Oxygen Left : " + hp + " %";

			other.gameObject.SetActive(false);
        }


    }

    void MoveToExit(Transform exitTransform ){
        // MoveOverSeconds(this.gameObject, exitTransform.position, restartLevelDelay);
        this.rb2d.velocity = Vector2.zero;
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

    protected override void AttemptMove<T>(float xDir, float yDir){
		hp--;

        foodText.text = "Oxygen Left : " + hp + " %";

		base.AttemptMove<T> (xDir, yDir);

		RaycastHit2D hit;

		// move returns true when player can successfully move into the new place
		if(Move(xDir, yDir, out hit)){
			//play sound
		}

		CheckIfGameOver ();

		GameManager.instance.playersTurn = false;
	}

    /**
     * Always call this every time player's attacked / loss hp
     */
	private void CheckIfGameOver(){
		if( hp <= 0){
			GameManager.instance.GameOver ();
		}
	}
}
