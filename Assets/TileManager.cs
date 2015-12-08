using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//this is where the heatmap data is stored and calculated
public class TileManager: MonoBehaviour {

	//container for all tile heat data
	public List<HeatMapEntity> tiles;
	
	//size of tiles
	public static float gridSize = 2.0f;
	
	//elevation for tiles to fall
	public static float lavaHeight = -10.0f;
	//base strength of tile heat
	public float heatStrength = 0.01f;
	//distance from tile to player to activate heat
	public float heatDistance = 5f;
	//curve to adjust strength of heat from player over distance
	public AnimationCurve heatFalloff;
	//heat limit, tiles break when exceeding this
	public float heatLimit = 5f;
	//player object
	private GameObject player;
	
	public List<Material> cracks;
	
	void Awake () {
		//find player
		player = GameObject.FindGameObjectWithTag("Player");
		
	}
	
	void FixedUpdate(){
		//if there are still tiles left
		if(tiles.Count > 0){
		
			//store any tile indicies that break
			List<HeatMapEntity> deadTiles = new List<HeatMapEntity>();
			
			//iterate through enabled tile list
			for(int i = 0;i<tiles.Count;i++){
			
				//get player distance to tile
				float playerDist = (tiles[i].tile.transform.position-player.transform.position).magnitude;
				if((playerDist < heatDistance)&&(tiles[i].properties.destructible)){
					//apply heat equasion to tile
					tiles[i].heat += heatStrength*heatFalloff.Evaluate(playerDist/heatDistance);
					//crack tiles according to heat
					tiles[i].properties.alpha.GetComponent<Renderer>().material = cracks[Mathf.Clamp((int)Mathf.Ceil((tiles[i].heat/heatLimit)*(cracks.Count-1)),0,cracks.Count-1)];
					
				}
				if(tiles[i].heat >= heatLimit){
					//break tiles that exceeded heat limit, initialize their fall logic
					tiles[i].tile.gameObject.SendMessage("breakTile");
					deadTiles.Add(tiles[i]);
					player.transform.position += Vector3.up*Random.Range(0.1f,0.2f)*4;
					//Debug.Log("Kill");
				}
			}
			//remove all broken tiles from list so we don't waste time applying heat
			foreach(HeatMapEntity i in deadTiles){
				tiles.Remove(i);
			}
		}
	}
	
	void OnDrawGizmos(){
		foreach(HeatMapEntity i in tiles){
			Gizmos.color = new Color(1f,0f,0f,0.5f);
			float size = (gridSize*(i.heat/heatLimit));
			Gizmos.DrawCube(i.tile.transform.position+(Vector3.up*1.5f),new Vector3(size,0.2f,size));
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(i.tile.transform.position,i.tile.transform.position+(Vector3.up*5));
		}
	}
	
	//populate tile list, called from Generate.cs
	void FindTiles(){
		Debug.Log("Finding Tiles");
		//initialize tile list
		tiles = new List<HeatMapEntity> ();
		//make temp list for found tiles
		GameObject[] aTiles = GameObject.FindGameObjectsWithTag ("Tile");
		//make noew hearmap entity for every tiles
		foreach(GameObject i in aTiles){
			tiles.Add(new HeatMapEntity(i,0,i.GetComponent<Tile>()));
			i.SendMessage("Wake");
			//Debug.Log("added");
		}
		
		//lock tiles near origin where player spawns
		foreach(HeatMapEntity i in tiles){
			if(i.tile.gameObject.transform.position.magnitude < heatDistance){
				i.tile.GetComponent<Tile>().destructible = false;
				//Debug.Log("locking tile");
			}
		}
	}
	
}

//small helper class for storing tile and tile's heat data
[System.Serializable]
public class HeatMapEntity{
	public GameObject tile;
	public float heat;
	public Tile properties;
	public HeatMapEntity(GameObject _tile,float _heat,Tile _properties){
		tile = _tile;
		heat = _heat;
		properties = _properties;
	}
}
