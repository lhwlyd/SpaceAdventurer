using System.Collections;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;
using UnityEngine;

public class BoardManager : MonoBehaviour {

	public GameManager gameManager;
	public MapGenerator mapGenerator;

    public Radar ppcRadar; // player position controller's radar

	[Serializable]
	public class Count{

		public int maximum;
		public int minimum;

		public Count (int min, int max){

			minimum = min;
			maximum = max;
		}
	}
		
	public int columns;
	public int rows;
	public Count foodCount;
	public Count wallCount = new Count(10,15);
	public GameObject[] exit;
	public GameObject player;
	public GameObject[] floorTiles;
	public GameObject[] foodTiles;
    public GameObject[] enemyTiles;
	public GameObject[] wallTiles;
	public GameObject[] roadTiles;
	public GameObject waterTile;

	public int centerX;
	public int centerY;

	//List of locations we want to connect with roads
	private List<Coord> importantLocations; 

	//Make all generated objects son to this, so we can clean them more easily when generate next level
	private Transform boardHolder;

    private Transform foodHolder;

	private List <Coord> gridPositions = new List<Coord> ();

	private int[,] map;



	/*
	 * Fill the gridPosition list with all the valid grid positions
	 */
	void InitializeList(){

		gridPositions.Clear ();

		//Fill the list with the position of each grid we have
		for( int x=1; x<columns-1;x++){
			//Leave 1 grid at the brim to make sure no impossible levels.
			for(int y=1;y<rows-1;y++){
				if(map[x,y] == 0)
					gridPositions.Add(new Coord(x,y));
			}
		}

		gridPositions.Remove(new Coord(centerX, centerY));
			
	}

	/*
	 * Set up the board with random floor tiles and surround it by water
	 */
	void BoardSetup(int mapSize){
		columns = mapSize;
		rows = mapSize;

        foodCount = new Count ((int)(mapSize/2), (int)(mapSize/2) + 5);

        if (boardHolder == null)
        {
            boardHolder = new GameObject("Board").transform;
        }
        else
        {
            Destroy(boardHolder.gameObject);
            boardHolder = new GameObject("Board").transform;
        }

        if (foodHolder == null)
		{
			foodHolder = new GameObject("Foods").transform;
		}
		else
		{
			Destroy(foodHolder.gameObject);
			foodHolder = new GameObject("Foods").transform;
		}

		map = mapGenerator.GenerateMap (mapSize, out centerX, out centerY);


		//Go 1 extra grid at the brim bc we want to surround the playground with water.
		for( int x=0; x<columns;x++){
			//Leave 1 grid at the brim to make sure no impossible levels.
			for(int y=0;y<rows;y++){
				GameObject toInstantiate;

				if (map [x, y] == 0) {
					toInstantiate = floorTiles [Random.Range (0, floorTiles.Length)];
				} else {
					toInstantiate = waterTile;
				}

				GameObject newInstance = Instantiate (toInstantiate, new Vector3(x,y,0F), Quaternion.identity) as GameObject;	

				newInstance.transform.SetParent (boardHolder);
			}
		}


	}

	/*
	 * Select a random position among all valid ones in the list
	 */
	Coord RandomPosition(){

		int randomIndex = Random.Range (0, gridPositions.Count);
		Coord randomPosition = gridPositions [randomIndex];
		gridPositions.RemoveAt(randomIndex);

		return randomPosition;
	}

	void LayoutObjectAtRandom( GameObject[] tileArray, int min, int max){

		int objectCount = Random.Range ( min, max+1);
		for(int i=0; i<objectCount;i++){
			Coord randomPosition = RandomPosition ();
   			GameObject tileChosen = tileArray [Random.Range (0, tileArray.Length)];

            GameObject tileGenerated = Instantiate (tileChosen, new Vector3(randomPosition.tileX,randomPosition.tileY, 0f), Quaternion.identity) as GameObject;

            // add the exit to radar's detectable tile set.
            if( tileArray[0].Equals(exit[0]) ){
				ppcRadar = GameObject.Find("PlayerPositionController").GetComponent<Radar>();

                ppcRadar.AddToTrackedObjects(tileGenerated);

            }

            if (tileArray == foodTiles){
                tileGenerated.transform.SetParent(foodHolder);
            }

			

		}

	}

	public void SetUpScene(int level, int mapSize){
    	
		mapGenerator = GetComponentInParent<MapGenerator> ();
		
		BoardSetup (mapSize);

		InitializeList ();

		LayoutObjectAtRandom (wallTiles, wallCount.minimum, wallCount.maximum);
		LayoutObjectAtRandom (foodTiles, foodCount.minimum, foodCount.maximum);

        int enemyCount = 2 * level;

        LayoutObjectAtRandom (enemyTiles, enemyCount, (int)(1.5f *enemyCount));

        Instantiate(player, new Vector3(centerX, centerY, 0f), Quaternion.identity);

		LayoutObjectAtRandom(exit, 1, 1);

	}

	//Utility
	struct Coord{

		public int tileX;
		public int tileY;

		public Coord(int x, int y){
			tileX = x;
			tileY = y;
		}
	}

}
