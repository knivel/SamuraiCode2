using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour 
{
	public float ScrollSpeed;

	private Vector3 targetPosition;
	private Vector3 offset;
	private Transform target;

	void Start () 
	{
		target = GameObject.FindGameObjectWithTag("Player").transform;
		offset = new Vector3(transform.position.x - target.position.x, 
		                     transform.position.y - target.position.y, 
		                     transform.position.z - target.position.z);
	}

	void Update () 
	{
		targetPosition = new Vector3(target.position.x + offset.x, target.position.y + offset.y, target.position.z + offset.z);
		transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * ScrollSpeed);
	}
}
