using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour
{
	public float MaxHP;

	private float _hp;
	
	void Start () 
	{
		_hp = MaxHP;
	}

	public float HP
	{
		get { return _hp; }
		set
		{
			_hp = Mathf.Min (MaxHP, Mathf.Max(0, value));

			if (_hp == 0)
				Kill();
		}
	}

	public void Kill()
	{
		Destroy(gameObject);
	}
}
