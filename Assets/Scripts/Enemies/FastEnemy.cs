using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * This class represents a fast enemy.
 * This enemy will locate the player then charge at the player in a linear fashion. If this enemy misses the player, this enemy will 
 * re-adjust itself to linearly charge at the player again.
 */
public class FastEnemy : Enemy
{
    public GameObject explosion;
    public bool hasCharged; // Whether or not the enemy has charged
    float xDir;
    float yDir;
    float vSquare;

    private Animator animator;
    private Transform target;

    protected override void Start()
    {
        //Register this enemy with our instance of GameManager by adding it to a list of Enemy objects. 
        //This allows the GameManager to issue movement commands.
        GameManager.instance.AddEnemyToList(this);

        //Get and store a reference to the attached Animator component.
        animator = GetComponent<Animator>();

        //Find the Player GameObject using it's tag and store a reference to its transform component.
        target = GameObject.FindGameObjectWithTag("Player").transform;

        rb2d = GetComponent<Rigidbody2D>();

        circleCollider2d = GetComponent<CircleCollider2D>();
        rb2d = GetComponent<Rigidbody2D>();

        this.hasCharged = true;
    }

    /**
     * Movement logic: Get player's position (Do not keep updating the player's position. Only 
     * reset target once after done charging.)
     * Move towards that position linearly (1 direction/ axis)
     * Stop once at the player's position
     */ 
    protected override void FixedUpdate()
    {
        if (GameManager.instance.doingSetup)
            return;

        if(this.hasCharged)
        {
            // Reset target
            yDir = (target.position.y - this.transform.position.y);
            xDir = (target.position.x - this.transform.position.x);
            vSquare = Mathf.Pow(xDir, 2) + Mathf.Pow(yDir, 2);
            hasCharged = false;
        }

        Vector2 velocity = new Vector2(xDir, yDir);
        // Control the velocity's value to be always speed^2
        if (vSquare > speed * speed)
        {
            velocity *= (Mathf.Pow(speed, 2) / vSquare);

        }
        else
        {
            velocity.Normalize();
            velocity *= speed;
        }

        rb2d.MovePosition(rb2d.position + velocity * Time.deltaTime);

    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            GameObject thisExplosion = Instantiate(explosion, this.gameObject.transform.position, Quaternion.identity) as GameObject;
            Destroy(thisExplosion, 2f);
            Destroy(this.gameObject);
        }
        this.hasCharged = true;
    }

    
}
