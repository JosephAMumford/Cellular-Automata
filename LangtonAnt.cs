using UnityEngine;
using System.Collections;

public class LangtonAnt : MonoBehaviour {

	public int TextureWidth;
	public int TextureHeight;
	public Texture2D AntTexture;
	public Texture2D ActivityTexture;
	public Color[] ColorSelection;
	public Color[] ActivityColorRange;

	Color[] ColorMap;
	float[] ActivityMap;
	int[] CellMap;
	public float ActivityIncrement;

	public int AntDirection;
	public int AntPositionX;
	public int AntPositionY;

	public float Timer;
	public float TimeStep;
	public int StepCounter;
	public bool Paused;

	public int MapId;

	// Use this for initialization
	void Start () {
	
		//Texture Setup
		AntTexture = new Texture2D(TextureWidth, TextureHeight);
		AntTexture.filterMode = FilterMode.Point;
		AntTexture.wrapMode = TextureWrapMode.Clamp;
		ColorMap = new Color[TextureWidth * TextureHeight];
		CellMap = new int[TextureWidth * TextureHeight];

		ActivityTexture = new Texture2D (TextureWidth, TextureHeight);
		ActivityTexture.filterMode = FilterMode.Bilinear;
		ActivityTexture.wrapMode = TextureWrapMode.Clamp;
		ActivityMap = new float[TextureWidth * TextureHeight];

		MapId = 0;
		SetTextureMap ();

		FillTextureMap (0);

		//Random Initial Setup
		//CreateCircle(16);
		//CreateRandomSquares(16);
		//CreateInitialState();
		UpdateTextureMap ();
	}
	
	// Update is called once per frame
	void Update () {
	
		KeyboardInput ();

		if (Paused == false) {
			Timer += Time.deltaTime;

			if (Timer >= TimeStep) {

				MoveAnt ();
				UpdateTextureMap ();
				StepCounter++;
				Timer = 0.0f;
			}
		}
	}

	public void MoveAnt (){
		//0 North, 1 East, 2 South, 3 West

		//North
		if (AntDirection == 0) {
			MoveNorth ();
			CheckPosition ();
		}

		//East
		else if (AntDirection == 1) {
			MoveEast ();
			CheckPosition ();
		}

		//South
		else if (AntDirection == 2) {
			MoveSouth ();
			CheckPosition ();
		}

		//West
		else if (AntDirection == 3) {
			MoveWest ();
			CheckPosition ();
		} 

		else {

		}

	}

	public void CreateInitialState(){
		for (int i = 0; i < 200; i++) {
			int x = Random.Range (0, TextureWidth);
			int y = Random.Range(0, TextureHeight);

			CellMap [y * TextureWidth + x] = 1;
		}
	}

