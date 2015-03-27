using UnityEngine;
using System.Collections;

public class EnemyManager : MonoBehaviour
{
	public GameObject EnemyPrefab;
	public int EnemyCount;
	public GameObject Player;

	void Start () 
	{
		for (int i = 0; i < EnemyCount; i++)
		{
			Vector3 randomPosition = new Vector3(Random.Range(0f, 128f), 1f, Random.Range(0f, 128f));
			GameObject obj = (GameObject)Instantiate(EnemyPrefab, randomPosition, Quaternion.identity);
			obj.GetComponent<NPCController>().player = Player;
		}
	}
}
