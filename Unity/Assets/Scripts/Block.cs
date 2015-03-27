using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Block : MonoBehaviour
{
	[HideInInspector]
	public GameObject worldGO;
	
	[HideInInspector]
	public int GridX;
	[HideInInspector]
	public int GridZ;

	private bool updateGeometry;

	private World world;
	private BlockConfig config;

	private float hp;

	private List<Vector3> newVertices = new List<Vector3>();
	private List<int> newTriangles = new List<int>();
	private List<Vector2> newUV = new List<Vector2>();

	private float textureUnit = 0.5f;
	private Vector2 textureTop = new Vector2 (0, 1);
	private Vector2 textureSide = new Vector2 (1, 1);
	private Vector2 textureBottom = new Vector2 (0, 0);
	
	private Mesh mesh;
	private Bounds bounds;
	
	private int faceCount;
		
	public BlockConfig Config
	{
		get { return config; }
		set
		{
			config = value;
			InitFromConfig();
			UpdateGeometry();
		}
	}

	void Start () 
	{
		world = worldGO.GetComponent("World") as World;

		mesh = GetComponent<MeshFilter> ().mesh;
	}

	void Update()
	{
		
	}

	void InitFromConfig()
	{
		hp = config.HP;
	}

	public float HP
	{
		get { return hp; }
		set 
		{ 
			hp = Mathf.Max (0, Mathf.Min (config.HP, value));
			if (hp == 0 && !config.IsInvincible)
				Kill ();
		}
	}

	public void UpdateGeometry()
	{
		updateGeometry = true;
	}

	void Kill()
	{
		config = BlockConfigs.Instance.GetByName(config.MutateToOnDeath);
		UpdateGeometry();
		world.UpdateNeighboringBlockGeometry(GridX, GridZ);
	}

	void LateUpdate() 
	{
		if (updateGeometry)
		{
			if (config.Geometry == BlockConfig.Geo.Floor)
			{
				FormatFloorCollider();
				GenerateFloorMesh();
			}
			else if (config.Geometry == BlockConfig.Geo.Wall)
			{
				FormatWallCollider();
				GenerateWallMesh();
			}
			else if (config.Geometry == BlockConfig.Geo.Box)
			{
				FormatWallCollider();
				GenerateBoxMesh();
			}
			else
			{
				FormatFloorCollider();
			}

			GetComponent<Collider>().isTrigger = config.IsTrigger;

			GetComponent<Renderer>().material.mainTexture = config.TextureMap;
			GetComponent<Renderer>().material.color = config.BaseColor;

			updateGeometry = false;
		}
	}

	private void FormatWallCollider()
	{
		BoxCollider boxCol = GetComponent<Collider>() as BoxCollider;

		boxCol.size = new Vector3 (world.ScaleX, world.ScaleY, world.ScaleZ);
		boxCol.center = Vector3.zero;
	}

	private void FormatFloorCollider()
	{
		BoxCollider boxCol = GetComponent<Collider>() as BoxCollider;
		
		boxCol.size = new Vector3 (world.ScaleX, 0.1f, world.ScaleZ);
		boxCol.center = new Vector3 (0f, -1.1f, 0f);
	}

	private void GenerateWallMesh()
	{
		CubeTop();
		
		if (world.BlockAt(GridX + 1, GridZ) == null
		    || world.BlockAt(GridX + 1, GridZ).config.Geometry != BlockConfig.Geo.Wall)
		{
			CubeEast();
		}
		
		if (world.BlockAt(GridX - 1, GridZ) == null
			|| world.BlockAt(GridX - 1, GridZ).config.Geometry != BlockConfig.Geo.Wall)
		{
			CubeWest();
		}
		
		if (world.BlockAt(GridX, GridZ + 1) == null
		    || world.BlockAt(GridX, GridZ + 1).config.Geometry != BlockConfig.Geo.Wall)
		{
			CubeNorth();
		}
		
		if (world.BlockAt(GridX, GridZ - 1) == null
		    || world.BlockAt(GridX, GridZ - 1).config.Geometry != BlockConfig.Geo.Wall)
		{
			CubeSouth();
		}

		UpdateMesh();
	}

	private void GenerateFloorMesh()
	{
		CubeBottom();

		UpdateMesh();
	}
	
	private void GeneratePitMesh()
	{
	}
	
	private void GenerateBoxMesh()
	{
		CubeTop();
		CubeBottom();
		CubeEast();
		CubeWest();
		CubeNorth();
		CubeSouth();

		UpdateMesh();
	}

	private void Cube (Vector2 texturePos) 
	{
		newTriangles.Add(faceCount * 4  ); //1
		newTriangles.Add(faceCount * 4 + 1 ); //2
		newTriangles.Add(faceCount * 4 + 2 ); //3
		newTriangles.Add(faceCount * 4  ); //1
		newTriangles.Add(faceCount * 4 + 2 ); //3
		newTriangles.Add(faceCount * 4 + 3 ); //4
		
		newUV.Add(new Vector2 (textureUnit * texturePos.x, textureUnit * texturePos.y + textureUnit));
		newUV.Add(new Vector2 (textureUnit * texturePos.x + textureUnit, textureUnit * texturePos.y + textureUnit));
		newUV.Add(new Vector2 (textureUnit * texturePos.x + textureUnit, textureUnit * texturePos.y));
		newUV.Add(new Vector2 (textureUnit * texturePos.x, textureUnit * texturePos.y));
		
		faceCount++;
	}

	private void CubeTop () 
	{
		newVertices.Add(Vertex(0, 0, 1));
		newVertices.Add(Vertex(1, 0, 1));
		newVertices.Add(Vertex(1, 0, 0));
		newVertices.Add(Vertex(0, 0, 0));

		Cube (textureTop);
	}
	
	private void CubeBottom () 
	{
		newVertices.Add(Vertex(0, -1, 1));
		newVertices.Add(Vertex(1, -1, 1));
		newVertices.Add(Vertex(1, -1, 0));
		newVertices.Add(Vertex(0, -1, 0));

		Cube (textureBottom);
	}
	
	private void CubeNorth () 
	{
		newVertices.Add(Vertex(1, 0, 1));
		newVertices.Add(Vertex(0, 0, 1));
		newVertices.Add(Vertex(0, -1, 1));
		newVertices.Add(Vertex(1, -1, 1));

		Cube (textureSide);
	}

	private void CubeSouth () 
	{
		newVertices.Add(Vertex(0, 0, 0));
		newVertices.Add(Vertex(1, 0, 0));
		newVertices.Add(Vertex(1, -1, 0));
		newVertices.Add(Vertex(0, -1, 0));
		
		Cube (textureSide);
	}
	
	private void CubeEast () 
	{
		newVertices.Add(Vertex(1, 0, 0));
		newVertices.Add(Vertex(1, 0, 1));
		newVertices.Add(Vertex(1, -1, 1));
		newVertices.Add(Vertex(1, -1, 0));
		
		Cube (textureSide);
	}
	
	private void CubeWest () 
	{
		newVertices.Add(Vertex(0, 0, 1));
		newVertices.Add(Vertex(0, 0, 0));
		newVertices.Add(Vertex(0, -1, 0));
		newVertices.Add(Vertex(0, -1, 1));
		
		Cube (textureSide);
	}

	private Vector3 Vertex(int x, int y, int z)
	{
		Vector3 vert = new Vector3(x * world.ScaleX - world.ScaleX / 2,
		                           y * world.ScaleY + world.ScaleY / 2,
		                           z * world.ScaleZ - world.ScaleZ / 2);

		return vert;
	}

	private void UpdateMesh()
	{
		mesh.Clear();
		mesh.vertices = newVertices.ToArray();
		mesh.uv = newUV.ToArray();
		mesh.triangles = newTriangles.ToArray();
		mesh.Optimize();
		mesh.RecalculateNormals();

		newVertices.Clear();
		newUV.Clear();
		newTriangles.Clear();
		
		faceCount = 0;
	}
}
