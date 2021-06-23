using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseScript : MonoBehaviour
{
    public Camera _camera;
    public float radius = 3;
    MeshShape meshShape;
    
    void Start()
    {
        _camera = this.gameObject.GetComponent<Camera>();
        meshShape = FindObjectOfType<MeshShape>();
    }

    void LateUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            RaycastHit hit;
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Transform objectHit = hit.transform;
                AddPoints(hit.point);

                print("adding");
            }
        }

        if (Input.GetMouseButton(1))
        {
            RaycastHit hit;
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Transform objectHit = hit.transform;
                RemovePoints(hit.point);

                print("adding");
            }
        }
    }


    void AddPoints(Vector3 mouseHit)
    {
        List<Vector3Int> points = new List<Vector3Int>();

        for (int x = 0; x < meshShape.size.x; x++)
        {
            for (int y = 0; y < meshShape.size.y; y++)
            {
                for (int z = 0; z < meshShape.size.z; z++)
                {
                    if ((new Vector3(x,y,z) - mouseHit).magnitude < radius )
                    {
                        points.Add(new Vector3Int(x, y, z));
                    }
                }
            }
        }

        foreach (Vector3Int point in points)
        {
            meshShape.noiseMap[point.x, point.y, point.z] = 0;
        }
        meshShape.AddToMesh();
    }

    void RemovePoints(Vector3 mouseHit)
    {
        List<Vector3Int> points = new List<Vector3Int>();

        for (int x = 0; x < meshShape.size.x; x++)
        {
            for (int y = 0; y < meshShape.size.y; y++)
            {
                for (int z = 0; z < meshShape.size.z; z++)
                {
                    if ((new Vector3(x, y, z) - mouseHit).magnitude < radius)
                    {
                        points.Add(new Vector3Int(x, y, z));
                    }
                }
            }
        }

        foreach (Vector3Int point in points)
        {
            meshShape.noiseMap[point.x, point.y, point.z] = meshShape.thickness + 0.1f;
        }
        meshShape.AddToMesh();
    }
}