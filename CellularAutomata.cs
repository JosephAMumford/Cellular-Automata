using UnityEngine;
using System.Collections;

public class CellularAutomata : MonoBehaviour {
	
	public int MapSize;
	public byte Rule;
	public Color[] WorldColors;
	public Texture2D CellMap;
	public int Dimension;
	int[] CellBinaryMap;
	int[] MyRuleArray;
	public int Iterations;

	float Timer;
	public bool Paused;
	
	void Start () {

		GenerateRule();
		CellBinaryMap = new int[MapSize*MapSize];
		CellMap = new Texture2D(MapSize, MapSize);
		CellMap.filterMode = FilterMode.Point;
		CellMap.wrapMode = TextureWrapMode.Clamp;
		gameObject.GetComponent<Renderer>().material.mainTexture = CellMap;

		GenerateCellMap(Dimension);

	}


	void OnGUI(){
		GUI.Label (new Rect(16, Screen.height-32,128,32), Iterations.ToString() );
	}

	// Update is called once per frame
	void Update () {
	
		if(Dimension == 2){
			if(Paused == false){
				Timer += Time.deltaTime;
				if(Timer > 1.0f){
					Timer = 0.0f;
					Update2DCellMap();
					Iterations++;
				}
			}
		}

		//Keyboard Input
		if(Input.GetKeyDown(KeyCode.G)){
			GenerateRule();
			GenerateCellMap(Dimension);
		}

		if(Input.GetKeyUp(KeyCode.LeftBracket)){
			Rule--;
			if(Rule < 0){
				Rule = 255;
			}
			GenerateCellMap(Dimension);
		}

		if(Input.GetKeyUp (KeyCode.RightBracket)){
			Rule++;
			if(Rule > 255){
				Rule = 0;
			}
			GenerateCellMap(Dimension);
		}

		if(Input.GetKeyUp (KeyCode.C)){
			Debug.Log ("Rule: " + Rule + " MyRuleArray: " + MyRuleArray[7] + MyRuleArray[6] + MyRuleArray[5] + 
			           MyRuleArray[4] + MyRuleArray[3] + MyRuleArray[2] + MyRuleArray[1] + MyRuleArray[0]);
			int d = BinaryToDecimal(MyRuleArray);
			Debug.Log ("Rule from binary: " + d);
		}

		if(Input.GetKeyDown(KeyCode.S)){
			if(Paused == false){
				Paused = true;
			}
			else{
				Paused = false;
			}
		}

		if(Input.GetKeyUp(KeyCode.Space)){
			Update2DCellMap();
		}
	}

	//Generate Random Rule
	void GenerateRule(){
		Rule = (byte)Random.Range (0,255); 
	}

	//Convert integer to 8-bit array
	int[] DecimalToBinary(int rule){
		int[] binary = new int[8];
		int r1, r2;
		r1 = rule;
		for(int i = 0; i < 8; i++){
			binary[i] = r1 % 2;
			r1 = r1/2;
		}

		return binary;
	}

	//Convert 8-bity array to integer
	int BinaryToDecimal(int[] binary){
		int d = 0;

		for(int i = 0; i < 8; i++){
			if(binary[i] == 1){
				d += (int)Mathf.Pow(2,i);
			}
		}

		return d;
	}


	//Generates map depending on dimension
	void GenerateCellMap(int dimension){
		if(dimension == 1){
			Iterations = 0;
			//SetBitArray();
			MyRuleArray = DecimalToBinary(Rule);
			Initialize1DCellBinaryMap();
			Generate1DCellMap();
		}
		if(dimension == 2){
			MyRuleArray = DecimalToBinary(Rule);
			Iterations = 0;
			Initialize2DCellBinaryMap();
			Initialize2DMap();
			//Generate2DCellMap();
		}
	}

	void Update2DCellMap(){
		Generate2DCellMap();
	}

	void Initialize2DCellBinaryMap(){
		for(int x = 0; x < MapSize; x++){
			for(int y = 0; y < MapSize; y++){
				CellBinaryMap[y * MapSize + x] = 0;
			}
		}

		for(int dx = 0; dx < MapSize/3; dx++){
			for(int dy = 0; dy < MapSize/3; dy++){
				int hx = Random.Range (0,MapSize);
				int hy = Random.Range (0,MapSize);
				CellBinaryMap[hy * MapSize + hx] = 1;
			}
		}
		//dx = Random.Range (0,MapSize);
		//dy = Random.Range (0,MapSize);
//		dx = MapSize/2;
//		dy = MapSize/2;
//		CellBinaryMap[0 * MapSize + dx] = 1;
	}

	void Initialize2DMap(){
		Color[] ColorMap = new Color[MapSize * MapSize];
		
		for(int y = 0; y < MapSize; y++){
			for(int x = 0; x < MapSize; x++){
				int ty = MapSize-y-1;		//Texture pixel array has inverted y, this flips it 

				ColorMap[ty * MapSize + x] = WorldColors[CellBinaryMap[y * MapSize + x]];
			}
		}

		CellMap.SetPixels(ColorMap);
		CellMap.Apply();
	}

