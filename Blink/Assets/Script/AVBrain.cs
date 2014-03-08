using UnityEngine;
using System.Collections;

public class AVBrain : MonoBehaviour {

	public float FieldOfViewAngle = 180f;
	public float SightRange = 15f;
	public float TargetRange = 10f;

	public float PatrolSpeed = 5f;
	public float PatrolRange = 20f;
	public float MoveSpeed = 10f;
	public Vector3 StartPosition;

	public GameObject player;

	private FSMSystem fsm;

	public void SetTransition(Transition t) {
		fsm.PerformTransition (t);
	}

	// Use this for initialization
	void Start () {
		StartPosition = transform.position;
		MakeFSM ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		fsm.CurrentState.Reason (player, gameObject);
		fsm.CurrentState.Act(player, gameObject);
	}

	private void MakeFSM(){
		// Create instances of the states
		PatrolState patrol = new PatrolState (gameObject);
		patrol.AddTransition (Transition.SawEnemy, StateID.Attack);

		AttackState attack = new AttackState (gameObject);
		attack.AddTransition (Transition.LostEnemy, StateID.Chase);
		attack.AddTransition (Transition.EnemyDead, StateID.Patrol);

		ChaseState chase = new ChaseState (gameObject);
		chase.AddTransition (Transition.CaughtUp, StateID.Attack);
		chase.AddTransition (Transition.CouldNotChase, StateID.Patrol);

		// Instance and init of FSMSystem
		fsm = new FSMSystem ();
		// Add states to the FSM
		fsm.AddState(patrol);
		fsm.AddState(attack);
		fsm.AddState(chase);
	}

	void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.tag == "Edge" || 
		    collision.gameObject.tag == "Wall") {
				fsm.CurrentState.OnCollide();
		}
	}
}

public class PatrolState : FSMState {

	private Vector3 npcPatrolDirection;
	private Vector3 npcMoveLocation;
	private float npcPatrolSpeed;
	private float npcPatrolRange;
	private Transform npcTransform;
	private float npcFieldOfViewAngle;
	private float npcSightRange;
	private Vector3 npcStartPosition;

	public PatrolState(GameObject npc){
		stateID = StateID.Patrol;

		npcPatrolSpeed = npc.GetComponent<AVBrain> ().PatrolSpeed;
		npcFieldOfViewAngle = npc.GetComponent<AVBrain> ().FieldOfViewAngle;
		npcSightRange = npc.GetComponent<AVBrain> ().SightRange;
		npcStartPosition = npc.GetComponent<AVBrain> ().StartPosition;
		npcPatrolRange = npc.GetComponent<AVBrain> ().PatrolRange;

		int direction_choice = Mathf.CeilToInt(Random.Range (0, 99)) % 2;
		npcPatrolDirection = (direction_choice == 1)? npc.transform.right: -npc.transform.right;

		npcMoveLocation = new Vector3 (npcStartPosition.x + (npcPatrolRange * npcPatrolDirection.x),
		                              npcStartPosition.y, 0);
	} // PatrolState() end

	public override void Reason (GameObject player, GameObject npc) {
		Vector3 direction = (player.transform.position - npc.transform.position).normalized;
		float angle = Vector3.Angle(direction, npc.transform.forward);
		
		if (angle <= npcFieldOfViewAngle) {
			// If in field of view
			RaycastHit hit;
			// and can see the enemy
			if (Physics.Raycast(npc.transform.position, direction, out hit, npcSightRange)) {
				if (hit.collider.gameObject == player) {
					npc.GetComponent<AVBrain>().SetTransition(Transition.SawEnemy);
				}
			}
		} 
	} // Reason() end

	public override void Act(GameObject player, GameObject npc) {
		// Move in the patrol direction then go back
		// to the start position, and repeat.
		Vector3 vel = npc.rigidbody.velocity;
		Vector3 MoveDistance = npcMoveLocation - npc.transform.position;

		// If near the move location, set the move location to start position
		// else move toward point
		if (MoveDistance.magnitude < 1) {
			if (npcMoveLocation == npcStartPosition) {
				// Go to new move position
				npcMoveLocation = new Vector3(npcStartPosition.x + (npcPatrolRange * npcPatrolDirection.x),
				                              npcStartPosition.y, 0);
				// TODO: make npc take a short pause before moving out again
			} else {
				npcPatrolDirection = (npcStartPosition - npc.transform.position).normalized;
				npcMoveLocation = npcStartPosition;
			}
		} else {
			vel = npcPatrolDirection * npcPatrolSpeed;
		}
		npc.rigidbody.velocity = vel;
	} // Act() end

	//public void DoBeforeEntering() {
	//	ResetPatrol ();
	//} // DoBeforeLeaving() end

	public override void OnCollide() {
		npcPatrolDirection = -npcPatrolDirection;
		npcMoveLocation = npcStartPosition;
	} // OnCollide() end

