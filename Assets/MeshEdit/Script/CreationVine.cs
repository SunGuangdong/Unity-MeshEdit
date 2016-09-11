using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class CreationVine : MonoBehaviour 
{
    /// <summary>
    /// 藤的转折点坐标
    /// </summary>
    public List<Vector3> MoveVertexs;
    /// <summary>
    /// 藤显示网格顶点坐标
    /// </summary>
    //[HideInInspector]
    public Vector3[] VineVertices;
    /// <summary>
    /// 藤UV纹理坐标
    /// </summary>
    //[HideInInspector]
    public Vector2[] VineUV;
    /// <summary>
    /// 藤网格三角形索引
    /// </summary>
    [HideInInspector]
    public int[] VineTriangles;

    /// <summary>
    /// 藤的宽度
    /// </summary>
    public float VineWidth = 1.5f;
    private MeshFilter meshFilter;
    void Start()
    {
        LineRenderer line = GetComponent<LineRenderer>();
        
        meshFilter = GetComponent<MeshFilter>();
        VineVertices = meshFilter.mesh.vertices;
        VineTriangles = meshFilter.mesh.triangles;
        VineUV = meshFilter.mesh.uv;
    }
    Mesh mesh;
    void Update()
    {
        mesh = new Mesh();
        mesh.name = "Mesh";
        mesh.vertices = VineVertices;
        mesh.triangles = VineTriangles;
        mesh.uv = VineUV;
        meshFilter.mesh = mesh;
    }
    
}
