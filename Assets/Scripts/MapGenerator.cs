using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System;


public class MapGenerator : MonoBehaviour{

	private int width;	
	private int height;

	public string seed = "";
	public int smoothLevel = 5;

	private bool useRandomSeed;

	private Room mainRoom;

	[Range(0,100)]
	public int randomFillPercent = 45;

	static int[,] map ;

	public int[,] GenerateMap(int mapSize, out int centerX, out int centerY){
		width = mapSize;
		height = mapSize;
		map = new int[width, height];

		if(seed == "" ){
			useRandomSeed = true;
		}

		RandomFillMap ();

		for(int i=0; i<smoothLevel; i++){
			smoothMap ();
		}

		Coord center = ProcessMap ();
		centerX = center.tileX;
		centerY = center.tileY;

		return map;
	}



	void smoothMap(){
		for(int x=1;x<width;x++){
			for(int y=1; y<height; y++){
				int neighborCount = GetSurroundingWallsCount (x, y);

				if (neighborCount > 4) {
					map [x, y] = 1; 
				} else if (neighborCount < 4){
					map [x, y] = 0;
				}
			}
		}
	}

	int GetSurroundingWallsCount( int gridX, int gridY ){
		int wallCount = 0;
		for(int neighborX = gridX - 1; neighborX <= gridX + 1; neighborX ++){
			for (int neighborY = gridY - 1; neighborY <= gridY + 1; neighborY++) {
				if (IsInMapRange(neighborX, neighborY)) {
					if (neighborX != gridX || neighborY != gridY) {
					
						wallCount += map [neighborX, neighborY];
					}
				}else {
					wallCount++;
				}
			} 
		}

		return wallCount;
	}


	void RandomFillMap(){
		//Based on seed
		if(useRandomSeed){
			seed = Time.time.ToString ();
		}

		System.Random psuedoRandom = new System.Random (seed.GetHashCode());

		for(int x = 0; x<width; x++){
			for(int y=0; y<height; y++){
				if (x == 0 || x == width - 1 || y == 0 || y == height - 1) {
					map [x, y] = 1;
				} else {
					map [x, y] = (psuedoRandom.Next (0,100) < randomFillPercent) ? 1 : 0;
				}
			}
		}

	}
		
	struct Coord{

		public int tileX;
		public int tileY;

		public Coord(int x, int y){
			tileX = x;
			tileY = y;
		}
	}


	List<Coord> GetRegionTiles(int startX, int startY){
		List<Coord> tiles = new List<Coord> ();

		int[,] mapFlags = new int[width, height];

		int tileType = map [startX, startY];

		Queue<Coord> queue = new Queue<Coord> ();
		queue.Enqueue (new Coord(startX,startY));
		mapFlags [startX, startY] = 1; //already examined this tile

		while(queue.Count > 0){
			Coord currentTile = queue.Dequeue ();
			tiles.Add (currentTile);

			//Add adjacent tiles
			for(int x=currentTile.tileX - 1; x <= currentTile.tileX + 1; x++){
				for(int y=currentTile.tileY - 1; y <= currentTile.tileY + 1; y++){
					if( IsInMapRange(x,y) && ( x == currentTile.tileX || y == currentTile.tileY ) ){
						if ( mapFlags[x,y] == 0 && map [x, y] == tileType) {
							mapFlags [x, y] = 1;
							queue.Enqueue (new Coord (x, y));
						}
					}
				}
			}
		}

		return tiles;
	}

	List<List<Coord>> GetRegions( int tileType ){
		List<List<Coord>> regions = new List<List<Coord>> ();

		int[,] mapFlags = new int[width, height];

		for (int x = 1; x < width; x++) {
			for (int y = 1; y < height; y++) {
				if( mapFlags[x,y] == 0 && map[x,y] == tileType){
					List<Coord> newRegion = GetRegionTiles (x, y);
					regions.Add (newRegion);
					foreach(Coord tile in newRegion){
						mapFlags [tile.tileX, tile.tileY] = 1;
					}
				}
			}
		}

		return regions;
	}

