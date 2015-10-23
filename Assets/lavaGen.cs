using UnityEngine;
using System.Collections;

public class lavaGen : MonoBehaviour {
		
	public GameObject tile;
	public float tileSize;
	public int size;
	
	void Start () {
		size = (size/2)*2;
		for(int i=-(size/2);i<(size/2);i++){
			for(int j=-(size/2);j<(size/2);j++){
				GameObject t = Instantiate(tile,this.transform.position+new Vector3(i*tileSize,0,j*tileSize),Quaternion.Euler(90,0,0)) as GameObject;
				t.transform.parent = this.transform;
			}
		}
	}
	
	void Update () {
	
	}
}
