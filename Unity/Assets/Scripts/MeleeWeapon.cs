using UnityEngine;
using System.Collections;

public class MeleeWeapon : MonoBehaviour 
{
	public float Damage;
	public float AnimationTime;

	BoxCollider collider;
	GameObject flash;
	float hitStartTime;
	bool isAnimating = false;

	void Start () 
	{
		collider = GetComponent<BoxCollider>();
		collider.enabled = false;

		flash = transform.Find("Flash").gameObject;

		gameObject.SetActive(false);
		
		GetComponent<ParticleSystem>().Stop();
	}
	
	void Update () 
	{
		float age = Time.time - hitStartTime;
		float scale = 1 - (age / AnimationTime);

		flash.transform.localScale = new Vector3(scale, scale, scale);
		flash.transform.Rotate(Random.Range(0, 30), Random.Range(0, 30), Random.Range(0, 30));

		if (age >= AnimationTime)
		{
			isAnimating = false;
			gameObject.SetActive(false);
			collider.enabled = false;
		}
	}
	
	public void Hit(Vector3 Position)
	{
		if (!isAnimating)
		{
			gameObject.SetActive(true);
			transform.position = Position;
			hitStartTime = Time.time;
			collider.enabled = true;
			isAnimating = true;
			flash.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
		}
	}

	void OnTriggerEnter(Collider other)
	{
		GetComponent<ParticleSystem>().Emit(Mathf.RoundToInt(Damage));

		if (other.gameObject.GetComponent<Block> () != null) 
		{
			other.gameObject.GetComponent<Block>().HP -= Damage;
		}
		
		if (other.gameObject.GetComponent<NPCController>() != null)
		{
			other.gameObject.GetComponent<NPCController>().Kill();
		}
	}
}
