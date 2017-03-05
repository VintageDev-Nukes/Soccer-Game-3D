using UnityEngine;
using System.Collections;

public class CamFollow : MonoBehaviour {

	//public Transform target;
	public float height = 20, margin = 20, gap = 0, bandMarginLeft = -50, bandMarginRight = 62;

	public static Transform target;

	// Use this for initialization
	void Start() {

	}
	
	// Update is called once per frame
	void Update() {
		if(transform.position.x < bandMarginLeft) {
			margin = -Mathf.Abs(margin);
		} else if(transform.position.x > bandMarginRight) {
			margin = Mathf.Abs(margin);
		} else {
			margin = Mathf.Abs(margin);
		}
		Vector3 tgPos = target.position;
		transform.position = new Vector3(tgPos.x+margin, height, tgPos.z+gap); //Mathf.Lerp(margin, tgPos.x, Time.deltaTime)
		transform.LookAt(target);
	}

}
