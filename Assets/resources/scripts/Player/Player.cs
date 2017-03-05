using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Player : MonoBehaviour {

	public static Transform playerFocused;

	public bool isFocused, isLeader, hasBall, imRobot;

	public float Attack, Defense, Total, Physic, Morale, Stamina, Velocity = 8;

	public Teams MyTeam = Teams.Local;

	public Positions myPosition;

	public int playerNum;

	public Vector3 myPos;

	Movement mv;

	SoccerManager sm;

	Ball pelota;

	Player myOldCopy;

	// Use this for initialization
	void Start() {
		mv = GetComponent<Movement>();
		mv.speedMovement = Velocity;
		sm = GameObject.Find("MainScripts").GetComponent<SoccerManager>();
		pelota = GameObject.Find("Ball").GetComponent<Ball>();
		if(isFocused) {
			CamFollow.target = transform;
		}
	}
	
	// Update is called once per frame
	void Update() {
		myOldCopy = this;
		imRobot = MyTeam != sm.whichIsMine;
		hasBall = pelota.playerCatched != null && pelota.playerCatched == transform;
		//if(transform.name == "Player 11") Debug.Log("Mine: "+transform.name+", closest: "+FindClosestPlayer().transform.name);
		if(isFocused) {
			playerFocused = transform;
		}
		if(isFocused && (!isLeader || pelota.firstCatch) && pelota.playerCatched != null && pelota.playerCatched != transform) {
			isFocused = false;
		}
		if(hasBall && imRobot) {
			try {
				Player closest = FindClosestPlayer(true).GetComponent<Player>();
				pelota.Pass(closest);
			} catch(System.Exception ex) {
				Debug.Log(ex);
			}
		}
		if(myOldCopy != this) {
			SoccerManager.EditPlayer(playerNum-1, this, MyTeam);
		}
	}

	public GameObject FindClosestPlayer(bool ismyTeam = false, float distance = Mathf.Infinity) {
		IEnumerable<GameObject> getted = GameObject.FindGameObjectsWithTag("Player").Where(x => x.transform != transform).Where(x => (x.transform.position - transform.position).sqrMagnitude < distance);
		if(ismyTeam) {
			float min = GameObject.FindGameObjectsWithTag("Player").Where(x => x.transform != transform).Where(x => (x.transform.position - transform.position).sqrMagnitude < distance).Where(x => x.GetComponent<Player>().MyTeam == MyTeam).Select(x => (x.transform.position - transform.position).sqrMagnitude).Min();
			return getted.Where(x => x.GetComponent<Player>().MyTeam == MyTeam).Where(x => (x.transform.position - transform.position).sqrMagnitude == min).ToArray()[0];
		} else {
			float min = GameObject.FindGameObjectsWithTag("Player").Where(x => x.transform != transform).Where(x => (x.transform.position - transform.position).sqrMagnitude < distance).Select(x => (x.transform.position - transform.position).sqrMagnitude).Min();
			return getted.Where(x => (x.transform.position - transform.position).sqrMagnitude == min).ToArray()[0];
		}
	}

}
