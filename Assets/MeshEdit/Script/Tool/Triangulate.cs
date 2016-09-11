using UnityEngine;
using System.Collections.Generic;
/// <summary>
/// 得到三角形索引
/// </summary>
public static class Triangulate
{
    /// <summary>
    /// 根据传入的多边形坐标返回该多边形的三角形顶点索引-构成多边形的索引
    /// </summary>
    /// <param name="points"></param>
    /// <returns></returns>
    public static int[] Points(Vector3[] points)
    {
        var indices = new List<int>();

        int n = points.Length;
        if (n < 3)
            return indices.ToArray();

        int[] V = new int[n];
        if (Area(points) > 0)
        {
            for (int v = 0; v < n; v++)
                V[v] = v;
        }
        else
        {
            for (int v = 0; v < n; v++)
                V[v] = (n - 1) - v;
        }

        int nv = n;
        int count = 2 * nv;
        for (int m = 0, v = nv - 1; nv > 2; )
        {
            if ((count--) <= 0)
                return indices.ToArray();

            int u = v;
            if (nv <= u)
                u = 0;
            v = u + 1;
            if (nv <= v)
                v = 0;
            int w = v + 1;
            if (nv <= w)
                w = 0;

            if (Snip(points, u, v, w, nv, V))
            {
                int a, b, c, s, t;
                a = V[u];
                b = V[v];
                c = V[w];
                indices.Add(a);
                indices.Add(b);
                indices.Add(c);
                m++;
                for (s = v, t = v + 1; t < nv; s++, t++)
                    V[s] = V[t];
                nv--;
                count = 2 * nv;
            }
        }

        indices.Reverse();
        return indices.ToArray();
    }

    private static float Area(Vector3[] points)
    {
        int n = points.Length;
        float A = 0.0f;
        for (int p = n - 1, q = 0; q < n; p = q++)
        {
            Vector3 pval = points[p];
            Vector3 qval = points[q];
            A += pval.x * qval.y - qval.x * pval.y;
        }
        return (A * 0.5f);
    }

    private static bool Snip(Vector3[] points, int u, int v, int w, int n, int[] V)
    {
        int p;
        Vector3 A = points[V[u]];
        Vector3 B = points[V[v]];
        Vector3 C = points[V[w]];
        if (Mathf.Epsilon > (((B.x - A.x) * (C.y - A.y)) - ((B.y - A.y) * (C.x - A.x))))
            return false;
        for (p = 0; p < n; p++)
        {
            if ((p == u) || (p == v) || (p == w))
                continue;
            Vector3 P = points[V[p]];
            if (InsideTriangle(A, B, C, P))
                return false;
        }
        return true;
    }

    private static bool InsideTriangle(Vector2 A, Vector2 B, Vector2 C, Vector2 P)
    {
        float ax, ay, bx, by, cx, cy, apx, apy, bpx, bpy, cpx, cpy;
        float cCROSSap, bCROSScp, aCROSSbp;

        ax = C.x - B.x; ay = C.y - B.y;
        bx = A.x - C.x; by = A.y - C.y;
        cx = B.x - A.x; cy = B.y - A.y;
        apx = P.x - A.x; apy = P.y - A.y;
        bpx = P.x - B.x; bpy = P.y - B.y;
        cpx = P.x - C.x; cpy = P.y - C.y;

        aCROSSbp = ax * bpy - ay * bpx;
        cCROSSap = cx * apy - cy * apx;
        bCROSScp = bx * cpy - by * cpx;

        return ((aCROSSbp >= 0.0f) && (bCROSScp >= 0.0f) && (cCROSSap >= 0.0f));
    }
}


[System.Serializable]
public class Bezier : System.Object
{
    public Vector3 p0;

    public Vector3 p1;
    public Vector3 p2;

    public Vector3 p3;

    public float ti = 0f;
    private Vector3 b0 = Vector3.zero;
    private Vector3 b1 = Vector3.zero;
    private Vector3 b2 = Vector3.zero;
    private Vector3 b3 = Vector3.zero;
    private float Ax;
    private float Ay;
    private float Bx;
    private float By;
    private float Cx;
    private float Cy;