	public void CheckPosition1(){
		if (AntPositionX > 1 && AntPositionX < TextureWidth-1 && AntPositionY > 1 && AntPositionY < TextureHeight -1) {
			if (CellMap [AntPositionY * TextureWidth + AntPositionX] == 0) {
				TurnRight ();

				CellMap [AntPositionY * TextureWidth + AntPositionX] = 1;


				if (CellMap [(AntPositionY + 1) * TextureWidth + (AntPositionX-1)] == 0) {
					CellMap [(AntPositionY + 1) * TextureWidth + (AntPositionX-1)] = 1;
				} else {
					CellMap [(AntPositionY + 1) * TextureWidth + (AntPositionX-1)] = 0;
				}


				if(CellMap [(AntPositionY+1) * TextureWidth + (AntPositionX + 1)] == 0){
					CellMap [(AntPositionY+1) * TextureWidth + (AntPositionX + 1)] = 1;
				} 
				else {
					CellMap [(AntPositionY+1) * TextureWidth + (AntPositionX + 1)] = 0;
				}


				if(CellMap [(AntPositionY-1) * TextureWidth + (AntPositionX - 1)] == 0){
					CellMap [(AntPositionY-1) * TextureWidth + (AntPositionX - 1)] = 1;
				}
				else {
					CellMap [(AntPositionY-1) * TextureWidth + (AntPositionX - 1)] = 0;
				}


				if(CellMap [(AntPositionY - 1) * TextureWidth + (AntPositionX+1)] == 0){
					CellMap [(AntPositionY - 1) * TextureWidth + (AntPositionX+1)] = 1;
				}
				else{
					CellMap [(AntPositionY - 1) * TextureWidth + (AntPositionX+1)] = 0;
				}


				ActivityMap [AntPositionY * TextureWidth + AntPositionX] += ActivityIncrement;
			} else {
				TurnLeft ();

				CellMap [AntPositionY * TextureWidth + AntPositionX] = 0;


				if (CellMap [(AntPositionY + 1) * TextureWidth + (AntPositionX-1)] == 1) {
					CellMap [(AntPositionY + 1) * TextureWidth + (AntPositionX-1)] = 0;
				} else {
					CellMap [(AntPositionY + 1) * TextureWidth + (AntPositionX-1)] = 1;
				}


				if(CellMap [(AntPositionY+1) * TextureWidth + (AntPositionX + 1)] == 1){
					CellMap [(AntPositionY+1) * TextureWidth + (AntPositionX + 1)] = 0;
				} 
				else {
					CellMap [(AntPositionY+1) * TextureWidth + (AntPositionX + 1)] = 1;
				}


				if(CellMap [(AntPositionY-1) * TextureWidth + (AntPositionX - 1)] == 1){
					CellMap [(AntPositionY-1) * TextureWidth + (AntPositionX - 1)] = 0;
				}
				else {
					CellMap [(AntPositionY-1) * TextureWidth + (AntPositionX - 1)] = 1;
				}


				if(CellMap [(AntPositionY - 1) * TextureWidth + (AntPositionX+1)] == 1){
					CellMap [(AntPositionY - 1) * TextureWidth + (AntPositionX+1)] = 0;
				}
				else{
					CellMap [(AntPositionY - 1) * TextureWidth + (AntPositionX+1)] = 1;
				}

				ActivityMap [AntPositionY * TextureWidth + AntPositionX] += ActivityIncrement;
			}
		} 
		else {
			Paused = true;
		}
	}

	public void CheckPosition(){
		if (AntPositionX > 0 && AntPositionX < TextureWidth && AntPositionY > 0 && AntPositionY < TextureHeight) {
			if (CellMap [AntPositionY * TextureWidth + AntPositionX] == 0) {
				TurnRight ();
				CellMap [AntPositionY * TextureWidth + AntPositionX] = 1;
				ActivityMap [AntPositionY * TextureWidth + AntPositionX] += ActivityIncrement;
			} else {
				TurnLeft ();
				CellMap [AntPositionY * TextureWidth + AntPositionX] = 0;
				ActivityMap [AntPositionY * TextureWidth + AntPositionX] += ActivityIncrement;
			}
		} 
		else {
			Paused = true;
		}
	}

	public void TurnRight(){
		AntDirection++;
		if (AntDirection > 3) {
			AntDirection = 0;
		}
	}

	public void TurnLeft(){
		AntDirection--;
		if (AntDirection < 0) {
			AntDirection = 3;
		}
	}

	public void MoveNorth(){
		AntPositionY++;
		if (AntPositionY > TextureHeight) {
			Paused = true;
		}
	}

	public void MoveEast(){
		AntPositionX++;
		if (AntPositionX > TextureWidth) {
			Paused = true;
		}
	}

	public void MoveSouth(){
		AntPositionY--;
		if (AntPositionY <= 0) {
			Paused = true;
		}
	}

	public void MoveWest(){
		AntPositionX--;
		if (AntPositionX <= 0) {
			Paused = true;
		}
	}