	Coord ProcessMap(){
		List<List<Coord>> wallRegions = GetRegions (1);

		int wallThreshHold = 30;

		foreach(List<Coord> region in wallRegions){
			if(region.Count < wallThreshHold){
				foreach( Coord tile in region){
					map [tile.tileX, tile.tileY] = 0;
				}
			}
		}

		int roomThreshHold = 50;
		List<List<Coord>> roomRegions = GetRegions (0);
		List<Room> survivingRooms = new List<Room> ();

		foreach(List<Coord> region in roomRegions){
			if (region.Count < roomThreshHold) {
				foreach (Coord tile in region) {
					map [tile.tileX, tile.tileY] = 1;
				}
			} else {
				survivingRooms.Add (new Room(region, map));
			}
		}

		survivingRooms.Sort ();
		survivingRooms [0].isMainRoom = true;
		survivingRooms [0].isAccessibleFromMainRoom = true;

		ConnectClosestRooms (survivingRooms);

		Coord center = CalculateCenterOfMainRoom (survivingRooms[0]);

		return center;
	
	}

	/*
	 * Find out the center tile of the main room
	 */
	Coord CalculateCenterOfMainRoom(Room mainRoom){
		int eastX = -1;
		int westX = -1;

		int northY = -1;
		int southY = -1;

		//Find the center tile's X value
		foreach(Coord tile in mainRoom.tiles){
			if( tile.tileX > eastX || eastX == -1){
				eastX = tile.tileX;
			}

			if(tile.tileX < westX || westX == -1){
				westX = tile.tileX;
			}

			if(tile.tileY > northY || northY == -1){
				northY = tile.tileY;
			}

			if(tile.tileY < southY || southY == -1){
				southY = tile.tileY;
			}
		}

		int midX = (eastX + westX) / 2;


		//Then find the tile using the x value
		foreach(Coord tile in mainRoom.tiles){
			if( tile.tileX == midX ){
				if(tile.tileY < southY || southY == -1){
					southY = tile.tileY;
				}

				if(tile.tileY > northY || northY == -1){
					northY = tile.tileY;
				}
					
			}
		}

		int midY = (southY + northY) / 2;

		int bestY = -1;
		foreach(Coord tile in mainRoom.tiles){
			if( tile.tileX == midX ){
				if( Mathf.Pow(tile.tileY - midY,2) < bestY || bestY == -1){
					bestY = tile.tileY;
				}

			}
		}

		return new Coord (midX,bestY);
	}

	bool IsInMapRange(int x, int y){
		return x > 0 && x < width && y > 0 && y < height;
	}



	//Connecting rooms

	class Room : IComparable<Room>{
		public List<Coord> tiles;
		public List<Coord> edgeTiles;
		public List<Room> connectedRooms;
		public int roomSize;
		public bool isAccessibleFromMainRoom;
		public bool isMainRoom;

		public Room(){
		}

		public Room(List <Coord> roomTiles, int[,] map){
			tiles = roomTiles;
			roomSize = tiles.Count;
			connectedRooms = new List<Room>();
			edgeTiles = new List<Coord>();

			foreach(Coord tile in tiles){
				for (int x = tile.tileX-1; x <= tile.tileX+1; x++) {
					for (int y = tile.tileY-1; y <= tile.tileY+1; y++) {
						if (x == tile.tileX || y == tile.tileY) {
							if (map[x,y] == 1) {
								edgeTiles.Add(tile);
							}
						}
					}
				}
			}

		}

		public static void ConnectRooms(Room roomA, Room roomB) {
			roomA.connectedRooms.Add (roomB);
			roomB.connectedRooms.Add (roomA);

			if(roomA.isAccessibleFromMainRoom){
				roomB.SetAccessibleFromMainRoom ();
			} else if( roomB.isAccessibleFromMainRoom){
				roomA.SetAccessibleFromMainRoom ();
			}
				
		}

		public bool IsConnected(Room otherRoom) {
			return connectedRooms.Contains(otherRoom);
		}

		public int CompareTo(Room otherRoom){
			return otherRoom.roomSize.CompareTo (roomSize);
		}

		public void SetAccessibleFromMainRoom(){
			if(!isAccessibleFromMainRoom){
				isAccessibleFromMainRoom = true;
				foreach(Room r in connectedRooms){
					r.SetAccessibleFromMainRoom ();
				}
			}
		}
	}

