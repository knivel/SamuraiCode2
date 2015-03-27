using UnityEngine;
using System.Collections;

public class TargetZone : MonoBehaviour 
{
	public float MaxDistance;
	public float MinDistance;

	GameObject reticule;
	float reticuleProjectionHeight = 5f;
	public LayerMask ReticuleProjectionLayerMask;
	
	Vector3 _targetPosition;
	public Vector3 TargetPosition
	{
		get { return _targetPosition; }
		set
		{
			_targetPosition = value;
			CalculatePosition();
		}
	}

	void Start()
	{
		reticule = transform.Find("Reticule").gameObject;
	}

	void Update()
	{
		RaycastHit hit;
		Vector3 rayDirection = Vector3.down;
		Vector3 rayOrigin = new Vector3(transform.position.x, reticuleProjectionHeight, transform.position.z);
		float reticuleHeight;
		
		if (Physics.Raycast (rayOrigin, rayDirection, out hit, reticuleProjectionHeight, ReticuleProjectionLayerMask)) 
		{
			reticuleHeight = hit.point.y + 0.1f;
		}
		else
		{
			reticuleHeight = 0.1f;
		}

		reticule.transform.position = new Vector3(reticule.transform.position.x, reticuleHeight, reticule.transform.position.z);

		//Debug.DrawLine(transform.parent.position, transform.parent.position + TargetPosition, Color.red);
	}

	void CalculatePosition()
	{
		Quaternion rotation = Quaternion.LookRotation(TargetPosition);
		float distance = Mathf.Clamp(Vector3.Magnitude(TargetPosition), MinDistance, MaxDistance);
		Vector3 forward = Vector3.forward * distance;
		transform.position = rotation * forward;
		transform.position += transform.parent.position;
	}

	void Show()
	{
		gameObject.SetActive(true);
	}

	void Hide()
	{
		gameObject.SetActive(false);
	}
}
