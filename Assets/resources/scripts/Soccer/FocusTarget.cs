using UnityEngine;
using System.Collections;
using System.Linq;

public class FocusTarget : MonoBehaviour {

	MeshRenderer mr;
	SoccerManager sm;

	// Use this for initialization
	void Start () {
		mr = GetComponent<MeshRenderer>();
		sm = GameObject.Find("MainScripts").GetComponent<SoccerManager>();
	}
	
	// Update is called once per frame
	void Update () {
		if(SoccerManager.GetPlayers().Where(x => x.isFocused == true).ToArray().Length > 0) {
			mr.enabled = SoccerManager.GetPlayers().Single(x => x.isFocused == true).MyTeam == sm.whichIsMine;
			Vector3 focusPlayer = SoccerManager.GetPlayers().Single(x => x.isFocused == true).transform.position;
			transform.position = new Vector3(focusPlayer.x, 0.01f, focusPlayer.z);
		} else {
			mr.enabled = false;
		}
	}

}