    public Bezier(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3)
    {
        this.p0 = v0;
        this.p1 = v1;
        this.p2 = v2;
        this.p3 = v3;
    }

    public Bezier(Vector3 v0, Vector3 v1,Vector3 v3)
    {
        this.p0 = v0;
        this.p1 = v1;
        this.p2 = v1;
        this.p3 = v3;
    }
    public void setPoint(Vector3 v0, Vector3 v1, Vector3 v2)
    {
        this.p0 = v0;
        this.p1 = v1; 
        this.p3 = v2;
    }
    /// <summary>
    /// 得到按比例的曲线坐标的点
    /// </summary>
    /// <param name="t">为当前要获取曲线坐标点数 X * (1 / n--曲线坐标点的个数)</param>
    /// <returns></returns>
    public Vector3 GetPointAtTime(float t)
    {
        this.CheckConstant();
        float t2 = t * t;
        float t3 = t * t * t;
        float x = this.Ax * t3 + this.Bx * t2 + this.Cx * t + p0.x;
        float y = this.Ay * t3 + this.By * t2 + this.Cy * t + p0.y;
        return new Vector3(x, y, 0.0f);
    }
    private void SetConstant()
    {
        this.Cx = 3f * ((this.p0.x + this.p1.x) - this.p0.x);
        this.Bx = 3f * ((this.p3.x + this.p2.x) - (this.p0.x + this.p1.x)) - this.Cx;
        this.Ax = this.p3.x - this.p0.x - this.Cx - this.Bx;
        this.Cy = 3f * ((this.p0.y + this.p1.y) - this.p0.y);
        this.By = 3f * ((this.p3.y + this.p2.y) - (this.p0.y + this.p1.y)) - this.Cy;
        this.Ay = this.p3.y - this.p0.y - this.Cy - this.By;
    }
    private void CheckConstant()
    {
        if (this.p0 != this.b0 || this.p1 != this.b1 || this.p2 != this.b2 || this.p3 != this.b3)
            this.SetConstant();
        this.b0 = this.p0;
        this.b1 = this.p1;
        this.b2 = this.p2;
        this.b3 = this.p3;
    }
}

/// <summary>
/// 藤制作工具类
/// 创建网格
/// 调整UV
/// </summary>
public class WireToMesh
{
    /// <summary>
    /// 将传入的藤网格轴坐标传入得到网格顶点坐标
    /// </summary>
    /// <param name="_MeshAxis">藤网格的轴顶点</param>
    /// <returns></returns>
    public static Vector3[] GetMeshVertexs(List<Vector3> _MeshAxis,Vector3 _Point,float _Length)
    {
        if (_MeshAxis == null || _MeshAxis.Count == 0)
        {
            return null;
        }
        int length = _MeshAxis.Count - 1;
        float[] m_Angle = new float[_MeshAxis.Count - 1];
        float[] m_Bisector = new float[_MeshAxis.Count - 2];
        Vector3[] m_VertexLength = new Vector3[length];
        int x = 0;
        for (x = 0; x < length; x++)
        {
            m_Angle[x] = MathfTool.GetVectorDeg(_MeshAxis[x], _MeshAxis[x + 1]);
            m_VertexLength[x] = GetVerticalVector(m_Angle[x], _Length,true);

        }
        Vector3[] meshVertexs = new Vector3[_MeshAxis.Count * 2];

        int m_MeshLength = meshVertexs.Length - 1;
        float angle;
        float included;
        for (x = 0; x < _MeshAxis.Count; x++)
        {
            meshVertexs[x] = _MeshAxis[x] - _Point;
            if (x != 0 && x != length)
            {
                angle = m_VertexLength[x].z + m_VertexLength[x - 1].z;
                if(m_VertexLength[x].z > m_VertexLength[x - 1].z)
                {
                    if (m_VertexLength[x].z - m_VertexLength[x - 1].z >= 180)
                    {
                        angle = (angle + 360) * 0.5f;
                        included = m_VertexLength[x - 1].z + 360 - angle;
                    }
                    else
                    {
                        angle = angle * 0.5f;
                        included = m_VertexLength[x].z - angle;
                    }
                }
                else
                {
                    if (m_VertexLength[x - 1].z - m_VertexLength[x].z >= 180)
                    {
                        angle = (angle + 360) * 0.5f;
                        included = m_VertexLength[x].z + 360 - angle;
                    }
                    else
                    {
                        angle = angle * 0.5f;
                        included = m_VertexLength[x - 1].z - angle;
                    }

                }
                if (included < 0)
                {
                    Debug.Log(included + "  " + x);
                }
                included = _Length / Mathf.Cos(included * Mathf.Deg2Rad);

                meshVertexs[m_MeshLength - x] = _MeshAxis[x] - _Point + GetVerticalVector(angle, included,false);
                meshVertexs[m_MeshLength - x].z = 0;
            }
            else
            {
                meshVertexs[m_MeshLength - x] = meshVertexs[x] + (x == length ? m_VertexLength[x - 1] : m_VertexLength[x]);
                meshVertexs[m_MeshLength - x].z = 0;
            }
        }
        
        return meshVertexs;
    }

