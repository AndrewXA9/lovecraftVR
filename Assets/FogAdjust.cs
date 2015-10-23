using UnityEngine;
using System.Collections;

public class FogAdjust : MonoBehaviour {
	
	public GameObject fog;
	
	private bool activate = false;
	private float alpha = 0;
	
	void Update () {
		if(this.transform.position.y < 0 && !activate){
			fog.SetActive(true);
			StartCoroutine(Foggy());
			activate = true;
		}
	}
	
	IEnumerator Foggy(){
		while(alpha < 255){
			alpha += Time.deltaTime;
			fog.GetComponent<Renderer>().material.color = new Color(0,255,0,alpha);
			yield return null;
		}
	}
	
	/*
	IEnumerator Foggy(){
		alpha += Time.deltaTime;
		fog.GetComponent<Renderer>().material.color = new Color(0,255,0,alpha);
		yield return null;
	}*/
}
