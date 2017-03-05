using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Ball : MonoBehaviour {

	public static Ball pelota;

	/// this script pushes all rigidbodies that the character touches
	public float pushPower = 2, radiusToStick = 1, distToFind = 2.5f, shootSpeed = 5, heightToShoot = 2.5f, accuracy = 75;

	public Transform playerCatched;

	public bool isCatched, isShooted, firstCatch, isInsideField;

	public Click buttonToClick;

	public Player lastPlayerCatched;

	private Vector3 vVelocity, lastPosition, curPosition, endPoint;

	private float velocity;

	private float mRad;

	SoccerManager sm;

	void Start() {
		pelota = this;
		sm = GameObject.Find("MainScripts").GetComponent<SoccerManager>();
		mRad = GetComponent<SphereCollider>().radius;
	}

	void Update() {
		RaycastHit hit = new RaycastHit();
		Vector3 ballPos = transform.position;
		isCatched = playerCatched != null;
		isInsideField = ballPos.x > -53.65878f && ballPos.x < 53.18615f && ballPos.z > -69.50797f && ballPos.z < 70.47273f;
		if(BallHasHit(out hit, (1 << 8), mRad*distToFind*10)) {
			Player playerHitted = hit.transform.GetComponent<Player>();
			//Debug.Log(playerHitted.name);
			Player pFocused = SoccerManager.GetPlayers().Where(x => x.MyTeam == sm.whichIsMine).SingleOrDefault(x => x.isFocused == true);
			if(pFocused != null && !pFocused.hasBall) {
				pFocused.isFocused = false;
				if(!playerHitted.hasBall && !playerHitted.isFocused) {
					playerHitted.isFocused = true;
				}
			}
		}
		if(BallHasHit(out hit, (1 << 8), mRad*distToFind)) { 
			if(!isCatched && !isShooted) {
				//Debug.Log("Ball catched.");
				playerCatched = hit.transform;
				playerCatched.GetComponent<Player>().isFocused = true;
			}
		} else {
			if(isShooted) {
				isShooted = false;
			}
		}
		if(isCatched) {
			CamFollow.target = playerCatched;
			if(Input.GetMouseButtonDown((int)buttonToClick)) {
				ShootBall();
			}
		} else {
			CamFollow.target = transform;
		}
		if(transform.position.y < 0 && isInsideField) {
			Vector3 pos = transform.position;
			transform.position = new Vector3(pos.x, 1, pos.z);
		}
		if(playerCatched != null && playerCatched.GetComponent<Player>() != null) {
			if(!Input.GetMouseButtonDown((int)buttonToClick)) {
				Catch();
			}
			if(!playerCatched.GetComponent<Player>().hasBall) {
				playerCatched.GetComponent<Player>().hasBall = true;
			}
		}
	}

	void OnControllerColliderHit(ControllerColliderHit hit) {

		Rigidbody body = hit.collider.attachedRigidbody;

		// no rigidbody
		if(body == null || body.isKinematic)
			return;
		
		// We dont want to push objects below us
		if(hit.moveDirection.y < -0.3f) 
			return;
		
		// Calculate push direction from move direction, 
		// we only push objects to the sides never up and down
		Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
		
		// If you know how fast your character is trying to move,
		// then you can also multiply the push velocity by that.
		
		// Apply the push
		body.velocity = pushDir * pushPower;
	
	} 

	void FixedUpdate() {
		//Debug.DrawRay(transform.position, transform.forward, Color.blue);
		curPosition = transform.position;
		vVelocity = (lastPosition - curPosition) / Time.deltaTime;
		velocity = (lastPosition - curPosition).magnitude / Time.deltaTime;
		lastPosition = transform.position;
	}

	void Catch() {

		Transform pl = playerCatched;
		float theta = pl.eulerAngles.y;
		Vector3 margin = new Vector3(radiusToStick*Mathf.Cos(theta*Mathf.PI/180), -0.5f, radiusToStick*Mathf.Sin(theta*Mathf.PI/180));
		transform.position = pl.position+margin;

		lastPlayerCatched = playerCatched.GetComponent<Player>();
		
		if(!firstCatch) {
			firstCatch = true;
		}

	}
	
	void ShootBall() {

		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit = new RaycastHit();
		Vector3 point = Vector3.zero;
	
		if (Physics.Raycast(ray, out hit)) {
			point = hit.point;
		}

		endPoint = point;
		StartCoroutine(Shoot(endPoint, heightToShoot, shootSpeed, accuracy));

	}

	public void Pass(Player pl) {
		endPoint = pl.transform.position;
		StartCoroutine(Shoot(endPoint, 0, shootSpeed, accuracy, !pl.imRobot));
	}

	IEnumerator Shoot(Vector3 end, float height = 1, float speed = 1, float acc = 100, bool bot = false, float overTime = 1) {
		rigidbody.constraints = RigidbodyConstraints.FreezePosition;
		float startTime = Time.time, startVelocity = 0;
		end += (Vector3.up * height) * ((bot == true) ? 1 : (acc / 100));
		Vector3 start = transform.position;
		while(Time.time < startTime + (overTime / speed))
		{
			float t = (Time.time - startTime) / (overTime / speed);
			transform.position = Vector3.Lerp(start, end, t);
			if(velocity > startVelocity) {
				startVelocity = velocity;
			}
			yield return null;
		}
		rigidbody.constraints = RigidbodyConstraints.None;
		RaycastHit hit = new RaycastHit();
		if(BallHasHit(out hit, (1 << 8), mRad*distToFind, 16, end)) {//Physics.SphereCast(new Ray(transform.position, transform.forward), mRad*1.5f, out hit, distToFind, (1 << 8))) {
			playerCatched = hit.transform;
			Player playerHitted = playerCatched.GetComponent<Player>();
			Player pFocused = SoccerManager.GetPlayers().Where(x => x.MyTeam == sm.whichIsMine).SingleOrDefault(x => x.isFocused == true);
			if(pFocused != null && !pFocused.hasBall) {
				pFocused.isFocused = false;
				if(!playerHitted.hasBall && !playerHitted.isFocused) {
					playerHitted.isFocused = true;
				}
			}	
		} else {
			rigidbody.velocity = end*((bot == true) ? 1 : (speed*velocity/startVelocity));
		}
		playerCatched = null;
		isShooted = true;
	}

	public bool BallHasHit(out RaycastHit hit, int lMask, float MAX_DISTANCE = 10, int DIRECTIONS_COUNT = 16, Vector3? nulpos = null, bool draw = false) {
		Vector3 pos = transform.position;
		if(nulpos != null) {
			pos = (Vector3)nulpos;
		}
		RaycastHit h = new RaycastHit();
		foreach (var direction in SphereCast.GetSphereDirections(DIRECTIONS_COUNT))
		{
			if(draw) Debug.DrawRay(transform.position, direction, Color.blue);
			if(Physics.Raycast(pos, direction, out h, MAX_DISTANCE, lMask)) {
				hit = h;
				return true;
			}
		}
		hit = h;
		return false;
	}

}