    /// <summary>
    /// 根据传入的藤的网格顶点计算相应的网格UV
    /// </summary>
    /// <param name="_MeshVertexs">藤网格顶点</param>
    /// <returns></returns>
    public static Vector2[] CountMeshUV(Vector3[] _MeshVertexs, float _VineWidth)
    {
        Vector2[] UV = new Vector2[_MeshVertexs.Length];
        Vector2[] minMax = GetMinMaxXY(_MeshVertexs);
        int l = _MeshVertexs.Length / 2 - 1;
        float U = minMax[1].x - minMax[0].x;
        float V = minMax[1].y - minMax[0].y;
        for (int x = 0; x < _MeshVertexs.Length; x++)
        {
            //UV[x].x = (_MeshVertexs[x].x - minMax[0].x) / U;
            //UV[x].y = (_MeshVertexs[x].y - minMax[0].y) / V;

            UV[x].x = _MeshVertexs[x].x * _VineWidth;
            if(x == l)
            {
                UV[x].y = 0;
            }
            else if(x == 0)
            {
                UV[x].y = 0;
            }
            else if(x == _MeshVertexs.Length - 1)
            {
                UV[x].y = 1;
            }
            else if(x < l)
            {
                UV[x].y = 0;
            }
            else
            {
                UV[x].y = 1;
            }
            //UV[x].y = UV[x].y < 0 ? (UV[x].y + 360) * Mathf.Deg2Rad : UV[x].y * Mathf.Deg2Rad;
            
        }
        return UV;
    }
    private static Vector2[] GetMinMaxXY(Vector3[] _MeshVertexs)
    {
        Vector2[] minMax = new Vector2[2];
        minMax[0] = _MeshVertexs[0];
        minMax[1] = _MeshVertexs[1];
        for (int x = 0; x < _MeshVertexs.Length; x++)
        {
            minMax[0].x = _MeshVertexs[x].x < minMax[0].x ? _MeshVertexs[x].x : minMax[0].x;
            minMax[0].y = _MeshVertexs[x].y < minMax[0].y ? _MeshVertexs[x].y : minMax[0].y;

            minMax[1].x = _MeshVertexs[x].x > minMax[1].x ? _MeshVertexs[x].x : minMax[1].x;
            minMax[1].y = _MeshVertexs[x].y > minMax[1].y ? _MeshVertexs[x].y : minMax[1].y;
        }
        return minMax;
    }

