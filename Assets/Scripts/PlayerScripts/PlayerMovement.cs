using UnityEngine;
using System.Collections;


/**
 * Separate the movement part from the player script to make things more clean
 * and easier to manage.
 * 
 * Not in use for now. It doens't work simply by calling a method from the
 * player script. Maybe corountine? Later. I feel like it's unnecessary
 * to separate them for now.
 */
public class PlayerMovement: MonoBehaviour
{

	private Animator animator;
	public Rigidbody2D rb2d;

    public Player player;
	public PlayerGears playerGearManager;



	private void Start()
    {
		animator = GetComponent<Animator>();
		rb2d = this.GetComponent<Rigidbody2D>();
        player = this.GetComponent<Player>();
		playerGearManager = this.GetComponent<PlayerGears>();


	}

    public void MoveThePlayer(){

    }



}
