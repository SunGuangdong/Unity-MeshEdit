using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class CreationMesh : MonoBehaviour
{/// <summary>
    /// 藤显示网格顶点坐标
    /// </summary>
    //[HideInInspector]
    public Vector3[] MeshVertices;
    /// <summary>
    /// 藤UV纹理坐标
    /// </summary>
    //[HideInInspector]
    public Vector2[] MeshUV;
    /// <summary>
    /// 藤网格三角形索引
    /// </summary>
    [HideInInspector]
    public int[] MeshTriangles;
	void Start() 
    {
	
	}
	
	void Update ()
    {
	
	}
}
