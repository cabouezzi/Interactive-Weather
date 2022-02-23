using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sphere : MonoBehaviour {

    Mesh mesh;
    MeshData data;
    public int resolution;

    // Start is called before the first frame update
    void Start() {

        mesh = new Mesh();
        this.GetComponent<MeshFilter>().mesh = mesh;

        resolution = 2;

        data = faceData(resolution);

        Debug.Log(data.vertices);
        Debug.Log(data.triangles);

        updateMesh();

    }

    // Update is called once per frame
    void Update() {
        
        updateMesh();

    }
    
    void updateMesh () {

        mesh.Clear();

        data = faceData(resolution);
        mesh.vertices = data.vertices;
        mesh.triangles = data.triangles;

        mesh.RecalculateNormals();

    }

    MeshData faceData (int resolution) {

        if (resolution < 1) {
            resolution = 1;
        }
        
        Vector3[] vertices = new Vector3[resolution * resolution];
        int[] triangles = new int[(resolution-1) * (resolution-1) * 6];

        int ti = 0;

        for (int y = 0; y < resolution; y++) {
            for (int x = 0; x < resolution; x++) {
            
                int vi = x + y * resolution;
                float xp = (float) x;
                float yp = (float) y;
                float mag = (float) Math.Sqrt(xp*xp + yp*yp);
                Vector3 point = new Vector3(xp, yp, 0);
                
                vertices[vi] = point;

                if (x != resolution - 1 && y != resolution - 1) {
                    triangles[ti] = vi;
                    triangles[ti + 1] = vi + resolution + 1;
                    triangles[ti + 2] = vi + resolution;
                    triangles[ti + 3] = vi;
                    triangles[ti + 4] = vi + 1;
                    triangles[ti + 5] = vi + resolution + 1;
                    ti += 6;
                }

            }
        }

        return new MeshData(vertices, triangles);

    }


}