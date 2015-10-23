using UnityEngine;
using System.Collections;

public class Footsteps : MonoBehaviour {
	
	public AudioSource myAudio;
	public AudioClip[] foots;
	
	public float speedMultiplier = 1;
	private float timer;
	private Vector3 prevLoc;
	
	void Start(){
		prevLoc = this.transform.position;
	}
	
	void Update () {
		timer += (this.transform.position-prevLoc).magnitude*speedMultiplier;
		prevLoc = this.transform.position;
		
		if (timer > 1){
			timer = 0;
			myAudio.clip = foots[(int)Mathf.Floor(Random.value*(foots.Length))];
			myAudio.Play();
		}
	}
}
