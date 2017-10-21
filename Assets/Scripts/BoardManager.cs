using System.Collections;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;
using UnityEngine;

/**
 * This is the board manager. Used to 1. make a map 2. distribute elements to the map
 * 3. put player and exit on it.
 * 
 */
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
	public GameObject outerWallTile;

	public int centerX;
	public int centerY;

	//List of locations we want to connect with roads
	private List<Coord> importantLocations; 

	//Make all generated objects son to this, so we can clean them more easily when generate next level
	private Transform boardHolder;

    private Transform foodHolder;

	private List <Coord> gridPositions = new List<Coord> ();

	private int[,] map;

    // Vars for putting exit away from player's initial position
    private GameObject currExit;

    private int exitRelocationTimes = 0;
    private int maxExitRelocationTimes = 15;
    private float tooCloseThreshold = 10f;


    // Leave more than 25% ground as walkable
    private float walkablePercent = 0.25f;
    private int walkableCount = 0;


	/*
	 * Fill the gridPosition list with all the valid grid positions
	 */
	void InitializeList(){

        /**
         * We use gridPostions to mark which grid is still available or not
         */

		gridPositions.Clear ();

		//Fill the list with the position of each grid we have
		for( int x=1; x<columns-1;x++){
			//Leave 1 grid at the brim to make sure no impossible levels.
			for(int y=1;y<rows-1;y++){
                // if this grid is not a wall, it means this is available.
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

        // Use holders to hold all these stuff, make the workspace less messy.
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


        /** Make sure there's enough place to put all elements we want on the map.
         */
        do
        {
            walkableCount = 0;
            map = mapGenerator.GenerateMap(mapSize, out centerX, out centerY);
            foreach (int i in map)
            {
                if (i == 0)
                {
                    walkableCount += 1;
                }
            }
            Debug.Log("generate map once");
        } while ((float)(walkableCount / mapSize * mapSize) < walkablePercent);


        // Set up the walls and grounds

		// Go 1 extra grid at the brim bc we want to surround the playground with water.
		for( int x=0; x<columns;x++){
			//Leave 1 grid at the brim to make sure no impossible levels.
			for(int y=0;y<rows;y++){
				GameObject toInstantiate;

				if (map [x, y] == 0) {
					toInstantiate = floorTiles [Random.Range (0, floorTiles.Length)];
				} else {
                    toInstantiate = outerWallTile;
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

    /**
     * Put a random obj from the input tile array and put it randomly on an 
     * available grid.
     */
	void LayoutObjectAtRandom( GameObject[] tileArray, int min, int max){

		int objectCount = Random.Range ( min, max+1);
		for(int i=0; i<objectCount;i++){
			Coord randomPosition = RandomPosition ();
   			GameObject tileChosen = tileArray [Random.Range (0, tileArray.Length)];

            GameObject tileGenerated = Instantiate (tileChosen, new Vector3(randomPosition.tileX,randomPosition.tileY, 0f), Quaternion.identity) as GameObject;

            // For the later on check to make sure exit is generated far from the player.
            if( tileArray[0].Equals(exit[0]) ){
                // Also update the exit location
                currExit = tileGenerated;
            }

            if (tileArray == foodTiles){
                tileGenerated.transform.SetParent(foodHolder);
            }


		}

	}

    public void SetUpScene(int level, int mapSize)
    {

        mapGenerator = GetComponentInParent<MapGenerator>();

        BoardSetup(mapSize);

        InitializeList();

        LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
        LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);

        int enemyCount = 2 * level;

        LayoutObjectAtRandom(enemyTiles, enemyCount, (int)(1.5f * enemyCount));


        /*
        // Make sure there's only one player
        GameObject tempPlayer = GameObject.FindWithTag("Player");
        if ( tempPlayer != null){
            Destroy(tempPlayer);
        }
        */

        Instantiate(player, new Vector3(centerX, centerY, 0f), Quaternion.identity);

        LayoutObjectAtRandom(exit, 1, 1);


        /** I feel like this is over complicated. Maybe should just write a layout exit method to make
         * things more convienient. But I feel like this can be reused sometime later if we want to
         * make tweaks to the exit or make multiple types of exits. This is more generic but may cause
         * unecessary problems and performance issues.
         */

        exitRelocationTimes = 0;
        // Make sure the exit is far away from the player so that player won't skip some level unwantedly.
        while (CheckDistanceBetween(currExit.transform, player.transform) < tooCloseThreshold 
               && exitRelocationTimes < maxExitRelocationTimes){
            exitRelocationTimes++;
            Destroy(currExit);
			LayoutObjectAtRandom(exit, 1, 1);
		}

        // If it's still very close to the player, it means this map is probably too small or 
        // weird looking. Generate a new one instead.
        if (CheckDistanceBetween(currExit.transform, player.transform) < tooCloseThreshold){
            Destroy(this.player.gameObject);
            Destroy(this.currExit);
            this.SetUpScene(level, mapSize);
        }

		// add the exit to radar's detectable tile set.
		ppcRadar = GameObject.Find("PlayerPositionController").GetComponent<Radar>();
        ppcRadar.AddToTrackedObjects(currExit);

	}

    // Helper method
    private float CheckDistanceBetween( Transform t1, Transform t2){
        return Mathf.Log(Mathf.Pow((t1.position.x - t2.transform.position.x), 2) +
                         Mathf.Pow((t1.position.y - t2.transform.position.y), 2), 2);
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
