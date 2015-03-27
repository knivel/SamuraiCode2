using UnityEngine;
using System.Collections;

public class Sword : MonoBehaviour 
{
	public float Damage;

	public Vector3 SwordPosition;
	public Vector3 SwordScale;
	public float SwordOpacity;
	public float SecondsToSword;
	
	public Vector3 ShieldPosition;
	public Vector3 ShieldScale;
	public float ShieldOpacity;
	public float SecondsToShield;
	
	public enum SwordState
	{
		Sheathed,
		Drawing,
		Drawn,
		Sheathing
	};

	SwordState _state = SwordState.Sheathed;
	public SwordState state
	{
		get { return _state; }
	}

	TrailRenderer trailRenderer;

	Vector3 lastPosition;
	Vector3 lastVelocity;

	float timer;

	void Start()
	{
		trailRenderer = GetComponent<TrailRenderer>();

		StartSheathed();

		lastPosition = transform.position;
		lastVelocity = Vector3.zero;

		GetComponent<ParticleSystem>().Stop();
	}

	void Update()
	{
		switch (state)
		{
		case SwordState.Sheathed: UpdateShield(); break;
		case SwordState.Drawing: UpdateDrawing(); break;
		case SwordState.Drawn: UpdateDrawn(); break;
		case SwordState.Sheathing: UpdateSheathing(); break;
		}

		lastVelocity = transform.position - lastPosition;
		lastPosition = transform.position;
	}
	
	public void Extend()
	{
		if(( state != SwordState.Drawing )&&( state != SwordState.Drawn))
			StartDrawing();
	}
	
	public void Retract()
	{
		if(( state != SwordState.Sheathing )&&( state != SwordState.Sheathed))
			StartSheathing();
	}

	void StartSheathed()
	{
		_state = SwordState.Sheathed;
		transform.localScale = ShieldScale;
		transform.localPosition = ShieldPosition;
		SetOpacity(ShieldOpacity);

		trailRenderer.startWidth = 0f;
	}

	void UpdateShield()
	{
	}
	
	void StartDrawing()
	{
		_state = SwordState.Drawing;
		timer = 0;
	}
	
	void UpdateDrawing()
	{
		transform.localScale = Vector3.Lerp(transform.localScale, SwordScale, timer / SecondsToSword);
		transform.localPosition = Vector3.Lerp(transform.localPosition, SwordPosition, timer / SecondsToSword);
		SetOpacity(Mathf.Lerp(GetOpacity(), SwordOpacity, timer / SecondsToSword));
		trailRenderer.startWidth = transform.localScale.z;

		timer += Time.deltaTime;

		if (timer >= SecondsToSword)
			StartDrawn();
	}
	
	void StartDrawn()
	{
		_state = SwordState.Drawn;
		transform.localScale = SwordScale;
		transform.localPosition = SwordPosition;
		SetOpacity(SwordOpacity);
		trailRenderer.startWidth = transform.localScale.z;
	}
	
	void UpdateDrawn()
	{
	}
	
	void StartSheathing()
	{
		_state = SwordState.Sheathing;
		timer = 0;
	}
	
	void UpdateSheathing()
	{
		transform.localScale = Vector3.Lerp(transform.localScale, ShieldScale, timer / SecondsToShield);
		transform.localPosition = Vector3.Lerp(transform.localPosition, ShieldPosition, timer / SecondsToShield);
		SetOpacity(Mathf.Lerp(GetOpacity(), ShieldOpacity, timer / SecondsToShield));
		trailRenderer.startWidth = transform.localScale.z;
		
		timer += Time.deltaTime;
		
		if (timer >= SecondsToShield)
			StartSheathed();
	}
	
	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.GetComponent<Block> () != null && state == SwordState.Drawn) 
		{
			float totalDamage = Damage * Time.deltaTime + lastVelocity.magnitude / Time.deltaTime;
			GetComponent<ParticleSystem>().Emit(Mathf.RoundToInt(totalDamage));
			other.gameObject.GetComponent<Block>().HP -= totalDamage;
		}
		
		if (other.gameObject.GetComponent<Bullet>() != null && state == SwordState.Sheathed)
		{
			other.gameObject.GetComponent<Bullet>().Kill();
		}
		
		if (other.gameObject.GetComponent<NPCController>() != null && state == SwordState.Drawn)
		{
			other.gameObject.GetComponent<NPCController>().Kill();
		}
	}
	
	void OnTriggerStay(Collider other)
	{
		if (other.gameObject.GetComponent<Block> () != null && state == SwordState.Drawn) 
		{
			float totalDamage = Damage * Time.deltaTime;
			GetComponent<ParticleSystem>().Emit(Mathf.RoundToInt(Damage));
			other.gameObject.GetComponent<Block>().HP -= totalDamage;
		}

		if (other.gameObject.GetComponent<Bullet>() != null && state == SwordState.Sheathed)
		{
			other.gameObject.GetComponent<Bullet>().Kill();
		}
		
		if (other.gameObject.GetComponent<NPCController>() != null && state == SwordState.Drawn)
		{
			other.gameObject.GetComponent<NPCController>().Kill();
		}
	}

	void SetOpacity(float Value)
	{
		Color tempColor = GetComponent<Renderer>().material.color;
		tempColor.a = Value;
		GetComponent<Renderer>().material.color = tempColor;
	}
		
	float GetOpacity()
	{
		return GetComponent<Renderer>().material.color.a;
	}
}
