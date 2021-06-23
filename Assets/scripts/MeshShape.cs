using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshShape : MonoBehaviour
{
    Mesh mesh;
    MeshFilter meshFilter;
    MeshCollider meshCollider;

    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> uv = new List<Vector2>();

    public bool updateRealtime = false;

    [Range(0.0f, 0.5f)]
    public float thickness = 0.36f;
    [Range(0.0f, 1.0f)]
    public float noisyness = 0.255f;

    public Vector3Int size = new Vector3Int(25, 25, 25);
    [Range(0.0f, 5.0f)]

    public float[,,] noiseMap;

    public int seedAsInteger = 1;

    private int tempUvIndex = 0;

    private void OnValidate()
    {
        if (meshFilter == null || meshCollider == null)
        {
            meshFilter = this.gameObject.GetComponent<MeshFilter>();
            meshCollider = this.gameObject.GetComponent<MeshCollider>();
        }
        if (updateRealtime)
        {
            GenerateNewMesh();
        }
    }

    void Start()
    {
        Random.InitState(seedAsInteger);

        meshFilter = this.gameObject.GetComponent<MeshFilter>();
        meshCollider = this.gameObject.GetComponent<MeshCollider>();
        noiseMap = new float[size.x + 1, size.y + 1, size.z + 1];

        GenerateNoiseMap();
        CreateMesh();
        UpdateMesh();
    }

    void CreateMesh()
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                for (int z = 0; z < size.z; z++)
                {
                    float[] cube = new float[8];
                    for (int i = 0; i < 8; i++)
                    {
                        Vector3Int corner = new Vector3Int(x, y, z) + LookupTable.CornerTable[i];
                        cube[i] = noiseMap[corner.x, corner.y, corner.z];
                    }

                    MarchCube(new Vector3(x, y, z), cube);
                }
            }
        }

        for (int i = 0; i < vertices.Count; i++)
        {
            if (tempUvIndex > 2)
                tempUvIndex = 0;

            if (tempUvIndex == 0)
                uv.Add(new Vector2(0.5f, 1f));
            else if (tempUvIndex == 1)
                uv.Add(new Vector2(1f, 0f));
            else
                uv.Add(new Vector2(0f, 0f));

            tempUvIndex++;
        }
    }


    void GenerateNoiseMap()
    {
        for (int x = 0; x <= size.x; x++)
        {
            for (int y = 0; y <= size.y; y++)
            {
                for (int z = 0; z <= size.z; z++)
                {
                    //Creates Perlin noise and indexes this into a 3D Array
                    float RandomIndex = Perlin3d(x * noisyness, y * noisyness, z * noisyness);
                    noiseMap[x, y, z] = RandomIndex;
                }
            }
        }
    }

    public float Perlin3d(float x, float y, float z)
    {
        float xy = Mathf.PerlinNoise(x + seedAsInteger, y + seedAsInteger);
        float yz = Mathf.PerlinNoise(y + seedAsInteger, z + seedAsInteger);
        float xz = Mathf.PerlinNoise(x + seedAsInteger, z + seedAsInteger);

        float xyz = xy + yz + xz;
        return xyz / 3f;
    }

    int GetMarchSample(float[] cube)
    {
        int configurationIndex = 0;
        for (int i = 0; i < 8; i++)
        {
            //adds a bit value in a sequence of up to 8
            //iserting corners 123 into the sequence 76543210 becomes 00001110
            //this later becomes the index of the Lookuptable
            if (cube[i] > thickness)
            {
                configurationIndex |= 1 << i;
            }

        }
        return configurationIndex;
    }

    void MarchCube(Vector3 position, float[] cube)
    {
        int tableIndex = GetMarchSample(cube);

        if (tableIndex == 0 || tableIndex == 255)
            return;

        int edgeIndex = 0;
        for (int i = 0; i <= 4; i++)
        {
            for (int j = 0; j <= 2; j++)
            {
                //checks the row index and inserts the value
                int rowIndex = LookupTable.triTable[tableIndex, edgeIndex];

                //if the rowIndex is -1 we are done checking the table index
                if (rowIndex == -1)
                    return;

                //stores the required position
                Vector3 vert1 = position + LookupTable.EdgeTable[rowIndex, 0];
                Vector3 vert2 = position + LookupTable.EdgeTable[rowIndex, 1];
                Vector3 vertPosition = (vert1 + vert2) / 2f;

                //adds the vertice and triangle to their respective lists at the required position
                vertices.Add(vertPosition);
                triangles.Add(vertices.Count - 1);

                edgeIndex++;
            }
        }
    }

    public void GenerateNewMesh()
    {
        vertices.Clear();
        triangles.Clear();
        uv.Clear();

        noiseMap = new float[size.x + 1, size.y + 1, size.z + 1];

        GenerateNoiseMap();
        CreateMesh();
        UpdateMesh();
    }

    public void AddToMesh()
    {
        vertices.Clear();
        triangles.Clear();
        uv.Clear();

        //noiseMap = new float[size.x + 1, size.y + 1, size.z + 1];

        //GenerateNoiseMap();
        CreateMesh();
        UpdateMesh();
    }

    public void UpdateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uv.ToArray();

        mesh.RecalculateNormals();
        
        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
    }



    /*private void OnDrawGizmos()
    {
        Random.InitState(1);
        for (int i = 0; i <= size; i++)
        {
            for (int j = 0; j <= size; j++)
            {
                for (int k = 0; k <= size; k++)
                {
                    float RandomIndex = Perlin3d(i * noiseScale, j * noiseScale, k * noiseScale);
                    Gizmos.color = new Color(1, 1, 1);

                    if (RandomIndex < appearThreshold) 
                    {
                        Gizmos.DrawCube(new Vector3(i, j, k), Vector3.one);
                    }
                    
                }
            }
        }
    }*/



    
}
