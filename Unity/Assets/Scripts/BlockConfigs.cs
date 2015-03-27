using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlockConfigs : MonoBehaviour
{
	public List<BlockConfig> Configs;
	static BlockConfigs instance = null;
	
	public static BlockConfigs Instance
	{
		get {
			if (instance == null)
				instance = GameObject.FindObjectOfType<BlockConfigs>();
			
			return instance;
		}
	}

	public BlockConfig GetByIndex(int Index)
	{
		if (Index >= 0 && Index < Configs.Count)
			return Configs[Index];
		else
			return null;
	}
	
	public BlockConfig GetByName(string Name)
	{
		for (int i = 0; i < Configs.Count; i++)
		{
			if (Configs[i].Name == Name)
				return Configs[i];
		}

		return new BlockConfig();
	}
}
