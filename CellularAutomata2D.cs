using UnityEngine;
using System.Collections;

public class CellularAutomata2D : MonoBehaviour {

	public int MapSize;				//Size in pixels of CellMap
	//public byte Rule;				//This will be the 512 rule id, need way to store and convert 512-bit numbers
	public Color[] WorldColors;		//Colors to use
	public Texture2D CellMap;		//Texture of CellBinaryMap
	int[] CellBinaryMap;			//Holds current cell states
	int[] BitArray;					//512 bit rule
	public int Iterations;			//Iteration counter

	public int SeedType;			//0=Center, 1=Circle, 2=Square

	float Timer;
	public bool Paused;
	public bool Wrap;				//Wrap edges or not

	// Use this for initialization
	void Start () {

		BitArray = new int[512];

		//Initialize MapSize and set texture
		CellBinaryMap = new int[MapSize*MapSize];
		CellMap = new Texture2D(MapSize, MapSize);
		CellMap.filterMode = FilterMode.Point;
		CellMap.wrapMode = TextureWrapMode.Clamp;
		gameObject.GetComponent<Renderer>().material.mainTexture = CellMap;

		InitializeCellMap();
		InitializeCellBinaryMap();
		CreateRandomRule();
		GenerateCellMap();
	}
	
	// Update is called once per frame
	void Update () {
		Timer += Time.deltaTime;

		if (Paused == false) {
			if (Timer > 0.15f) {
				GenerateCellMap ();
				Iterations++;
				Timer = 0.0f;
			}
		}

		if (Input.GetKeyUp (KeyCode.P)) {
			Paused = Paused != true ? true : false;

			Timer = 0.0f;
		}

		if (Paused == true) {
			if (Input.GetKeyUp (KeyCode.Space)) {
				GenerateCellMap ();
				Iterations++;
			}
		}
			
		if (Input.GetKeyUp (KeyCode.R)) {
			CreateRandomRule ();					//Create new rule
			InitializeCellMap ();					//Set Cell Map to white
			InitializeCellBinaryMap ();				//Create center point
			GenerateCellMap ();						//Create Cell Map based on Cell Binary Map
			Iterations = 0;
			Timer = 0.0f;
		}

		if (Input.GetKeyUp (KeyCode.Q)) {
			//Don't change rule, seed only, regenerate
			InitializeCellMap ();					//Set Cell Map to white
			InitializeCellBinaryMap ();				//Create center point
			GenerateCellMap ();						//Create Cell Map based on Cell Binary Map
			Iterations = 0;
			Timer = 0.0f;
		}

		if(Input.GetKeyUp(KeyCode.S)){
			SeedType++;
			if (SeedType > 2) {
				SeedType = 0;
			}

			//Don't change rule, seed only, regenerate
			InitializeCellMap ();					//Set Cell Map to white
			InitializeCellBinaryMap ();				//Create center point
			GenerateCellMap ();						//Create Cell Map based on Cell Binary Map
			Iterations = 0;
			Timer = 0.0f;
		}

		if (Input.GetKeyUp (KeyCode.W)) {
			Wrap = Wrap != true ? true : false;
		}
	}

	public void InitializeCellMap(){

		Color[] ColorMap = new Color[MapSize * MapSize];

		for (int x = 0; x < MapSize; x++) {
			for (int y = 0; y < MapSize; y++) {
				ColorMap [y * MapSize + x] = Color.white;
			}
		}

		CellMap.SetPixels(ColorMap);
		CellMap.Apply();
	}

	public void InitializeCellBinaryMap (){

		for (int x = 0; x < MapSize; x++) {
			for (int y = 0; y < MapSize; y++) {
				CellBinaryMap[y * MapSize + x] = 0;
			}
		}
			
		int cx = MapSize / 2;
		int cy = MapSize / 2;

		//Center Point
		if (SeedType == 0) {
//			for (int x = 0; x < MapSize; x++) {
//				for (int y = 0; y < MapSize; y++) {
//					CellBinaryMap[y * MapSize + x] = Random.Range(0,2);
//				}
//			}
			CellBinaryMap[cy * MapSize + cx] = 1;
		}

		//Circle
		if (SeedType == 1) {
			int r = 16;
			for (int x = cx-16; x < cx+16; x++) {
				for (int y = cy-16; y < cy+16; y++) {
					if (Mathf.Sqrt (Mathf.Pow(x-cx,2) + Mathf.Pow(y-cy,2)) <= r) {
						CellBinaryMap [y * MapSize + x] = 1;
					} 
					else {
						CellBinaryMap[y * MapSize + x] = 0;
					}
				}
			}
		}

		//Square
		if (SeedType == 2) {
			for (int x = cx-16; x < cx+16; x++) {
				CellBinaryMap [(cy-16) * MapSize + x] = 1;
				CellBinaryMap [(cy+16) * MapSize + x] = 1;
			}

			for (int y = cy-16; y < cy+16; y++) {
				CellBinaryMap [y * MapSize + (cx-16)] = 1;
				CellBinaryMap [y * MapSize + (cx+16)] = 1;
			}
		}

	}

