using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Generate : MonoBehaviour {
	
	private List<Vector2> squares = new List<Vector2>();
	private List<Vector2> extSquares = new List<Vector2>();
	
	//size of the seed rectangle
	public int initialSizeMin = 4;
	public int initialSizeMax = 10;
	
	//number of rectangles to generate the open areas
	public int plazaCountMin = 0;
	public int plazaCountMax = 6;
	
	//size of rectangles
	public int plazaSizeMin = 4;
	public int plazaSizeMax = 20;
	
	//number of protruding alleyways
	public int alleyCountMin = 4;
	public int alleyCountMax = 8;
	
	//length of alleyways
	public int alleyLengthMin = 10;
	public int alleyLengthMax = 20;
	
	//width of alleyways
	public int alleyWidth = 4;
	
	//how many times an alleyway will generate forward, turn 90 degrees, and continue. 1 = no turns
	public int alleyStretchesMax = 2;
	
	public List<GameObject> tiles;
	public GameObject dummyTile;
	public List<GameObject> walls;
	public GameObject Altar;
	public GameObject pickups;
	public int numPickups = 5;
	
	void Awake(){
		Gen();
	}
	
	void Gen(){
		
		squares.Clear();
		extSquares.Clear();
		
		List<Vector2> plaza = new List<Vector2>();
		List<Vector2> alleys = new List<Vector2>();
		
		Vector2 main = new Vector2(Random.Range((int)((initialSizeMin/2)*2),(int)((initialSizeMax/2)*2)),
								   Random.Range((int)((initialSizeMin/2)*2),(int)((initialSizeMax/2)*2)));
		
		//INITIAL SQUARE
		for(int i=(int)-(main.x/2);i<main.x/2;i++){
			for(int j=(int)-(main.y/2);j<main.y/2;j++){
				plaza.Add(new Vector2(i,j));
			}
		}
		
		//CENTER PLAZA
		int plazaCount = (int)Random.Range(plazaCountMin,plazaCountMax);
		for(int i=0;i<plazaCount;i++){
			Vector2 pick = plaza[Random.Range(0,plaza.Count-1)];
			//Debug.Log("Picked "+pick.x.ToString()+","+pick.y.ToString());
			Vector2 offset = new Vector2(Random.Range((int)((plazaSizeMin/2)*2),(int)((plazaSizeMax/2)*2)),
			                             Random.Range((int)((plazaSizeMin/2)*2),(int)((plazaSizeMax/2)*2)));
			//Debug.Log("Offset of "+offset.x.ToString()+","+offset.y.ToString());
			Vector4 extras = new Vector4(pick.x-(offset.x/2),pick.y-(offset.y/2),pick.x+(offset.x/2),pick.y+(offset.y/2));
			//Debug.Log("Result: "+extras.x.ToString()+","+extras.y.ToString()+" x "+extras.z.ToString()+","+extras.w.ToString());
			for(int j=(int)extras.x;j<extras.z;j++){
				for(int k=(int)extras.y;k<extras.w;k++){
					bool add = true;
					Vector2	newVec = new Vector2(j,k);
					foreach(Vector2 l in plaza){
						if(l == newVec){
							add = false;
						}
					}
					if(add){
						plaza.Add(newVec);
					}
				}
			}
		}
		
		//ALLEYS
		int alleyCount = (int)Random.Range(alleyCountMin,alleyCountMax);
		alleyWidth = (int)((alleyWidth/2)*2);
		for(int i=0;i<alleyCount;i++){
			
			List<Vector2> alley = new List<Vector2>();
			Vector2 dir = new Vector2(DirectionalBool(),DirectionalBool());
			Vector2 pick = plaza[Random.Range(0,plaza.Count-1)];
			
			
			int alleyStretches = Random.Range(1,alleyStretchesMax);
			for(int l=0;l<alleyStretches+1;l++){
			
			int alleyLength = (int)Random.Range(alleyLengthMin,alleyLengthMax);
			
				//plot alley
				for(int j=0;j<alleyLength;j++){
					for(int k=-(alleyWidth/2);k<alleyWidth/2;k++){
						Vector2 fill;
						if(dir.x>0){
							fill = pick+(Vector2.up*k);
						}
						else{
							fill = pick+(Vector2.right*k);
						}
						alleys.Add(fill);
					}
					
					if(dir.x>0){
						pick = pick+(Vector2.right*dir.y);
					}
					else{
						pick = pick+(Vector2.up*dir.y);
					}
					
				}
				
				//back up
				for(int j=0;j<(alleyWidth/2);j++){
					if(dir.x>0){
						pick = pick-(Vector2.right*dir.y);
					}
					else{
						pick = pick-(Vector2.up*dir.y);
					}
				}
				
				//rotate
				dir = new Vector2(-dir.x,DirectionalBool());
				//dir = new Vector2(DirectionalBool(),-dir.y);
				
				
			}
		}
		
		
		List<Vector2> tSquares = new List<Vector2>();
		tSquares.AddRange(plaza);
		tSquares.AddRange(alleys);
		
		foreach(Vector2 i in tSquares){
			bool add = true;
			foreach(Vector2 j in squares){
				if(i == j){
					add = false;
					break;
				}
			}
			if(add){
				squares.Add(i);
			}
		}
		
		//Add edge tiles
		List<Vector2> tEdge = new List<Vector2>();
		//Debug.Log("Adding edges");
		foreach(Vector2 i in squares){
			//Debug.Log("Checking "+i.ToString());
			for(int k=0;k<4;k++){
				bool match = false;
				Vector2 shifted = VectorShift(i,k);
				foreach(Vector2 j in squares){
					//Debug.Log("@"+i.ToString()+",Direction "+k.ToString()+": Checking "+j.ToString()+" to "+VectorShift(i,k).ToString());
					if(j == shifted){
						match = true;
						//Debug.Log("Match");
					}
				}
				if(!match){
					//Debug.Log("========ADDED "+VectorShift(i,k).ToString()+"========");
					tEdge.Add(shifted);
					Instantiate(walls[(int)Mathf.Floor(Random.value*(walls.Count))],new Vector3((shifted.x+0.5f)*2f,0f,(shifted.y+1.5f)*2f),DirToQuat((k+2)%4));
				}
			}
			
		}
		
		foreach(Vector2 i in tEdge){
			bool add = true;
			foreach(Vector2 j in extSquares){
				if(i == j){
					add = false;
					break;
				}
			}
			if(add){
				extSquares.Add(i);
			}
		}
		
		
		//Place tiles
		foreach(Vector2 i in squares){
			Instantiate(tiles[(int)Mathf.Floor(Random.value*(tiles.Count))],new Vector3((i.x+0.5f)*2f,0f,(i.y+1.5f)*2f),Quaternion.identity);
		}
		foreach(Vector2 i in extSquares){
			Instantiate(dummyTile,new Vector3((i.x+0.5f)*2f,0f,(i.y+1.5f)*2f),Quaternion.identity);
		}
		this.gameObject.SendMessage("FindTiles");
		
		//Instantiate(Altar,Vector3.zero,Quaternion.identity);
		for(int i=0;i<numPickups;i++){
			int rng = Random.Range(0,squares.Count-1);
			Instantiate(pickups,new Vector3((squares[rng].x+0.5f)*2f,0.75f,(squares[rng].y+1.5f)*2f),Quaternion.identity);
		}
		
	}
	
	void Update(){
		if(Input.GetKeyDown(KeyCode.Space)){
			Application.LoadLevel(Application.loadedLevel);
		}
	}
	
	void OnDrawGizmos(){
		Gizmos.color = Color.red;
		foreach(Vector2 i in squares){
			Gizmos.DrawWireCube(new Vector3((i.x+0.5f)*2f,0f,(i.y+1.5f)*2f),new Vector3(2f,0f,2f));
		}
		Gizmos.color = Color.green;
		foreach(Vector2 i in extSquares){
			Gizmos.DrawWireCube(new Vector3((i.x+0.5f)*2f,0f,(i.y+1.5f)*2f),new Vector3(2f,0f,2f));
		}
	}
	
	Vector2 VectorShift(Vector2 origin,int dir){
		Vector2 tVec = origin;
		if(dir<4){
			switch(dir){
			case 0:
				tVec += (Vector2.right);
				break;
			case 1:
				tVec += (Vector2.up);
				break;
			case 2:
				tVec -= (Vector2.right);
				break;
			case 3:
				tVec -= (Vector2.up);
				break;
			}
			
		}
		return tVec;
	}
	
	int DirectionalBool(bool input){
		if (input){
			return 1;
		}
		else{
			return -1;
		}
	}
	int DirectionalBool(){
		if (((int)((Random.value*10)%2)) == 0){
			return DirectionalBool(true);
		}
		else{
			return DirectionalBool(false);
		}
	}
	Quaternion DirToQuat(int dir){
		float deg = 90;
		for(int i=0;i<dir;i++){
			deg -= 90;
		}
		return Quaternion.Euler(new Vector3(0,deg,0));
	}
	
}



