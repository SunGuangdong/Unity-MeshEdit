using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(CreationVine))]
public class CreationVineEntity : Editor
{
    #region 藤网格数据
    /// <summary>
    /// 藤生成方法
    /// </summary>
    private CreationVine vine;
    /// <summary>
    /// 藤显示网格顶点坐标
    /// </summary>
    private Vector3[] VineVertices;
    /// <summary>
    /// 藤UV纹理坐标
    /// </summary>
    private Vector2[] VineUV;
    /// <summary>
    /// 藤网格三角形索引
    /// </summary>
    private int[] VineTriangles;
    /// <summary>
    /// 创建的网格
    /// </summary>
    private MeshFilter mesh;
    /// <summary>
    /// 存储移动的骨骼点
    /// </summary>
    private List<Vector3> MoveVertexs;
    /// <summary>
    /// 藤的宽度
    /// </summary>
    private float VineWidth = 1.5f;
    /// <summary>
    /// 藤的宽度的最小值
    /// </summary>
    private const float MIN_VINE_WIDTH = .1f;
    /// <summary>
    /// 藤的宽度的最大值
    /// </summary>
    private const float MAX_VINE_WIDTH = 1f;
    /// <summary>
    /// 是否开始显示
    /// </summary>
    private bool acceptInput = false;
    #endregion

    /// <summary>
    /// 当选中的游戏对象含有 CreationVine类的时候调用该方法初始化
    /// </summary>
    public void OnEnable()
    {
        vine = (CreationVine)target;

        mesh = vine.GetComponent<MeshFilter>();
        Vector3 ver = mesh.transform.position;
        if (vine.MoveVertexs == null || vine.MoveVertexs.Count == 0)
        {
            MoveVertexs = new List<Vector3>();
            MoveVertexs.Add(new Vector3(-1 + ver.x, ver.y, 0));
            MoveVertexs.Add(new Vector3(1 + ver.x, ver.y, 0));
        }
        else
        {
            MoveVertexs = vine.MoveVertexs;
            for (int x = 0; x < MoveVertexs.Count; x++)
            {
                MoveVertexs[x] = MoveVertexs[x] + ver;
                MoveVertexs.Add(MoveVertexs[x]);
            }
        }
    }
    [MenuItem("Plug/AddVine")]
    public static void AddMesh()
    {
        GameObject obj = new GameObject("Vine");
        obj.AddComponent<CreationVine>();
        //obj.GetComponent<MeshRenderer>().materials[0];
    }

    public override void OnInspectorGUI()
    {
        GUI.changed = false;
        vine.VineWidth = EditorGUILayout.Slider("Vine Width",vine.VineWidth,MIN_VINE_WIDTH,MAX_VINE_WIDTH);

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        if (acceptInput)
        {
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("关 闭 Vine 编 辑"))
            {
                //line.enabled = false;
                ///关闭插件
                acceptInput = false;
                SceneView.RepaintAll();
            }

        }
        else
        {
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("开 始 Vine 编 辑"))
            {
                //line.enabled = true;
                acceptInput = true;
                SceneView.RepaintAll();
            }
        }
    }
    Vector3 tp;
    public void OnSceneGUI()
    {
        if (!acceptInput)
            return;

        DrawHandleGUI(MoveVertexs);

        #region 创建藤节点坐标显示轴
        for (int i = 0; i < MoveVertexs.Count; i++)
        {
            tp = MoveVertexs[i];
            MoveVertexs[i] = Handles.PositionHandle(MoveVertexs[i], Quaternion.identity);

            if (tp != MoveVertexs[i])
            {
                Vector3 p = MoveVertexs[i];
                p.z = MoveVertexs[i].z;
                MoveVertexs[i] = p;
                
            }
        }
        #endregion


        VineVertices = WireToMesh.GetMeshVertexs(MoveVertexs, vine.transform.position,2.56f);
        VineTriangles = WireToMesh.GetMeshTriangles(VineVertices);
        VineUV = WireToMesh.CountMeshUV(VineVertices,vine.VineWidth);
        Mesh meshs = new Mesh();
        meshs.name = "Mesh";
        meshs.vertices = VineVertices;
        meshs.uv = VineUV;
        meshs.triangles = VineTriangles;
        mesh.mesh = meshs;

    }

    /// <summary>
    /// 显示藤转折节点
    /// </summary>
    /// <param name="points">藤节点</param>
    public void DrawHandleGUI(List<Vector3> points)
    {
        if (points == null || points.Count < 1)
            return;
        Handles.BeginGUI();
        for (int i = 0; i < points.Count; i++)
        {
            Vector2 p = HandleUtility.WorldToGUIPoint(points[i]);

            GUI.backgroundColor = Color.red;
            if (GUI.Button(new Rect(p.x - 30, p.y - 30, 25, 25), "x"))
                DeletePoint(i);

            GUI.backgroundColor = Color.green;
            if (GUI.Button(new Rect(p.x + 10, p.y - 30, 25, 25), "+"))
                AddPoint(i);

            GUI.Label(new Rect(p.x, p.y - 50, 200, 25), "Point: " + i.ToString());
        }
        GUI.backgroundColor = Color.white;
        Handles.EndGUI();
    }
    /// <summary>
    /// 删除该节点
    /// </summary>
    /// <param name="index">节点下标</param>
    public void DeletePoint(int index)
    {
        Debug.Log("Delete Point" + index);
        MoveVertexs.RemoveAt(index);
        SceneView.RepaintAll();//重新绘制所有
    }

    /// <summary>
    /// 添加新的节点
    /// </summary>
    /// <param name="index"></param>
    public void AddPoint(int index)
    {
        if (index == MoveVertexs.Count - 1)
        {
            MoveVertexs.Add(new Vector3(MoveVertexs[index].x + 1,MoveVertexs[index].y,MoveVertexs[index].z));
        }
        else
        {
            Vector3 pon = new Vector3(MoveVertexs[index].x + 0.5f,MoveVertexs[index].y,MoveVertexs[index].z);
            Vector3 outs;
            MoveVertexs.Add(MoveVertexs[MoveVertexs.Count - 1]);
            for (int x = index + 1; x < MoveVertexs.Count; x++)
            {
                outs = MoveVertexs[x];
                MoveVertexs[x] = pon;
                pon = outs;
            }
        }
        SceneView.RepaintAll();//重新绘制所有
    }
}