	public void CreateRandomRule(){
		// string rule = "";
		for (int i = 0; i < 512; i++) {
			BitArray [i] = Random.Range (0, 2);
		//	rule = string.Concat (rule, BitArray [i].ToString ());
		}

		//Debug.Log (rule);
	}

	public void GenerateCellMap(){

		Color[] ColorMap = new Color[MapSize * MapSize];

		if (Wrap == false) {
			for(int y = 1; y < MapSize-1; y++){
				for(int x = 1; x < MapSize-1; x++){
					int ty = MapSize-y-1;		//Texture pixel array has inverted y, this flips it 

					int Sample = 0;

					//Get neighbors
					int[] Neighbors = new int[9];

					//Get upper left
					Neighbors[0] = CellBinaryMap[(y-1) * MapSize + (x-1)];
					//Get upper middle
					Neighbors[1] = CellBinaryMap[(y-1) * MapSize + (x)];
					//Get upper right
					Neighbors[2] = CellBinaryMap[(y-1) * MapSize + (x+1)];
					//Get left
					Neighbors[3] = CellBinaryMap[(y) * MapSize + (x-1)];
					//Get middle
					Neighbors[4] = CellBinaryMap[(y) * MapSize + (x)];
					//Get right
					Neighbors[5] = CellBinaryMap[(y) * MapSize + (x+1)];
					//Get lower left
					Neighbors[6] = CellBinaryMap[(y+1) * MapSize + (x-1)];
					//Get lower middle
					Neighbors[7] = CellBinaryMap[(y+1) * MapSize + (x)];
					//Get lower right
					Neighbors[8] = CellBinaryMap[(y+1) * MapSize + (x+1)];

					Sample = GetValue (Neighbors);

					//int h = (int)(Sample / 32);
					int j = BitArray[Sample];

					ColorMap[ty * MapSize + x] = WorldColors[j];
					CellBinaryMap[y * MapSize + x] = j;
				}
			}
		}

		if (Wrap == true) {
			for(int y = 0; y < MapSize; y++){
				for(int x = 0; x < MapSize; x++){
					int ty = MapSize-y-1;		//Texture pixel array has inverted y, this flips it 
					int dx = 1;
					int dy = 1;
					int Sample = 0;

					//Get neighbors
					int[] Neighbors = new int[9];

					if (x == 0) {
						x = MapSize-1;
						dx = 0;
					}
					if (x == MapSize-1) {
						x = 0;
						dx = 0;
					}
					if (y == 0) {
						y = MapSize-1;
						dy = 0;
					}
					if (y == MapSize-1) {
						y = 0;
						dy = 0;
					}

					//Get upper left
					Neighbors[0] = CellBinaryMap[(y-dy) * MapSize + (x-dx)];
					//Get upper middle
					Neighbors[1] = CellBinaryMap[(y-dy) * MapSize + (x)];
					//Get upper right
					Neighbors[2] = CellBinaryMap[(y-dy) * MapSize + (x+dx)];
					//Get left
					Neighbors[3] = CellBinaryMap[(y) * MapSize + (x-dx)];
					//Get middle
					Neighbors[4] = CellBinaryMap[(y) * MapSize + (x)];
					//Get right
					Neighbors[5] = CellBinaryMap[(y) * MapSize + (x+dx)];
					//Get lower left
					Neighbors[6] = CellBinaryMap[(y+dy) * MapSize + (x-dx)];
					//Get lower middle
					Neighbors[7] = CellBinaryMap[(y+dy) * MapSize + (x)];
					//Get lower right
					Neighbors[8] = CellBinaryMap[(y+dy) * MapSize + (x+dx)];

					Sample = GetValue (Neighbors);

					//int h = (int)(Sample / 32);
					int j = BitArray[Sample];

					ColorMap[ty * MapSize + x] = WorldColors[j];
					CellBinaryMap[y * MapSize + x] = j;
				}
			}
		}


		CellMap.SetPixels(ColorMap);
		CellMap.Apply();
	}

	public int GetValue(int[] n){

		int Sample = 0;

		if (n [0] == 1) {
			Sample += 1;
		}

		if (n [1] == 1) {
			Sample += 2;
		}

		if (n [2] == 1) {
			Sample += 4;
		}

		if (n [3] == 1) {
			Sample += 8;
		}

		if (n [4] == 1) {
			Sample += 16;
		}

		if (n [5] == 1) {
			Sample += 32;
		}

		if (n [6] == 1) {
			Sample += 64;
		}

		if (n [7] == 1) {
			Sample += 128;
		}

		if (n [8] == 1) {
			Sample += 256;
		}

		return Sample;
	}
}
