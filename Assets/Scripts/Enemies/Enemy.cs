using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	public int playerDamage;

	public Animator animator;                          //Variable of type Animator to store a reference to the enemy's Animator component.
	public Transform target;                           //Transform to attempt to move toward each turn.
	//private bool skipMove; Not used

    public Rigidbody2D rb2d;
    public float speed;
    public CircleCollider2D circleCollider2d;


	protected virtual void Start ()
	{
		//Register this enemy with our instance of GameManager by adding it to a list of Enemy objects. 
		//This allows the GameManager to issue movement commands.
		GameManager.instance.AddEnemyToList (this);

		//Get and store a reference to the attached Animator component.
		animator = GetComponent<Animator> ();

		//Find the Player GameObject using it's tag and store a reference to its transform component.
		target = GameObject.FindGameObjectWithTag ("Player").transform;

        rb2d = GetComponent<Rigidbody2D>();

		circleCollider2d = GetComponent<CircleCollider2D>();
        rb2d = GetComponent<Rigidbody2D>();
	}

    // Shouldn't be overriden. If new check conditions are needed, please write new check fucntions
    protected bool CheckInactive(){
        return GameManager.instance.doingSetup || !this.GetComponent<Activatable>().active;
    }


    protected virtual void FixedUpdate()
    {

        if( CheckInactive() ){
            return;
        }
        // Default enemy movement will be to always float towards player.
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

    // Use this
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player"){
            collision.gameObject.SendMessage("LoseHp", playerDamage);

            Destroy(this.gameObject);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision){
        if ( collision.gameObject.GetComponent<Interactive>() != null)
        {
            collision.gameObject.SendMessage("Interact", this.gameObject);
        }
    }

}
