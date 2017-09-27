using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MovingObjects {

	public int attackDamage = 1;
	public int healthPerFood = 25;
	public float restartLevelDelay = 1f;

	private Animator animator;
	[HideInInspector]public int hp;


	//UI control
	public Text foodText;

	// Use this for initialization
	protected override void Start () {
		animator = GetComponent<Animator> ();

		hp = GameManager.instance.playerHealth;

		foodText = GameObject.Find ("FoodText").GetComponent<Text>();

		foodText.text = "Torch Light : " + hp;

		base.Start ();
	}

	private void OnDisable(){
		GameManager.instance.playerHealth = hp;
	}

	// Update is called once per frame
	void Update () {

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
			AttemptMove<Temp> (horizontal, vertical);
		}

	}

	protected override void OnCantMove<T> (T component){
		
	}

	private void Restart(){

		SceneManager.LoadScene (0);
	}


	public void LoseHp( int loss ){
		hp -= loss;

		foodText.text = "-" + loss  + " Torch Light : " + hp;

		CheckIfGameOver ();
	}

	private void OnTriggerEnter2D (Collider2D other){
		if(other.tag == "Exit"){
			Invoke ("Restart", restartLevelDelay);
			enabled = false;
		} else if ( other.tag == "Torch"){
			hp += healthPerFood;

			foodText.text = "+" + healthPerFood + " Torch Light : " + hp;

			other.gameObject.SetActive (false);
		}

			
	}

	protected override void AttemptMove<T>(int xDir, int yDir){
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

	private void CheckIfGameOver(){
		if( hp <= 0){
			GameManager.instance.GameOver ();
		}
	}
}
