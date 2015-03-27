using UnityEngine;
using System.Collections;

public class World : MonoBehaviour
{
	public int Length = 128;
	public int Width = 128;

	public float ScaleX = 1f;
	public float ScaleY = 1f;
	public float ScaleZ = 1f;
	
	public float distToLoad = 32f;
	public float distToUnload = 48f;
	public float loadFrequency = 1f;

	public GameObject Player;

	public Block BlockPrefab;
	public Block[,] Blocks;

	private string[,] Data;

	private float loadTimer;

	void Start () 
	{
		Data = new string[Length, Width];

		// random data
		for (int x = 0; x < Length; x++)
		{
			for (int z = 0; z < Width; z++)
			{
				float noise = PerlinNoise(x, 0, z, 10, 2, 1);

				if (noise >= 0.8f)
				{
					Data[x, z] = "Wallbasic";
				}
				else if (Random.Range(0, 10) >= 9)
				{
					Data[x, z] = "Pit";
				}
				else
				{
					Data[x, z] = "FloorBasic";
				}
			}
		}

		Blocks = new Block[Length, Width];
		BlockPrefab.CreatePool ();

		UpdateAllBlocks(true);
		loadTimer = loadFrequency;
	}

/*	void Update()
	{
		loadTimer -= Time.deltaTime;
		if (loadTimer <= 0)
		{
			UpdateAllBlocks(true);
			loadTimer += loadFrequency;
		}
	}
*/
	public void UpdateAllBlocks(bool ignoreDistance = false)
	{
		for(int x = 0 ; x < Blocks.GetLength(0); x++)
		{
			for(int z = 0 ; z < Blocks.GetLength(1); z++)
			{
				float dist = Vector3.Distance(BlockPosition(x, z), Player.transform.position);
				
				if (ignoreDistance || dist < distToLoad)
				{
					LoadBlock(x, z);
				} 
				else if(dist > distToUnload)
				{
					UnloadBlock(x, z);
				}
			}
		}
	}

	public void LoadBlock(int x, int z)
	{
		Block newBlock = BlockPrefab.Spawn(BlockPosition(x, z), new Quaternion(0, 0, 0, 0));

		newBlock.transform.parent = gameObject.transform;
		Blocks[x, z] = newBlock.GetComponent("Block") as Block;
		Blocks[x, z].worldGO = gameObject;
		Blocks[x, z].GridX = x;
		Blocks[x, z].GridZ = z;
		Blocks[x, z].Config = BlockConfigs.Instance.GetByName(DataAt(x, z));
	}
	
	public void UnloadBlock(int x, int z)
	{
		if( x >= Length || x < 0 || z >= Width || z < 0)
		{
			return;
		}

		Blocks [x, z].Recycle ();
	}
	
	public void UpdateBlockGeometry(int x, int z)
	{
		if( x >= Length || x < 0 || z >= Width || z < 0)
		{
			return;
		}

		Blocks [x, z].UpdateGeometry();
	}
	
	public void UpdateNeighboringBlockGeometry(int x, int z)
	{
		UpdateBlockGeometry(x - 1, z);
		UpdateBlockGeometry(x + 1, z);
		UpdateBlockGeometry(x, z - 1);
		UpdateBlockGeometry(x, z + 1);
	}
	
	public string DataAt(int x, int z)
	{
		if( x >= Length || x < 0 || z >= Width || z < 0)
		{
			return null;
		}
		
		return Data[x, z];
	}
	
	public Block BlockAt(int x, int z)
	{
		if( x >= Length || x < 0 || z >= Width || z < 0)
		{
			return null;
		}
		
		return Blocks[x, z];
	}
	
	Vector3 BlockPosition(int x, int z)
	{
		Vector3 blockPosition = new Vector3(transform.position.x + (float)x * ScaleX + ScaleX / 2,
		                               		transform.position.y + ScaleY / 2,
		                                    transform.position.z + (float)z * ScaleZ + ScaleZ / 2);
		
		return blockPosition;
	}

	int PerlinNoise(int x, int y, int z, float scale, float height, float power)
	{
		float rValue;
		rValue = Noise.GetNoise (((double)x) / scale, ((double)y) / scale, ((double)z) / scale);
		rValue *= height;
		
		if(power != 0)
		{
			rValue = Mathf.Pow( rValue, power);
		}
		
		return (int) rValue;
	}
}
