using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour {

	public float speedMovement = 2, jumpSpeed = 8.0f, gravity = 20.0f, distToGround = 1.05f;

	public bool onlyClick;

	public Transform ball;

	private Vector3 moveDirection = Vector3.zero;
	private float jumpTimer, heighbtBall;

	private Vector3 playerpos, velocity, lastPosition, curPosition;

	Player pl;

	SoccerManager sm;

	bool IsGrounded() {
		return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
	}

	void Start() {
		ball = GameObject.Find("Ball").transform;
		heighbtBall = ball.position.y - transform.position.y;
		pl = GetComponent<Player>();
		sm = GameObject.Find("MainScripts").GetComponent<SoccerManager>();
	}
	
	// Update is called once per frame
	void Update() {

		if(pl.isFocused) {

			if((!onlyClick || Input.GetMouseButton(0)) && pl.MyTeam == sm.whichIsMine) {
				
				Plane playerPlane = new Plane(Vector3.up, transform.position);
				
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				
				float hitdist = 0.0f;
				
				if (playerPlane.Raycast (ray, out hitdist)) 
				{
					Vector3 targetPoint = ray.GetPoint(hitdist);
					transform.position = Vector3.MoveTowards(transform.position, targetPoint, Time.deltaTime * speedMovement);
					Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position);
					transform.eulerAngles = Vector3.up * 90 - Quaternion.Slerp(transform.rotation, targetRotation, speedMovement * Time.time).eulerAngles;
				}

			}

			if (IsGrounded())
			{
				// We are grounded, so recalculate movedirection directly from axes
				//moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
				moveDirection = transform.TransformDirection(Vector3.zero);
				moveDirection *= speedMovement;

				if(Input.GetButton("Jump")) {
					moveDirection.y = jumpSpeed;
				}

			}
			
			// Apply gravity
			moveDirection.y -= gravity * Time.deltaTime;
			
			// Move the controller
			CharacterController controller = (CharacterController)GetComponent(typeof(CharacterController));
			controller.Move(moveDirection * Time.deltaTime);

		}

	}

	void FixedUpdate() {
		heighbtBall = ball.position.y - transform.position.y;
		curPosition = transform.position;
		velocity = (lastPosition - curPosition) / Time.deltaTime;
		lastPosition = transform.position;
	}

}