	void ConnectClosestRooms(List<Room> allRooms, bool forceAccessibilityFromMainRoom = false){

		List<Room> roomListA = new List<Room> (); //Rooms that are not accessible from the main room
		List<Room> roomListB = new List<Room> (); //Rooms that are accessible from the main room

		if (forceAccessibilityFromMainRoom) {
			foreach (Room r in allRooms) {
				if (r.isAccessibleFromMainRoom) {
					roomListB.Add (r);
				} else {
					roomListA.Add (r);
				}
			}
		} else {
			roomListA = allRooms;
			roomListB = allRooms;
		}

		int bestDistance = 0;
		Coord bestTileA = new Coord ();
		Coord bestTileB = new Coord ();
		Room bestRoomA = new Room ();
		Room bestRoomB = new Room ();
		bool possibleConnectionFound = false;

		foreach( Room roomA in roomListA){
			if (!forceAccessibilityFromMainRoom) {
				possibleConnectionFound = false;

				if(roomA.connectedRooms.Count > 0){
					continue; 
				}
			} 

			foreach( Room roomB in roomListB){
				if(roomA == roomB || roomA.IsConnected(roomB)){
					continue;
				}
					
				for( int tileIndexA = 0; tileIndexA < roomA.edgeTiles.Count; tileIndexA++){
					for( int tileIndexB = 0; tileIndexB < roomB.edgeTiles.Count; tileIndexB++){
						Coord tileA = roomA.edgeTiles[tileIndexA];
						Coord tileB = roomB.edgeTiles [tileIndexB];

						int distanceBetweenRooms = (int)(Mathf.Pow (tileA.tileX - tileB.tileX, 2) +
							Mathf.Pow (tileA.tileY - tileB.tileY, 2));

						if(distanceBetweenRooms < bestDistance || possibleConnectionFound == false){
							bestDistance = distanceBetweenRooms;
							possibleConnectionFound = true;
							bestTileA = tileA;
							bestTileB = tileB;
							bestRoomA = roomA;
							bestRoomB = roomB;
						}
					}
				}
			}

			if(possibleConnectionFound && !forceAccessibilityFromMainRoom){
				CreatePassage (bestRoomA, bestRoomB, bestTileA, bestTileB);
			}
		}

		if(possibleConnectionFound && forceAccessibilityFromMainRoom){
			CreatePassage (bestRoomA, bestRoomB, bestTileA, bestTileB);
			ConnectClosestRooms (allRooms, true);
		}

		if(!forceAccessibilityFromMainRoom){
			ConnectClosestRooms (allRooms, true);
		}
	}

	void CreatePassage( Room roomA, Room roomB, Coord tileA, Coord tileB){
		Room.ConnectRooms (roomA, roomB);

		Debug.DrawLine (new Vector3(tileA.tileX, tileA.tileY, 0f), new Vector3(tileB.tileX, tileB.tileY, 0f), Color.red, 3);

		List<Coord> line = GetLine (tileA,tileB);
		foreach(Coord c in line){
			DrawCircle (c, 1);
		}

	}

	void DrawCircle(Coord c, int r){
		for(int x = -r; x<= r; x++){
			for(int y = -r; y<= r; y++){
				if(x*x + y*y <= r*r){
					int drawX = c.tileX + x;
					int drawY = c.tileY + y;
					if(IsInMapRange(drawX, drawY)){
						map [drawX, drawY] = 0;
					}
				}
			}
		}
	}

	List<Coord> GetLine(Coord from, Coord end){
		List<Coord> line = new List<Coord> ();

		int x = from.tileX;
		int y = from.tileY;

		int dx = end.tileX - from.tileX;
		int dy = end.tileY - from.tileY;

		bool inverted = false;
		int step = Math.Sign (dx);
		int gradientStep = Math.Sign (dy);

		int longest = Mathf.Abs (dx);
		int shortest = Mathf.Abs (dy);

		if(longest < shortest){
			inverted = true;
			longest = Mathf.Abs (dy);
			shortest = Mathf.Abs (dx);

			step = Math.Sign (dy);
			gradientStep = Math.Sign (dx);
		}

		int gradientAccumulation = longest / 2;
		for(int i=0; i<longest; i++){
			line.Add (new Coord(x,y));

			if (inverted) {
				y += step;
			} else {
				x += step;
			}

			gradientAccumulation += shortest;
			if(gradientAccumulation >= longest ){
				if (inverted) {
					x += gradientStep;
				} else {
					y += gradientStep;
				}

				gradientAccumulation -= longest;
			}
		}

		return line;
	}
}