	void Generate2DCellMap(){
		Color[] ColorMap = new Color[MapSize * MapSize];

		for(int y = 0; y < MapSize; y++){
			for(int x = 0; x < MapSize; x++){
				int ty = MapSize-y-1;		//Texture pixel array has inverted y, this flips it 

				int a,b,c,d,e,f,g,h,i;

				//Get upper left value
				if(x-1 >= 0 && y > 0){	a = CellBinaryMap[(y-1) * MapSize + (x-1)];	} else { a = CellBinaryMap[(MapSize-1) * MapSize + (MapSize-1)];	}
				
				//Get upper center value
				if(y > 0){	b = CellBinaryMap[(y-1) * MapSize + x];	} else { b = CellBinaryMap[(MapSize-1) * MapSize + x];	}

				//Get upper right value
				if(x+1 < MapSize && y > 0){ c = CellBinaryMap[(y-1) * MapSize + (x+1)];	} else { c = CellBinaryMap[(MapSize-1) * MapSize + (0)];	}

				//Get left value
				if(x-1 >= 0){	 d = CellBinaryMap[y * MapSize + (x-1)];	} else { d = CellBinaryMap[y * MapSize + (MapSize-1)];	}
				
				//Get center value
				e = CellBinaryMap[y * MapSize + x];
				
				//Get right value
				if(x+1 < MapSize){ f = CellBinaryMap[y * MapSize + (x+1)];	} else { f = CellBinaryMap[y * MapSize + (0)];	}

				//Get lower left value
				if(x-1 >= 0 && y < MapSize-1){	g = CellBinaryMap[(y+1) * MapSize + (x-1)];	} else { g = CellBinaryMap[(0) * MapSize + (MapSize-1)];	}

				//Get lower center value
				if(y < MapSize-1){	h = CellBinaryMap[(y+1) * MapSize + x];	} else { h = CellBinaryMap[(0) * MapSize + x];	}

				//Get lower right value
				if(x+1 < MapSize && y < MapSize-1){ i = CellBinaryMap[(y+1) * MapSize + (x+1)];	} else { i = CellBinaryMap[(0) * MapSize + (0)];	}

				//Get result of rule using left, center, and right values
				//int o = Get2DResult(a,b,c,d,e,f,g,h,i);

				int n,m,o;

				n = Get1DResult(a,b,c);
				m = Get1DResult(d,e,f);
				o = Get1DResult(g,h,i);

				d = Get1DResult(n,m,o);

				ColorMap[ty * MapSize + x] = WorldColors[d];
				CellBinaryMap[y * MapSize + x] = d;

				//ColorMap[ty * MapSize + x] = WorldColors[CellBinaryMap[y * MapSize + x]];
			}
		}

		
		CellMap.SetPixels(ColorMap);
		CellMap.Apply();
	}

	int Get2DResult(int a, int b, int c, int d, int e, int f, int g, int h, int i){
		int w = 0;
		int z = 0;
//		int[] binaryArray = new int[8];
//		binaryArray[0] = a;
//		binaryArray[1] = b;
//		binaryArray[2] = c;
//		binaryArray[3] = d;
//		binaryArray[4] = f;
//		binaryArray[5] = g;
//		binaryArray[6] = h;
//		binaryArray[7] = i;
//
//		int x = BinaryToDecimal(binaryArray);
//
//		if(x < Rule){
//			w = 1;
//		}
//		else {
//			w = 0;
//		}

		if(a == 1){ w++;	}
		if(b == 1){ w++;	}
		if(c == 1){ w++;	}
		if(d == 1){ w++;	}
		if(f == 1){ w++;	}
		if(g == 1){ w++;	}
		if(h == 1){ w++;	}
		if(i == 1){ w++;	}


		if(e == 1 && w <= 1){
			z = 0;
		}
		if(e == 1 && w >= 2){
			z = 1;
		}
		if(e == 0 && w == 3){
			z = 1;
		}

		return z;
	}

	void Initialize1DCellBinaryMap(){
		for(int x = 0; x < MapSize; x++){
			for(int y = 0; y < MapSize; y++){
				CellBinaryMap[y * MapSize + x] = 0;
			}
		}
		int g = MapSize/2;
		CellBinaryMap[0 * MapSize + g] = 1;			//Sets initial value on top row in the middle
	}

	void Generate1DCellMap(){

		Color[] ColorMap = new Color[MapSize * MapSize];

		for(int y = 0; y < MapSize; y++){
			for(int x = 0; x < MapSize; x++){
				int ty = MapSize-y-1;		//Texture pixel array has inverted y, this flips it 

				if(y != 0){
					int a,b,c,d;

					//Get left value
					if(x-1 >= 0){	 a = CellBinaryMap[(y-1) * MapSize + (x-1)];	} else { a = CellBinaryMap[(y-1) * MapSize + (MapSize-1)];	}

					//Get center value
					b = CellBinaryMap[(y-1) * MapSize + x];

					//Get right value
					if(x+1 < MapSize){ c = CellBinaryMap[(y-1) * MapSize + (x+1)];	} else { c = CellBinaryMap[(y-1) * MapSize + (0)];	}

					//Get result of rule using left, center, and right values
					d = Get1DResult(a,b,c);

					ColorMap[ty * MapSize + x] = WorldColors[d];
					CellBinaryMap[y * MapSize + x] = d;
				}
				else{
					ColorMap[ty * MapSize + x] = WorldColors[CellBinaryMap[y * MapSize + x]];
				}
			}
		}
		
		CellMap.SetPixels(ColorMap);
		CellMap.Apply();
	}

	int Get1DResult(int x, int y, int z){
		int w = 0;

		if(x == 1 && y == 1 && z == 1){	w = MyRuleArray[7];	}
		if(x == 1 && y == 1 && z == 0){	w = MyRuleArray[6];	}
		if(x == 1 && y == 0 && z == 1){	w = MyRuleArray[5];	}
		if(x == 1 && y == 0 && z == 0){	w = MyRuleArray[4];	}
		if(x == 0 && y == 1 && z == 1){	w = MyRuleArray[3];	}
		if(x == 0 && y == 1 && z == 0){	w = MyRuleArray[2];	}
		if(x == 0 && y == 0 && z == 1){	w = MyRuleArray[1];	}
		if(x == 0 && y == 0 && z == 0){	w = MyRuleArray[0];	}

		return w;
	}
}
