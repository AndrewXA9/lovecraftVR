using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour {

	//neighboring tiles
	public GameObject[] neighbors;
	
	//can tile be destroyed?
	public bool destructible = true;
	
	[HideInInspector]
	//minimal state machine value
	public int fallState = 0;
	
	//this vector array is for translating 0 through 3 into vector directions
	private Vector3[] directions = new Vector3[]{Vector3.forward,
												 Vector3.left,
												-Vector3.forward,
												-Vector3.left};
	
	//visual properties for falling tiles
	public Vector3 rotationSpeedBase;
	public float rotationSpeedRandom = 5;
	private Quaternion rotationSpeedFinal;
	public float fallSpeed = 1.0f;
	public float fallCoefficient = 1.0f;//incorrect usage of coefficient but whatever
	
	//light source
	public GameObject myLight;
	
	//cracks on tile
	public GameObject alpha;
	
	//broken tile edges
	public GameObject[] brokens;
	
	//audio source for breaking sound
	public AudioSource myAudio;
	//breaking sounds
	public AudioClip[] breakSounds;
	
	void Wake() {
	
		//force debug drawing?
		on = true;
		
		//creating random rotation values
		rotationSpeedFinal.eulerAngles = rotationSpeedBase+new Vector3(Random.Range(-rotationSpeedRandom,rotationSpeedRandom),
		                                                               Random.Range(-rotationSpeedRandom,rotationSpeedRandom),
		                                                               Random.Range(-rotationSpeedRandom,rotationSpeedRandom));
		                                                               
		//initialize neighbor array
		neighbors = new GameObject[4];
		
		//find neighbors via raycast
		RaycastHit hit;
		int mask = 1 << 8; //only detect tiles
		for(int i=0;i<directions.Length;i++){
			if (Physics.Raycast(this.transform.position+(directions[i]*TileManager.gridSize)+Vector3.up,Vector3.down,out hit,5f,mask)){
				//assign neighbor tile when hit
				neighbors[i] = hit.collider.gameObject.transform.parent.gameObject;
			}
			else{
				//no neighbor found
				neighbors[i] = null;
			}
		}
		
		
	}
	
	public void Update(){
		
		//0 = not falling, 1 = falling, 2 = fell
		if(fallState == 1){
		
			//rotate and drop during fall
			this.transform.position -= Vector3.up*fallSpeed*Time.deltaTime;
			fallSpeed += fallCoefficient;
			this.transform.rotation = this.transform.rotation*rotationSpeedFinal;
			
			if(this.transform.position.y < TileManager.lavaHeight){
				//stop falling
				fallState = 2;
			}
			
		}
		
	}
	
	//for initiating falling, called from Beam.cs or TileManager.cs
	public void breakTile(){
		//check if destructible
		if(destructible){
			//change state, activate light, play sound
			fallState = 1;
			myLight.active = true;
			myLight.transform.parent = null;
			myAudio.clip = breakSounds[(int)Mathf.Floor(Random.value*(breakSounds.Length))];
			myAudio.Play();
		}
		//send lighting check to neighbors
		for(int i=0;i<4;i++){
			if(neighbors[i] != null){
				neighbors[i].SendMessage("LightingCheck");
			}
		}
		//check own lighting (y'know, just in case)
		this.gameObject.SendMessage("LightingCheck");
		
	}
	
	//for checking if tile is broken and has no neighbors to turn off light
	public void LightingCheck(){
	
		//variable to see if there are neighbors left
		int keep = 0;
		for(int i=0;i<4;i++){
			if(neighbors[i] != null){
				//iterate thru neighbors and check their fall states
				if(neighbors[i].GetComponent<Tile>().fallState == 0){
					keep += 1;
				}
				else{
					GameObject t = Instantiate(brokens[(int)Mathf.Floor(Random.value*(brokens.Length))],this.transform.position,DirToQuat((i+1)%4)) as GameObject;
					t.transform.position += t.transform.forward;
					t.transform.position -= t.transform.up*Random.Range(0.0f,0.1f);
					t.transform.parent = this.transform;
				}
			}
		}
		//if no neighbors, slowly and sadly disable light (see LightToggle.cs)
		if(keep == 0){
			myLight.SendMessage("FadeOut",SendMessageOptions.DontRequireReceiver);
			//Debug.Log("sent");
		}
	}
	
	//toggle for debug gizmos
	private bool on = false;
	
	void OnDrawGizmos() {
		
		//draw connections to neighbor tiles
		Gizmos.color = Color.red;
		if (on){
			for(int i=0;i<4;i++){
				if(neighbors[i] != null){
					Gizmos.DrawLine(this.transform.position+Vector3.up,neighbors[i].transform.position);
				}
			}
		}
	}
	
	//Helper class to convert from int direction to quaternion
	Quaternion DirToQuat(int dir){
		float deg = 90;
		for(int i=0;i<dir;i++){
			deg -= 90;
		}
		return Quaternion.Euler(new Vector3(0,deg,0));
	}
	
}

