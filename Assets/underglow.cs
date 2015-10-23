using UnityEngine;
using System.Collections;

public class underglow : MonoBehaviour {

	private GameObject player;
	
	void Start () {
		player = GameObject.FindGameObjectWithTag("Player");
	}
	
	void Update () {
		this.transform.position = new Vector3(player.transform.position.x,-1f,player.transform.position.z);
	}
}