	public override void DoBeforeEntering() {
		Debug.Log ("PatrolState");
	}
} // class PatrolState end

public class AttackState : FSMState {

	private float npcFireRange;
	private float npcSightRange;
	private float npcFieldOfViewAngle;
	private float npcMoveSpeed;

	public AttackState(GameObject npc) {
		npcFieldOfViewAngle = npc.GetComponent<AVBrain> ().FieldOfViewAngle;
		npcSightRange = npc.GetComponent<AVBrain> ().SightRange;
		npcFireRange = npc.GetComponent<AVBrain> ().TargetRange;
		npcMoveSpeed = npc.GetComponent<AVBrain> ().PatrolSpeed * 1.5f;
		stateID = StateID.Attack;
	} // AttackState() end

	public override void Reason (GameObject player, GameObject npc) {
		// if the enemy is more than sight range + 5, fire LostEnemy transition
		// or outside of the field of view
		Vector3 distance = player.transform.position - npc.transform.position;
		Vector3 direction = distance.normalized;
		float angle = Vector3.Angle(distance, npc.transform.forward);

		// If in field of view
		RaycastHit hit;
		// and can see the enemy
		if (Physics.Raycast(npc.transform.position, direction, out hit, npcSightRange + 5f) ||
		    angle > npcFieldOfViewAngle) {
				if (hit.collider.gameObject != player) {
					npc.GetComponent<AVBrain>().SetTransition(Transition.LostEnemy);
				}
		}
		// If the enemy is dead, fire EnemyDead transition
		// TODO: implement character health
	} // Reason() end
	
	public override void Act(GameObject player, GameObject npc) {
		Transform TurretTransform = npc.transform.GetChild(0);
		Vector3 direction = player.transform.position - TurretTransform.position;
		float distance = direction.magnitude;

		if (distance < npcFireRange) {
			// If in firing range check if enemy is in field on view
			float angle = Vector3.Angle(direction, TurretTransform.forward);
			npc.rigidbody.velocity = new Vector3(0f, 0f, 0f);

			if (angle <= npcFieldOfViewAngle) {
				// If in field of view, fire
				RaycastHit hit;

				if (Physics.Raycast(TurretTransform.position, direction.normalized, out hit, npcFireRange)) {
					if (hit.collider.gameObject == player) {
						Debug.Log("pew");
					}
				}
			} 
		} else {
			// else move to target
			npc.rigidbody.velocity = (player.transform.position - npc.transform.position).normalized * npcMoveSpeed;
		}
	} // Act() end

	public override void OnCollide() {
		// Do something
	} // OnCollide() end

	public override void DoBeforeEntering() {
		Debug.Log ("AttackState");
	}
} // class AttackState end

public class ChaseState : FSMState {
	private float npcSightRange;
	private float npcMoveSpeed;
	private float npcFieldOfViewAngle;

	public ChaseState(GameObject npc) {
		stateID = StateID.Chase;

		npcSightRange = npc.GetComponent<AVBrain> ().SightRange;
		npcMoveSpeed = npc.GetComponent<AVBrain> ().PatrolSpeed * 1.5f;
		npcFieldOfViewAngle = npc.GetComponent<AVBrain> ().FieldOfViewAngle;
	} // ChaseState() end

	public override void Reason (GameObject player, GameObject npc) {
		Vector3 distance = player.transform.position - npc.transform.position;
		Vector3 direction = distance.normalized;
		float angle = Vector3.Angle(distance, npc.transform.forward);
		
		// If in field of view
		RaycastHit hit;
		// and can see the enemy
		if (Physics.Raycast (npc.transform.position, direction, out hit, npcSightRange + 7f) ||
			angle > npcFieldOfViewAngle) {
			if (hit.collider.gameObject != player) {
				npc.GetComponent<AVBrain> ().SetTransition (Transition.CouldNotChase);
			}
		} else {
			npc.GetComponent<AVBrain> ().SetTransition (Transition.CaughtUp);
		}
	} // Reason() end
	
	public override void Act(GameObject player, GameObject npc) {
		Vector3 direction = (player.transform.position - npc.transform.position).normalized;
		float angle = Vector3.Angle(direction, npc.transform.forward);
		
		if (angle <= npcFieldOfViewAngle) {
			// If in field of view
			RaycastHit hit;
			// and can see the enemy
			if (Physics.Raycast(npc.transform.position, direction, out hit, npcSightRange)) {
				if (hit.collider.gameObject == player) {
					// close in
					npc.rigidbody.velocity = direction * npcMoveSpeed;
				}
			}
		} 
	} // Act() end

	public override void OnCollide() {
		// Do something
	} // OnCollide() end

	public override void DoBeforeEntering() {
		Debug.Log ("ChaseState");
	}
} // class ChaseState end
