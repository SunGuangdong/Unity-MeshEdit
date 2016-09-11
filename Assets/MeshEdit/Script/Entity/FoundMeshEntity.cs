using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(CreationMesh))]
public class FoundMeshEntity : Editor 
{
    /// <summary>
    /// 网格对象
    /// </summary>
    private CreationMesh meshs;
    /// <summary>
    /// 藤显示网格顶点坐标
    /// </summary>
    private Vector3[] MeshVertices;
    /// <summary>
    /// 藤UV纹理坐标
    /// </summary>
    private Vector2[] MeshUV;
    /// <summary>
    /// 藤网格三角形索引
    /// </summary>
    private int[] MeshTriangles;
    /// <summary>
    /// 藤显示网格顶点坐标
    /// </summary>
    private List<Vector3> MeshPath;
    /// <summary>
    /// 创建的网格
    /// </summary>
    private MeshFilter mesh;
    /// <summary>
    /// 控制开关
    /// </summary>
    private bool acceptInput;
    private bool isEnable;
    /// <summary>
    /// 当选中的游戏对象含有 CreationVine类的时候调用该方法初始化
    /// </summary>
    public void OnEnable()
    {
        isEnable = true;
        meshs = (CreationMesh)target;
        MeshPath = new List<Vector3>();
        mesh = meshs.GetComponent<MeshFilter>();
        Vector3 ver = meshs.transform.position;
        if (meshs.MeshVertices == null || meshs.MeshVertices.Length == 0)
        {
            MeshPath.Add(new Vector3(-1 + ver.x, -1 + ver.y, 0));
            MeshPath.Add(new Vector3(1 + ver.x, -1 + ver.y, 0));
            MeshPath.Add(new Vector3(1 + ver.x, 1 + ver.y, 0));
            MeshPath.Add(new Vector3(-1 + ver.x, 1 + ver.y, 0));
            
        }
        else
        {
            MeshVertices = meshs.MeshVertices;
            for(int x = 0; x < MeshVertices.Length; x++)
            {
                MeshVertices[x] = MeshVertices[x] + ver;
                MeshPath.Add(MeshVertices[x]);
            }
        }
    }


    [MenuItem("Plug/AddMesh")]
    public static void AddMesh()
    {
        GameObject obj = new GameObject("Mesh");
        obj.AddComponent<CreationMesh>();
        //obj.GetComponent<MeshRenderer>().materials[0];
    }

    bool showPosition;
    public override void OnInspectorGUI()
    {
        GUI.changed = false;
        showPosition = EditorGUILayout.Foldout(showPosition, "MeshPoint");
        if (showPosition)
        {
            for(int x = 0; x < MeshPath.Count;x++)
            {
                EditorGUILayout.Vector3Field("P" + x ,MeshPath[x]);
            }
        }
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        if (acceptInput)
        {
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("关 闭 Mesh 编 辑"))
            {
                isEnable = false;
                //line.enabled = false;
                ///关闭插件
                meshs.MeshVertices = GetMeshVertices(MeshPath);
                acceptInput = false;
                SceneView.RepaintAll();
            }

        }
        else
        {
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("开 始 Mesh 编 辑"))
            {
                if (!isEnable)
                {
                    OnEnable();
                }
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

        DrawHandleGUI(MeshPath);
        #region 创建藤节点坐标显示轴
        for (int i = 0; i < MeshPath.Count; i++)
        {
            MeshPath[i] = Handles.PositionHandle(MeshPath[i], Quaternion.identity);
            tp = MeshPath[i];
            if (tp != MeshPath[i])
            {
                Vector3 p = MeshPath[i];
                p.z = MeshPath[i].z;
                MeshPath[i] = p;

            }
        }
        #endregion

        MeshVertices = GetMeshVertices(MeshPath);
        MeshTriangles = Triangulate.Points(MeshVertices);
        Mesh meshS = new Mesh();
        meshS.name = "Mesh2";
        meshS.vertices = MeshVertices;
        meshS.uv = GetMeshUV(MeshPath);
        meshS.triangles = MeshTriangles;
        mesh.mesh = meshS;

    }

    private Vector2[] GetMeshUV(List<Vector3> _Points)
    {
        Vector2[] ver = new Vector2[_Points.Count];
        for (int x = 0; x < ver.Length; x++)
        {
            ver[x].x = _Points[x].x;
            ver[x].y = _Points[x].y;
        }
        return ver;
    }
    private Vector3[] GetMeshVertices(List<Vector3> _Points)
    {
        Vector3[] ver = new Vector3[_Points.Count];
        for (int x = 0; x < ver.Length; x++)
        {
            ver[x] = _Points[x] - meshs.transform.position;
        }
        return ver;
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
        MeshPath.RemoveAt(index);
        SceneView.RepaintAll();//重新绘制所有
    }

    /// <summary>
    /// 添加新的节点
    /// </summary>
    /// <param name="index"></param>
    public void AddPoint(int index)
    {
        if (index == MeshPath.Count - 1)
        {
            MeshPath.Add(new Vector3(MeshPath[index].x + 1, MeshPath[index].y, MeshPath[index].z));
        }
        else
        {
            Vector3 pon = new Vector3(MeshPath[index].x + 0.5f, MeshPath[index].y, MeshPath[index].z);
            Vector3 outs;
            MeshPath.Add(MeshPath[MeshPath.Count - 1]);
            for (int x = index + 1; x < MeshPath.Count; x++)
            {
                outs = MeshPath[x];
                MeshPath[x] = pon;
                pon = outs;
            }
        }
        SceneView.RepaintAll();//重新绘制所有
    }
}