	public void KeyboardInput(){

		//Pause
		if (Input.GetKeyUp (KeyCode.Space)) {
			if (Paused == false) {
				Paused = true;
			} else {
				Paused = false;
			}
		}

		if (Input.GetKeyUp (KeyCode.A)) {
			if (MapId == 0) {
				MapId = 1;		//Activity Map
				SetTextureMap();
			}
			else {
				MapId = 0;		//Ant Texture Map
				SetTextureMap();
			}
		}
	}

	public void SetTextureMap(){
		if (MapId == 0) {
			gameObject.GetComponent<Renderer> ().material.mainTexture = AntTexture;
		}

		if (MapId == 1) {
			gameObject.GetComponent<Renderer> ().material.mainTexture = ActivityTexture;
		}
	}

	public void UpdateTextureMap(){
		if (MapId == 0) {
			for (int x = 0; x < TextureWidth; x++) {
				for (int y = 0; y < TextureHeight; y++) {
					float h = CellMap [y * TextureWidth + x];
					ColorMap [y * TextureWidth + x] = ColorSelection [CellMap [y * TextureWidth + x]];
				}
			}

			AntTexture.SetPixels (ColorMap);
			AntTexture.Apply ();
		}

		if (MapId == 1) {
			float c = 1.0f / 6.0f;
			for (int x = 0; x < TextureWidth; x++) {
				for (int y = 0; y < TextureHeight; y++) {
					int a = 0;
					int b = 1;
					float d = ActivityMap [y * TextureWidth + x];
					float g = 0.0f;

					if (d < c) {
						a = 0; b = 1;
						g = d / c;
					}
					if (d >= c && d < (c * 2.0f)) {
						a = 1; b = 2;
						g = (d - c) / c;
					}
					if (d >= (c * 2.0f) && d < (c * 3.0f)) {
						a = 2; b = 3;
						g = (d - (c * 2.0f)) / c;
					}
					if (d >= (c * 3.0f) && d < (c * 4.0f)) {
						a = 3; b = 4;
						g = (d - (c * 3.0f)) / c;
					}
					if (d >= (c * 4.0f) && d < (c * 5.0f)) {
						a = 4; b = 5;
						g = (d - (c * 4.0f)) / c;
					}
					if (d >= (c * 5.0f) && d < (c * 6.0f)) {
						a = 5; b = 6;
						g = (d - (c * 5.0f)) / c;
					}
//					if (d >= c && d < (c * 7.0f)) {
//						a = 6; b = 7;
//						g = d / (c * 7.0f);
//					}

					ColorMap [y * TextureWidth + x] = Color.Lerp ( ActivityColorRange[a], ActivityColorRange[b], g);
				}
			}

			ActivityTexture.SetPixels (ColorMap);
			ActivityTexture.Apply ();
		}
	}

	public void CreateCircle(int radius){
		for (int x = -radius; x < radius+1; x++) {
			for (int y = -radius; y < radius+1; y++) {
				float m = Mathf.Sqrt (Mathf.Pow((64 + x) - 64,2) + Mathf.Pow((64 + y) - 64, 2));
				float r = radius;
				if (m >= (r - 0.5f) && m <= (r + 0.5f)) {
					CellMap [(64 + y) * TextureWidth + (64 + x)] = 1;
				}
			}
		}
	}

	public void CreateRandomSquares(int number){
		int w = TextureWidth / 2;
		int h = TextureHeight / 2;

		for (int x = 0; x < number; x++) {
			int rx = Random.Range (w - 16, w + 16);
			int ry = Random.Range (h - 16, h + 16);

			CellMap [ry * TextureWidth + rx] = 1;
		}
	}

	public void FillTextureMap(int id){
		for (int x = 0; x < TextureWidth; x++) {
			for (int y = 0; y < TextureHeight; y++) {
				ColorMap [y * TextureWidth + x] = ColorSelection [id];
			}
		}

		AntTexture.SetPixels (ColorMap);
		AntTexture.Apply ();
	}
		
}
