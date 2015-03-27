using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{
	public Bullet bulletPrefab;
	public float ReloadTime;
	public float KickbackStrength;
	float lastShotTime = 0f;

	void Start()
	{
		bulletPrefab.CreatePool();
	}

	void Update()
	{
		if (Input.GetButton("Fire1")) {
			Shoot();
		}
	}

	public void Shoot()
	{
		if (!IsReloading)
		{
			for (int i = 0; i < 1; i++)
			{
				Bullet bullet = bulletPrefab.Spawn(transform.TransformPoint(Vector3.zero), transform.rotation);
				bullet.Shoot();
			}

			Rigidbody parentRigidBody = transform.parent.GetComponent<Rigidbody>();
			if (parentRigidBody != null) {
				parentRigidBody.AddForce(-transform.forward * KickbackStrength);
			}
			lastShotTime = Time.time;
		}
	}

	public bool IsReloading
	{
		get
		{
			if (Time.time - lastShotTime > ReloadTime)
				return false;
			else
				return true;
		}
	}
}
