using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObjects {

	public int playerDamage;

	private Animator animator;                          //Variable of type Animator to store a reference to the enemy's Animator component.
	private Transform target;                           //Transform to attempt to move toward each turn.
	private bool skipMove;

    public Rigidbody2D rb2d;
    public float speed;

	protected override void Start ()
	{
		//Register this enemy with our instance of GameManager by adding it to a list of Enemy objects. 
		//This allows the GameManager to issue movement commands.
		GameManager.instance.AddEnemyToList (this);

		//Get and store a reference to the attached Animator component.
		animator = GetComponent<Animator> ();

		//Find the Player GameObject using it's tag and store a reference to its transform component.
		target = GameObject.FindGameObjectWithTag ("Player").transform;

        rb2d = GetComponent<Rigidbody2D>();

		//Call the start function of our base class MovingObject.
		base.Start ();
	}

    protected override void AttemptMove<T>(float xDir, float yDir){
		if(skipMove == true){
			skipMove = false;
			return;
		}

		base.AttemptMove<T> (xDir, yDir);

		skipMove = true;
	}

	public void MoveEnemy(){
		int xDir = 0;
		int yDir = 0;

		if (Mathf.Abs (target.position.x - transform.position.x) < float.Epsilon) {
			yDir = target.position.y > transform.position.y ? 1 : -1;
		} else {
			xDir = target.position.x > transform.position.x ? 1 : -1;
		}

		AttemptMove<Player> (xDir, yDir);


	}

	protected override void OnCantMove<T> (T component)
	{
		Player hitPlayer = component as Player;



		hitPlayer.LoseHp (playerDamage);
	}

	void OnTriggerEnter2D(Collider2D other){
		if ( other.tag == "Torch"){
			other.gameObject.SetActive (false);
		}
	}

    void FixedUpdate()
    {
        float yDir = (target.position.y - this.transform.position.y);
        float xDir = (target.position.x - this.transform.position.x);
        float vSquare = Mathf.Pow(xDir, 2) + Mathf.Pow(yDir, 2);

        Vector2 velocity = new Vector2(xDir, yDir);
        // Control the velocity's value to be always speed^2
        if(vSquare > speed * speed){
            velocity *= (Mathf.Pow(speed, 2) / vSquare);

        } else {
            velocity.Normalize();
            velocity *= speed;
        }

        rb2d.MovePosition(rb2d.position + velocity * Time.deltaTime);

        //AttemptMove<Player>(xDir, yDir);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player"){
            collision.gameObject.SendMessage("LoseHp", playerDamage);

            this.gameObject.SetActive(false);
        }
    }
}
