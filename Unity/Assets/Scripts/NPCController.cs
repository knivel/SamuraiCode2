using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class NPCController : MonoBehaviour
{
	public GameObject player;
	public LayerMask RaycastLayerMask;
	public Gun gun;

	public GUIText label;
	GameObject labelGO;

	public float MoveSpeed;
	public float TurnSpeed;
	public float MinPlayerDetectDistance;
	public float FieldOfViewRange;

	public enum AIState
	{
		Initial,
		Idle,
		NoticePlayer,
		Chase,
		Search,
		Shoot
	};

	AIState _state = AIState.Initial;
	public AIState state
	{
		get { return _state; }
		set
		{
			if (value != _state)
			{
				endState();
				_state = value;
				beginState();
			}
		}
	}
	float stateTimer;
	float stateStartTime;

	Quaternion targetRotation;
	Vector3 lastKnownPlayerPosition;
	LineRenderer lineRenderer;

	public void Start()
	{
		labelGO = new GameObject();
		label = labelGO.AddComponent<GUIText>();
		label.anchor = TextAnchor.LowerCenter;
		ObjectLabel objLabel = labelGO.AddComponent<ObjectLabel>();
		objLabel.target = this.transform;
		objLabel.clampToScreen = true;

		lineRenderer = GetComponent<LineRenderer>();

		state = AIState.Idle;
	}
	
	public void Update()
	{
		updateState();
	}

	public void Kill()
	{
		Destroy(labelGO);
		Destroy(gameObject);
	}

	void beginState()
	{
		stateStartTime = Time.time;
		switch (_state)
		{
		case AIState.Idle: beginIdle(); break;
		case AIState.NoticePlayer: beginNoticePlayer(); break;
		case AIState.Chase: beginChase(); break;
		case AIState.Search: beginSearch(); break;
		case AIState.Shoot: beginShoot(); break;
		}
	}

	void updateState()
	{
		stateTimer = Time.time - stateStartTime;
		switch (_state)
		{
		case AIState.Idle: updateIdle(); break;
		case AIState.NoticePlayer: updateNoticePlayer(); break;
		case AIState.Chase: updateChase(); break;
		case AIState.Search: updateSearch(); break;
		case AIState.Shoot: updateShoot(); break;
		}
	}

	void endState()
	{
		switch (_state)
		{
		case AIState.Idle: endIdle(); break;
		case AIState.NoticePlayer: endNoticePlayer(); break;
		case AIState.Chase: endChase(); break;
		case AIState.Search: endSearch(); break;
		case AIState.Shoot: endShoot(); break;
		}
	}
	
	void beginIdle()
	{
		label.color = Color.green;
		GetComponent<Rigidbody>().velocity = Vector3.zero;
		targetRotation = Quaternion.Euler(0f, Random.Range(0, 360), 0f);
	}
	
	void updateIdle()
	{
		if (CanSeePlayer())
		{
			state = AIState.NoticePlayer;
		}

		LookAround(50f);
		
		label.text = string.Format("{0}\nIDLE...", name);
	}
	
	void endIdle()
	{
	}
	
	////////////////////////////////////////////////////////////
	
	void beginNoticePlayer()
	{
		label.color = Color.red;
		GetComponent<Rigidbody>().velocity = Vector3.zero;
		lineRenderer.enabled = true;
	}
	
	void updateNoticePlayer()
	{
		bool canSeePlayer = CanSeePlayer();
		if (stateTimer >= 0.75f)
		{
			if (canSeePlayer)
			    state = AIState.Shoot;
			else
			    state = AIState.Chase;
		}
		
		lineRenderer.SetPosition(0, transform.position);
		lineRenderer.SetPosition(1, lastKnownPlayerPosition);

		RotateTowardsPoint(lastKnownPlayerPosition, 10f);
		
		label.text = string.Format("{0}\nNOTICED PLAYER!", name);
	}
	
	void endNoticePlayer()
	{
		lineRenderer.enabled = false;
	}
	
	////////////////////////////////////////////////////////////

	void beginChase()
	{
		label.color = Color.red;
		lineRenderer.enabled = true;
	}

	void updateChase()
	{
		if (CanSeePlayer())
		{
			state = AIState.Shoot;
		}
		else if ((transform.position - lastKnownPlayerPosition).magnitude <= 0.1f)
		{
			state = AIState.Search;
		}

		RotateTowardsPoint(lastKnownPlayerPosition, 10f);
		MoveTowardsPoint(lastKnownPlayerPosition, MoveSpeed);

		lineRenderer.SetPosition(0, transform.position);
		lineRenderer.SetPosition(1, lastKnownPlayerPosition);

		label.text = string.Format("{0}\nCHASING...", name);
	}

	void endChase()
	{
		lineRenderer.enabled = false;
		GetComponent<Rigidbody>().velocity = Vector3.zero;
	}

	////////////////////////////////////////////////////////////

	void beginShoot()
	{
		lineRenderer.enabled = true;
		label.color = Color.red;
	}

	void updateShoot()
	{
		if (!CanSeePlayer())
		{
			state = AIState.Chase;
		}

		RotateTowardsPoint(lastKnownPlayerPosition, 20f);
		MoveTowardsPoint(lastKnownPlayerPosition, MoveSpeed * 0.5f);

		Gun gun = GetComponent<NPCController>().gun;

		if (gun.IsReloading)
		{
			lineRenderer.enabled = true;
		}
		else
		{
			lineRenderer.enabled = false;
			gun.Shoot();
		}

		lineRenderer.SetPosition(0, transform.position);
		lineRenderer.SetPosition(1, lastKnownPlayerPosition);
		
		label.text = string.Format("{0}\nSHOOTING...", name);
	}

	void endShoot()
	{
		lineRenderer.enabled = false;
		GetComponent<Rigidbody>().velocity = Vector3.zero;
	}
	
	////////////////////////////////////////////////////////////
	
	void beginSearch()
	{
		label.color = Color.yellow;
		GetComponent<Rigidbody>().velocity = Vector3.zero;
		targetRotation = Quaternion.Euler(0f, Random.Range(0, 360), 0f);
	}
	
	void updateSearch()
	{
		if (CanSeePlayer())
		{
			state = AIState.Shoot;
		}
		
		LookAroundRandomly(2f);
		
		if (stateTimer >= 3f)
		{
			state = AIState.Idle;
		}
		
		label.text = string.Format("{0}\nSEARCHING... {1}", name, stateTimer.ToString("F2"));
	}
	
	void endSearch()
	{
	}

	////////////////////////////////////////////////////////////
	
	void LookAround(float Speed)
	{
		transform.Rotate(0f, Speed * Time.deltaTime, 0f);
	}

	void LookAroundRandomly(float Speed)
	{
		transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Speed);
		transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);

		if (Quaternion.Angle(transform.rotation, targetRotation) <= 1f)
			targetRotation = Quaternion.Euler(0f, Random.Range(0, 360), 0f);
	}

	void RotateTowardsPoint(Vector3 Point, float Speed)
	{
		Quaternion targetRotation = Quaternion.LookRotation(Point - transform.position);

		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Speed * Time.deltaTime);
	}

	void MoveTowardsPoint(Vector3 Point, float Speed)
	{
		Vector3 vel = GetComponent<Rigidbody>().velocity;
		Vector3 moveDir = Point - transform.position;
		
		vel = moveDir.normalized * Speed;

		GetComponent<Rigidbody>().velocity = vel;
	}

	bool CanSeePlayer()
	{
		RaycastHit hit;
		Vector3 rayDirection = player.transform.position - transform.position;
		float distanceToPlayer = rayDirection.magnitude;
		
		if (distanceToPlayer <= MinPlayerDetectDistance)
		{
			lastKnownPlayerPosition = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);
		//	Debug.Log("Player close!");
			return true;
		}

		if ((Vector3.Angle(rayDirection, transform.forward)) < FieldOfViewRange)
		{
			if (Physics.Raycast (transform.position, rayDirection, out hit, RaycastLayerMask)) 
			{
				if (hit.transform.tag == "Player") 
				{
					lastKnownPlayerPosition = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);
				//	Debug.Log("Can see player.");
					return true;
				}
				else
				{
				//	Debug.Log("Something in the way.");
					return false;
				}
			}
		}
		
	//	Debug.Log("Can't see player.");
		return false;
	}

	bool HasLineOfSightToPlayer()
	{
		RaycastHit hit;
		Vector3 rayDirection = player.transform.position - transform.position;

		if (Physics.Raycast (transform.position, rayDirection, out hit) && (hit.transform.tag == "Player"))
		{	
			lastKnownPlayerPosition = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);
			return true;
		}
		else
		{
			return false;
		}
	}
}