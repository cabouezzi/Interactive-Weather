using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshData : ScriptableObject {

    public Vector3[] vertices;
    public int[] triangles;

    public MeshData(Vector3[] vertices, int[] triangles) {
        this.vertices = vertices;
        this.triangles = triangles;
    }

}
