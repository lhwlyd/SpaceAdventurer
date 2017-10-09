using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MovingObjects {

	public int attackDamage = 1;
	public int healthPerFood = 25;
	public float restartLevelDelay = 2f;

	private Animator animator;
	[HideInInspector]public int hp;

    public Rigidbody2D rb2d;

    //UI control
    public Text foodText;

    public float speed;

    public GameObject rocketFire;

	// Use this for initialization
	protected override void Start () {
		animator = GetComponent<Animator> ();

		hp = GameManager.instance.playerHealth;

		foodText = GameObject.Find ("FoodText").GetComponent<Text>();

		foodText.text = "Torch Light : " + hp;

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

        //Store the current horizontal input in the float moveHorizontal.
        float moveHorizontal = Input.GetAxis("Horizontal");

        //Store the current vertical input in the float moveVertical.
        float moveVertical = Input.GetAxis("Vertical");


        if (moveHorizontal - Mathf.Abs(moveVertical) < Mathf.Epsilon)
        {
            moveHorizontal *= 0.71f;
            moveVertical *= 0.71f;
        }


        //Use the two store floats to create a new Vector2 variable movement.
        Vector2 velocity = speed * new Vector2(moveHorizontal, moveVertical);

        /*
		//Call the AddForce function of our Rigidbody2D rb2d supplying movement multiplied by speed to move our player.
        rb2d.MovePosition(rb2d.position + velocity * Time.deltaTime);
        */



        // Check for Rocket input
        if( Input.GetKeyDown("space") ){
            rb2d.AddForce( 50 * velocity );
            FireRocket();
        } else {
			rb2d.AddForce(velocity);
		}

    }


    /**
     * Create a rocket fire at opposite to the moving direction of the player.
     */
    private void FireRocket(){
        
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

		foodText.text = "-" + loss  + " Torch Light : " + hp;

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
            hp += healthPerFood;

            foodText.text = "+" + healthPerFood + " Torch Light : " + hp;

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

		foodText.text = "Torch Light : " + hp;

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