    /// <summary>
    /// 获取网格三角形索引
    /// </summary>
    /// <param name="_MeshVertexs"></param>
    /// <returns></returns>
    public static int[] GetMeshTriangles(Vector3[] _MeshVertexs)
    {
        int[] m_Triangles = new int[(_MeshVertexs.Length - 2) * 3];
        int length = (int)_MeshVertexs.Length / 2 - 1;
        int counts = _MeshVertexs.Length - 1;
        int x = 0;
        for (int y = 0; y < length; y++)
        {
            m_Triangles[x++] = y;
            m_Triangles[x++] = counts - y;
            m_Triangles[x++] = counts - y - 1;

            m_Triangles[x++] = y;
            m_Triangles[x++] = counts - y - 1;
            m_Triangles[x++] = y + 1;
        }
        return m_Triangles;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_Angle"></param>
    /// <param name="_Length"></param>
    /// <returns></returns>
    private static Vector3 GetVerticalVector(float _Angle, float _Length,bool _Bool)
    {
        Vector3 m_Vertical = new Vector3(0,0,0);
        
        m_Vertical.z = _Bool ?  _Angle + 90 : _Angle;
        m_Vertical.z = m_Vertical.z > 360 ? m_Vertical.z - 360 : m_Vertical.z;
        if (m_Vertical.z <= 90)
        {
            m_Vertical.x = Mathf.Cos(m_Vertical.z * Mathf.Deg2Rad) * _Length;
            m_Vertical.y = Mathf.Sin(m_Vertical.z * Mathf.Deg2Rad) * _Length;
        }
        else if (m_Vertical.z <= 180)
        {
            m_Vertical.x = -Mathf.Sin((m_Vertical.z - 90) * Mathf.Deg2Rad) * _Length;
            m_Vertical.y = Mathf.Cos((m_Vertical.z - 90) * Mathf.Deg2Rad) * _Length;
        }
        else if (_Angle <= 270)
        {
            m_Vertical.x = -Mathf.Cos((m_Vertical.z - 180) * Mathf.Deg2Rad) * _Length;
            m_Vertical.y = -Mathf.Sin((m_Vertical.z - 180) * Mathf.Deg2Rad) * _Length;
        }
        else
        {
            m_Vertical.x = -Mathf.Cos((m_Vertical.z - 270) * Mathf.Deg2Rad) * _Length;
            m_Vertical.y = Mathf.Sin((m_Vertical.z - 270) * Mathf.Deg2Rad) * _Length;
        }
        return m_Vertical;
    }
}

public class MathfTool
{
    /// <summary>
    /// 计算两个向量和X轴的弧度
    /// </summary>
    /// <param name="_Point1">起点</param>
    /// <param name="_Point2">终点</param>
    /// <returns>返回两坐标和X轴的弧度</returns>
    public static float GetVectorRad(Vector3 _Point1, Vector3 _Point2)
    {
        return GetVectorDeg(_Point1,_Point2) * Mathf.Deg2Rad;
    }

    /// <summary>
    /// 返回两个向量和X轴的角度
    /// </summary>
    /// <param name="_Point1">起点</param>
    /// <param name="_Point2">终点</param>
    /// <returns>返回两坐标和X轴的角度</returns>
    public static float GetVectorDeg(Vector3 _Point1, Vector3 _Point2)
    {
        Vector3 m_Point = _Point2 - _Point1;
        float m_Rad = Mathf.Atan(m_Point.y / m_Point.x) * Mathf.Rad2Deg;
        if (m_Point.y < 0 && m_Point.x < 0)
        {
            m_Rad += 180;
        }
        else if (m_Point.x < 0)
            m_Rad = 180 + m_Rad;
        else
            m_Rad = m_Point.y < 0 ? 360 + m_Rad : m_Rad;
        
        m_Rad = m_Rad > 360 ? m_Rad - 360 : m_Rad;
        return m_Rad;
    }

    /// <summary>
    /// 求平分角
    /// </summary>
    /// <param name="_Angle1">左边顶点</param>
    /// <param name="_Angle2">中点</param>
    /// <param name="_Angle3">右边顶点</param>
    /// <returns></returns>
    public static float GetBisectorDeg(Vector3 _Angle1,Vector3 _Angle2,Vector3 _Angle3)
    {
        float m_Angle = (MathfTool.GetVectorDeg(_Angle2, _Angle1) + MathfTool.GetVectorDeg(_Angle2, _Angle3)) * 0.5f;
        return m_Angle;
    }
}
