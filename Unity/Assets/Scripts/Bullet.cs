using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour 
{
	public float Damage;
	public float Speed;
	public float Lifespan;

	private float age;
	
	void Start () 
	{
		Shoot();
	}

	void Update () 
	{
		age += Time.deltaTime;
		if (age >= Lifespan)
			Kill();
	}

	public void Shoot()
	{
		Vector3 vector = transform.forward * Speed * Random.Range(0.95f, 1.05f);

		vector = Quaternion.Euler(0, Random.Range(-5, 5), 0) * vector;

		GetComponent<Rigidbody>().AddForce (vector);

		age = 0f;
	}

	public void Kill()
	{
		GetComponent<Rigidbody>().velocity = Vector3.zero;
		GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
		this.Recycle ();
	}

	void OnCollisionEnter(Collision collision)
	{
		GameObject other = collision.gameObject;

		if (other.GetComponent<Block> () != null) 
		{
			other.GetComponent<Block>().HP -= Damage;
		}

		Kill();
	}
}
