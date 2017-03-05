using UnityEngine;
using System.Collections;

public class GoalDetector : MonoBehaviour {

	public Teams goalType;

	public static bool isGoal;
	
	public static int lastGoal;

	bool GoalCounted, iisGoal;

	Ball pelota;

	Transform largero;

	// Use this for initialization
	void Start () {
		pelota = GameObject.Find("Ball").GetComponent<Ball>();
		largero = transform.FindChild("Cylinder06");
	}
	
	// Update is called once per frame
	void Update () {

		if(isGoal) {
			Debug.Log("Last goal: "+lastGoal+", Sum: "+(SoccerManager.AGs+SoccerManager.LGs));
			Debug.Log("Results: "+SoccerManager.LGs+" - "+SoccerManager.AGs);
			isGoal = false;
		}

		if(!pelota.isCatched) {
			Vector3 p = pelota.transform.position;
			if(goalType == Teams.Local) {
				iisGoal = (p.y < 3.8f && p.y > 0 && p.x > -5.5f && p.x < 5.5f && p.z > -73 && p.z < -70);
			} else if(goalType == Teams.Away) {
				iisGoal = (p.y < 3.8f && p.y > 0 && p.x > -5.5f && p.x < 5.5f && p.z > 70 && p.z < 73);
			} 
			if(iisGoal) {
				lastGoal = SoccerManager.AGs+SoccerManager.LGs;
				if(pelota.lastPlayerCatched.MyTeam == Teams.Local && goalType == Teams.Local) {
					if(!GoalCounted) {
						Debug.Log("Own goal!");
						SoccerManager.AGs += 1;
						GoalCounted = true;
						isGoal = true;
					}
				} else if(pelota.lastPlayerCatched.MyTeam == Teams.Away && goalType == Teams.Away) {
					if(!GoalCounted) {
						Debug.Log("Own goal!");
						SoccerManager.LGs += 1;
						GoalCounted = true;
						isGoal = true;
					}
				} else if(pelota.lastPlayerCatched.MyTeam == Teams.Local && goalType == Teams.Away) {
					if(!GoalCounted) {
						Debug.Log("Goal!");
						SoccerManager.LGs += 1;
						GoalCounted = true;
						isGoal = true;
					}
				} else if(pelota.lastPlayerCatched.MyTeam == Teams.Away && goalType == Teams.Local) {
					if(!GoalCounted) {
						Debug.Log("Goal!");
						SoccerManager.AGs += 1;
						GoalCounted = true;
						isGoal = true;
					}
				} 
			}
			if(!iisGoal && GoalCounted && lastGoal == SoccerManager.AGs+SoccerManager.LGs-1) {
				GoalCounted = false;
			}

		}

	}

}
