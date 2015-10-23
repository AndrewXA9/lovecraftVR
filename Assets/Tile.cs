using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour {
	public GameObject[] neighbors;
	public bool destructible = true;
	[HideInInspector]
	public int fallState = 0;
	
	private Vector3[] directions = new Vector3[]{Vector3.forward,
												 Vector3.left,
												-Vector3.forward,
												-Vector3.left};
	
	public Vector3 rotationSpeedBase;
	public float rotationSpeedRandom = 5;
	private Quaternion rotationSpeedFinal;
	public float fallSpeed = 1.0f;
	public float fallCoefficient = 1.0f;
	
	public GameObject myLight;
	public GameObject alpha;
	
	public GameObject[] brokens;
	public AudioSource myAudio;
	public AudioClip[] breakSounds;
	
	void Wake() {
		on = true;
		
		rotationSpeedFinal.eulerAngles = rotationSpeedBase+new Vector3(Random.Range(-rotationSpeedRandom,rotationSpeedRandom),
		                                                               Random.Range(-rotationSpeedRandom,rotationSpeedRandom),
		                                                               Random.Range(-rotationSpeedRandom,rotationSpeedRandom));
	
		neighbors = new GameObject[4];
		
		RaycastHit hit;
		int mask = 1 << 8;
		for(int i=0;i<directions.Length;i++){
			if (Physics.Raycast(this.transform.position+(directions[i]*TileManager.gridSize)+Vector3.up,Vector3.down,out hit,5f,mask)){
				neighbors[i] = hit.collider.gameObject.transform.parent.gameObject;
			}
			else{
				neighbors[i] = null;
			}
		}
		
		
	}
	
	public void Update(){
		if(fallState == 1){
			this.transform.position -= Vector3.up*fallSpeed*Time.deltaTime;
			fallSpeed += fallCoefficient;
			this.transform.rotation = this.transform.rotation*rotationSpeedFinal;
			
			if(this.transform.position.y < TileManager.lavaHeight){
				fallState = 2;
			}
			
		}
		
	}
	
	public void breakTile(){
		if(destructible){
			fallState = 1;
			myLight.active = true;
			myLight.transform.parent = null;
			myAudio.clip = breakSounds[(int)Mathf.Floor(Random.value*(breakSounds.Length))];
			myAudio.Play();
		}
		for(int i=0;i<4;i++){
			if(neighbors[i] != null){
				neighbors[i].SendMessage("LightingCheck");
			}
		}
		this.gameObject.SendMessage("LightingCheck");
		
	}
	public void LightingCheck(){
		int keep = 0;
		for(int i=0;i<4;i++){
			if(neighbors[i] != null){
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
		if(keep == 0){
			myLight.SendMessage("FadeOut",SendMessageOptions.DontRequireReceiver);
			//Debug.Log("sent");
		}
	}
	
	private bool on = false;
	
	void OnDrawGizmos() {
		
		Gizmos.color = Color.red;
		if (on){
			for(int i=0;i<4;i++){
				if(neighbors[i] != null){
					Gizmos.DrawLine(this.transform.position+Vector3.up,neighbors[i].transform.position);
				}
			}
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

