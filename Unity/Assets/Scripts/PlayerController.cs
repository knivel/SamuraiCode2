using UnityEngine;
using System.Collections;

[RequireComponent (typeof (CharacterController))]
public class PlayerController : MonoBehaviour 
{
	public float RotationSpeed;
	public float MoveSpeed;
	public float Acceleration;

	public Transform Turret;
	public Sword sword;
	public MeleeWeapon meleeWeapon;
	public TargetZone targetZone;
	
	public GUIText label;
	GameObject labelGO;

	Quaternion targetRotation;
	CharacterController controller;
	Vector3 currentVelocityMod;
	float startY;

	void Start ()
	{
		controller = GetComponent<CharacterController>();
		startY = transform.position.y;

		labelGO = new GameObject();
		label = labelGO.AddComponent<GUIText>();
		label.anchor = TextAnchor.LowerCenter;
		ObjectLabel objLabel = labelGO.AddComponent<ObjectLabel>();
		objLabel.target = this.transform;
		objLabel.clampToScreen = true;
		label.color = Color.gray;
	}

	void Update () 
	{
		HandleControls();
		transform.position.Set(transform.position.x, startY, transform.position.z);

		label.text = string.Format("pos: {0}\ntgtZone: {1}", transform.position, targetZone.transform.position);
	}

	void HandleControls()
	{
		Plane plane = new Plane(Vector3.up, transform.position);
		float swordX = Input.GetAxis( "SwordStickX" );
		float swordY = Input.GetAxis( "SwordStickY" );
		float mouseX = Input.GetAxis( "Mouse X");
		float mouseY = Input.GetAxis( "Mouse Y");

		if(( Mathf.Abs( swordX ) > Mathf.Epsilon ) || (Mathf.Abs( swordY ) > Mathf.Epsilon ))
		{
			float rotation = Mathf.Atan2( swordY, swordX );
			Turret.rotation = Quaternion.Euler( new Vector3( 0f, Mathf.Rad2Deg * rotation + 90f, 0f ));

			targetZone.TargetPosition = new Vector3(swordX, 0f, -swordY) * 10f;
		}
		else if (( Mathf.Abs( mouseX ) > Mathf.Epsilon ) || (Mathf.Abs( mouseY ) > Mathf.Epsilon ))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			float hit;
			if (plane.Raycast (ray, out hit)) 
			{
				Vector3 aimDirection = Vector3.Normalize (ray.GetPoint (hit) - transform.position);
				Quaternion targetRotation = Quaternion.LookRotation (aimDirection);
				Turret.rotation = Quaternion.Euler(new Vector3(0f, targetRotation.eulerAngles.y, 0f));

				targetZone.TargetPosition = ray.GetPoint (hit) - transform.position;
			}
		}
//
//		if (( Mathf.Abs( swordX ) > 0.3f )||(Mathf.Abs( swordY ) > 0.3f )
//		   || (Input.GetButton("Fire1")))
//		{
//			sword.Extend();
//		}
//		else
//		{
//			sword.Retract();
//		}
//
//		if (Input.GetButtonDown("Fire1") && meleeWeapon.enabled)
//	    {
//			meleeWeapon.Hit(targetZone.transform.position);
//		}

		Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
		currentVelocityMod = Vector3.MoveTowards(currentVelocityMod, input, Acceleration * Time.deltaTime);
		Vector3 motion = currentVelocityMod;
		motion *= (Mathf.Abs(input.x) == 1 && Mathf.Abs(input.z) == 1) ? 0.7f : 1.0f;
		motion *= MoveSpeed;

		controller.Move(motion * Time.deltaTime);
	}
}
