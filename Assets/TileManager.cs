using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileManager: MonoBehaviour {
	public List<HeatMapEntity> tiles;
	public static float gridSize = 2.0f;
	public static float lavaHeight = -10.0f;
	public float heatStrength = 0.01f;
	public float heatDistance = 5f;
	public AnimationCurve heatFalloff;
	public float heatLimit = 5f;
	private GameObject player;
	
	public List<Material> cracks;
	
	void Awake () {
		player = GameObject.FindGameObjectWithTag("Player");
		tiles = new List<HeatMapEntity>();
	}
	
	void FixedUpdate(){
		if(tiles.Count > 0){
			//Debug.Log("greater than 0");
			List<HeatMapEntity> deadTiles = new List<HeatMapEntity>();
			for(int i = 0;i<tiles.Count;i++){
				float playerDist = (tiles[i].tile.transform.position-player.transform.position).magnitude;
				if((playerDist < heatDistance)&&(tiles[i].properties.destructible)){
					tiles[i].heat += heatStrength*heatFalloff.Evaluate(playerDist/heatDistance);
					
					tiles[i].properties.alpha.GetComponent<Renderer>().material = cracks[Mathf.Clamp((int)Mathf.Ceil((tiles[i].heat/heatLimit)*(cracks.Count-1)),0,cracks.Count-1)];
					
				}
				if(tiles[i].heat >= heatLimit){
					tiles[i].tile.gameObject.SendMessage("breakTile");
					deadTiles.Add(tiles[i]);
					player.transform.position += Vector3.up*Random.Range(0.1f,0.2f)*4;
					//Debug.Log("Kill");
				}
			}
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
	
	
	void FindTiles(){
		Debug.Log("Finding Tiles");
		tiles = new List<HeatMapEntity> ();
		GameObject[] aTiles = GameObject.FindGameObjectsWithTag ("Tile");
		foreach(GameObject i in aTiles){
			tiles.Add(new HeatMapEntity(i,0,i.GetComponent<Tile>()));
			i.SendMessage("Wake");
			//Debug.Log("added");
		}
		
		foreach(HeatMapEntity i in tiles){
			if(i.tile.gameObject.transform.position.magnitude < heatDistance){
				i.tile.GetComponent<Tile>().destructible = false;
				//Debug.Log("locking tile");
			}
		}
	}
	
}

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
