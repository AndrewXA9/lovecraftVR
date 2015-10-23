using UnityEngine;
using System.Collections;

public class Pickup : MonoBehaviour {
	
	public AudioSource audio;
	public GameObject fog;
		
	private int pickups = 0;
	
	private bool win = false;
	private float alpha;
	
	public GameObject[] scrolls;
	
	public MonoBehaviour[] disables;
	
	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "Pickup"){
			scrolls[pickups].SetActive(true);
			pickups++;
			Debug.Log("Pickup");
			Destroy(other.gameObject);
			audio.Play();
			//StartCoroutine(Progress());
		}
		
		if (other.gameObject.tag == "Altar" && !win && pickups >= 3){
			win = true;
			StartCoroutine(Winrar());
		}
	}
	
	IEnumerator Progress(){
		alpha = 255;
		while(alpha >0){
			alpha -= Time.deltaTime*100f;
			Debug.Log(alpha);
			//fog.GetComponent<Renderer>().material.SetColor("_Color",new Color(0,255,255,alpha));
			yield return null;
		}
	}
	
	
	IEnumerator Winrar(){
		alpha = 0;
		foreach(MonoBehaviour i in disables){
			i.enabled = false;
		}
		while(alpha < 255){
			alpha += Time.deltaTime/2f;
			fog.GetComponent<Renderer>().material.color = new Color(0,255,255,alpha);
			AudioListener.volume -= Time.deltaTime/2f;
			yield return null;
		}
	}
	
	
}
