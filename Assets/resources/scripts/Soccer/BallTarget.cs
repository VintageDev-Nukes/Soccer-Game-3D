using UnityEngine;
using System.Collections;
using System.Linq;

public class BallTarget : MonoBehaviour {

	Ball pelota;

	void Start() {
		pelota = GameObject.Find("Ball").GetComponent<Ball>();
	}
	
	// Update is called once per frame
	void Update () {

		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit = new RaycastHit();
		Vector3 point = Vector3.zero;
		
		if (Physics.Raycast(ray, out hit)) {
			point = hit.point;
		}

		transform.position = new Vector3(point.x, 0.01f, point.z);

	}

}
