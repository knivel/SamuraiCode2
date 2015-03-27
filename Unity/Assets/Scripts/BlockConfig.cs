using UnityEngine;
using System.Collections;

[System.Serializable]
public class BlockConfig 
{
	public string Name;

	public enum Geo { Floor, Wall, Pit, Box }
	public Geo Geometry = Geo.Floor;

	public Texture2D TextureMap;
	public Color BaseColor;
	
	public float HP;
	public bool IsInvincible;
	public bool IsTrigger;

	public string MutateToOnDeath;
}
