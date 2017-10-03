using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	//No more than one game manager
	public static GameManager instance = null;

	public BoardManager boardScript;

	private int level = 1;

	public float turnDelay = 0.1f;

	public int playerHealth = 100;

	[HideInInspector]public int mapSize = 30;

	[HideInInspector]public bool playersTurn = false;

	private List<Enemy> enemies;
	private bool enemiesMoving;

	//UI control
	public float levelStartingDelay = 2f;
	private Text levelText;
	private GameObject levelImage;
	private bool doingSetup;

	public GameObject miniMap;

	// Use this for initialization
	void Awake () {

		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}

		DontDestroyOnLoad (gameObject);

		enemies = new List<Enemy> ();
		boardScript = GetComponent<BoardManager> ();
		InitGame ();
	}

	void InitGame(){
		doingSetup = true;

		levelImage = GameObject.Find ("LevelImage");
		levelText = GameObject.Find ("LevelText").GetComponent<Text>();
		levelText.text = "Night " + level;	
		levelImage.SetActive(true);
		miniMap = GameObject.Find ("MiniMap");
		//miniMap.SetActive (false);
		Invoke ("HideLevelImage", levelStartingDelay);

        enemies.Clear();

        mapSize = level * 5 + 30;

		boardScript.SetUpScene (level, mapSize);
	}

	private void HideLevelImage(){
		levelImage.SetActive (false);
		doingSetup = false;
	}

	public void GameOver(){
		levelText.text = "you died, sucker";
		levelImage.SetActive (true);
		enabled = false;
	}


	void Update(){

		//miniMap.enabled = !miniMap.GetComponent<Renderer>().enabled;

		if(playersTurn || enemiesMoving || doingSetup){
			return;
		}
		StartCoroutine (MoveEnemies ());
	}

	public void AddEnemyToList(Enemy enemy){
		enemies.Add (enemy);
	}

	IEnumerator MoveEnemies(){
		enemiesMoving = true;
		yield return new WaitForSeconds(turnDelay);

		if(enemies.Count == 0){
			yield return new WaitForSeconds(turnDelay);
		}

		for(int i=0; i < enemies.Count; i++){
			enemies [i].MoveEnemy ();
			yield return new WaitForSeconds (enemies[i].moveTime);
		}

		playersTurn = true;
		enemiesMoving = false;
	}


	void OnLevelWasLoaded(int index){
		level++;
		InitGame ();
	}

}
