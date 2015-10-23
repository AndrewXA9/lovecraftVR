using UnityEngine;
using System.Collections;

public class Floaty : MonoBehaviour {

	public float speed;
	public float distance;
	public float rotateSpeed;
	private Vector3 origin;
	
	void Start () {
		origin = this.transform.position;
	}
	
	void Update () {
		this.transform.position = new Vector3(origin.x,origin.y+(Mathf.Sin(Time.time*speed)*distance),origin.z);
		this.transform.RotateAround(this.transform.position,Vector3.up,rotateSpeed*Time.deltaTime);
	}
	
	void OnDrawGizmos(){
		Gizmos.color = Color.yellow;
		Gizmos.DrawLine(this.transform.position,this.transform.position+(Vector3.up*20f));
	}
}
