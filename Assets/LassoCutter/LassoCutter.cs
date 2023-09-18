using System.Collections.Generic;
using UnityEngine;
using BNG;

public enum CollisionMode { None = 0, Sphere, Mesh}
public enum VertexClosedWith { DownEdge = 0, UpEdge, LeftEdge, RightEdge, InsideOfVertex}


public class LassoCutter : MonoBehaviour
{
    [SerializeField] private CollisionMode collisionMode;
    [SerializeField] private bool generateRigidbody;

    [SerializeField] public LineRenderer[] tempLine;
    [SerializeField] public GameObject CollagePrefab;

    private Mesh mesh;

    private Vector3[] lineVertices;

    [SerializeField] public MeshFilter tempTargetMeshFilter;

    private void OnEnable()
    {
        //Debug.Log(filter.sharedMesh.vertices.Length);
        //Debug.Log(filter.sharedMesh.uv.Length);

    }

    //public void OnReleasedLine(LineRenderer line)
    //{
    //    StartCoroutine(GetMesh(line));
    //}
    float Map(float x, float in_min, float in_max, float out_min, float out_max)
    {
        return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
    }

    Vector2 Map(Vector2 x, float in_min, float in_max, float out_min, float out_max)
    {
        return new Vector2(Map(x.x, in_min, in_max, out_min, out_max), Map(x.y, in_min, in_max, out_min, out_max));
    }

    // Start is called before the first frame update
    private void Update()
    {/*
        if(tempLine!=null && tempTargetMeshFilter)
        {
            if (tempLine.Length == 1)
            {
                CutFromQuad(tempLine[0], tempTargetMeshFilter);
            }
            else
            {
                CutFromQuad(tempLine, tempTargetMeshFilter);
            }
            enabled = false;
            //Destroy(gameObject);
        }*/
    }

    

    public void CutFromQuad(LineRenderer[] lines , MeshFilter targetMeshFilter)
    {
        mesh = new Mesh()
        {
            name = "Procedural Mesh"
        };

        List<Vector3> tempVertices = new List<Vector3>();

        for(int i =0; i < lines.Length; ++i)
        {
            if (!lines[i].useWorldSpace)
            {
                for (int j = 0; j < lines[i].positionCount; ++j)
                {
                    tempVertices.Add( lines[i].transform.TransformPoint(lines[i].GetPosition(j)));
                }
            }
            else
            {
                Vector3[] temp = new Vector3[lines[i].positionCount];
                lines[i].GetPositions(temp);
                tempVertices.AddRange(temp);
            }
        }
        

        lineVertices = tempVertices.ToArray();

        CreateCuttedQuad(targetMeshFilter);
    }

    public void CutFromQuad(LineRenderer line, MeshFilter targetMeshFilter)
    {

        mesh = new Mesh()
        {
            name = "Procedural Mesh"
        };
        //line.BakeMesh(mesh);
        lineVertices = new Vector3[line.positionCount];
        line.GetPositions(lineVertices);


        //Vector3[] targetMeshVertices = targetMeshFilter.sharedMesh.vertices;

        //for (int i = 0; i < targetMeshVertices.Length; ++i)
        //{
        //    targetMeshVertices[i] = targetMeshFilter.transform.TransformPoint(targetMeshVertices[i]);
        //}

        if (!line.useWorldSpace)
        {
            for (int i = 0; i < lineVertices.Length; ++i)
            {
                lineVertices[i] = line.transform.TransformPoint(lineVertices[i]);
            }
        }
        //Vector3 planeEdgeA= Vector3.Normalize(targetMeshVertices[1] - targetMeshVertices[0]);
        // Vector3 planeEdgeB= Vector3.Normalize(targetMeshVertices[2] - targetMeshVertices[0]);
        //Vector3 planeNormal = Vector3.Cross(planeEdgeA, planeEdgeB);
        CreateCuttedQuad(targetMeshFilter);
    }

    private void CreateCuttedQuad(MeshFilter targetMeshFilter)
    {
        bool intersedted = false;
        Vector2 crossedPoint = Vector2.zero;
        int lassoFirst = 0;
        int lassoLast = lineVertices.Length - 1;
        for (int i = 3; i < lineVertices.Length - 1; ++i)
        {
            for (int j = 0; j < i - 2; ++j)
            {
                if (LineSegmentsIntersectionWithPrecisonControl(
                   targetMeshFilter.transform.InverseTransformPoint(lineVertices[i])// planeNormal)
                   , targetMeshFilter.transform.InverseTransformPoint(lineVertices[i + 1])
                   , targetMeshFilter.transform.InverseTransformPoint(lineVertices[j])
                   , targetMeshFilter.transform.InverseTransformPoint(lineVertices[j + 1])
                    , ref crossedPoint, 0.0001f))
                {
                    lassoFirst = j + 2;
                    lassoLast = i;
                    intersedted = true;
                    break;
                }
            }
            if (intersedted)
                break;

        }
        Vector3 centerPos;
        Vector3 firstPoint;
        Vector3 lastPoint;
        //bool 
        if (intersedted)
        {
            Debug.Log(lassoFirst + "/" + lassoLast);
            Vector3 worldCrossedPoint = targetMeshFilter.transform.TransformPoint(crossedPoint);

            centerPos = GetAveragePoint(lineVertices, lassoFirst, lassoLast);
            //tempPos = worldCrossedPoint;

            CreateLassoFigure(targetMeshFilter, lassoFirst, lassoLast, centerPos, worldCrossedPoint);

        }
        else
        {
            Vector3 firstLocal = targetMeshFilter.transform.InverseTransformPoint(lineVertices[0]);
            bool firstIn = firstLocal.x < 0.5 && firstLocal.x > -0.5 && firstLocal.y < 0.5 && firstLocal.y > -0.5;

            Vector3 lastLocal = targetMeshFilter.transform.InverseTransformPoint(lineVertices[lineVertices.Length - 1]);
            bool lastIn = lastLocal.x < 0.5 && lastLocal.x > -0.5 && lastLocal.y < 0.5 && lastLocal.y > -0.5;

            centerPos = GetAveragePoint(lineVertices, lassoFirst, lassoLast);

            if (firstIn && lastIn)
            {

                centerPos = GetAveragePoint(lineVertices, lassoFirst, lassoLast);
                firstPoint = lineVertices[lassoLast];
                lastPoint = lineVertices[lassoLast];
                CreateLassoFigure(targetMeshFilter, lassoFirst, lassoLast, centerPos, firstPoint, lastPoint);
            }
            //else if(firstIn)
            //{

            //}
            //else if (lastIn)
            //{

            //}

        }

        GameObject newObj = new GameObject("Cutted Mesh");
        newObj.AddComponent<MeshFilter>().sharedMesh = mesh;
        newObj.AddComponent<MeshRenderer>().sharedMaterial = targetMeshFilter.GetComponent<MeshRenderer>().sharedMaterial;
        GameObject parentObj = CollagePrefab != null ? Instantiate(CollagePrefab) : new GameObject("Cutted Mesh Parent");
        parentObj.transform.position = newObj.transform.position;
        newObj.transform.parent = parentObj.transform;


        switch (collisionMode)
        {
            case CollisionMode.None:
                break;
            case CollisionMode.Sphere:
                parentObj.AddComponent<SphereCollider>().radius = Mathf.Max(mesh.bounds.size.x, mesh.bounds.size.y);
                break;
            case CollisionMode.Mesh:
                MeshCollider col = parentObj.AddComponent<MeshCollider>();//newObj.AddComponent<MeshCollider>();
                col.sharedMesh = mesh;//AddComponent<SphereCollider>().radius = Mathf.Max(mesh.bounds.size.x, mesh.bounds.size.y);
                col.convex = true;
                break;
        }


        if (generateRigidbody)
        {
            //Rigidbody body = parentObj.AddComponent<Rigidbody>();
            //body.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        }
        if (parentObj.TryGetComponent<CutObj>(out CutObj cutObj))
        {
            cutObj.LassoFigure = newObj.transform;
        }
    }

    public Vector3 GetPlanedPoint(Vector3 worldPos, Transform targetTransform)
    {
        Vector3 pos = targetTransform.InverseTransformPoint(worldPos);
        pos.z = 0;
        return targetTransform.TransformPoint(pos);
    }

    private void CreateLassoFigure(MeshFilter targetMeshFilter, int lassoFirst, int lassoLast, Vector3 centerPos, Vector3 endPoint)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();
        Transform meshTf = targetMeshFilter.transform;

        vertices.Add(GetPlanedPoint(centerPos,meshTf));
        uvs.Add(Map(targetMeshFilter.transform.InverseTransformPoint(centerPos), -0.5f, 0.5f, 0, 1));
        //vertices.AddRange(lineVertices);

        vertices.Add(GetPlanedPoint( endPoint, meshTf));
        uvs.Add(Map(targetMeshFilter.transform.InverseTransformPoint(endPoint), -0.5f, 0.5f, 0, 1));
        for (int i = lassoFirst; i <= lassoLast && i < lineVertices.Length; ++i)
        {
            vertices.Add(GetPlanedPoint(lineVertices[i], meshTf));
            uvs.Add(Map(targetMeshFilter.transform.InverseTransformPoint(lineVertices[i]), -0.5f, 0.5f, 0, 1));
        }

        int index = 2;
        for (; index < vertices.Count; ++index)
        {
            triangles.Add(0);
            triangles.Add(index);
            triangles.Add(index - 1);
        }
        triangles.Add(0);
        triangles.Add(1);
        triangles.Add(vertices.Count - 1//index
            );


        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }

    private void CreateLassoFigure(MeshFilter targetMeshFilter, int lassoFirst, int lassoLast, Vector3 centerPos, Vector3 firstPoint, Vector3 lastPoint)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();
        Transform meshTf = targetMeshFilter.transform;

        vertices.Add(GetPlanedPoint(centerPos, meshTf));
        uvs.Add(Map(targetMeshFilter.transform.InverseTransformPoint(centerPos), -0.5f, 0.5f, 0, 1));
        //vertices.AddRange(lineVertices);

        vertices.Add(GetPlanedPoint(firstPoint, meshTf));
        uvs.Add(Map(targetMeshFilter.transform.InverseTransformPoint(firstPoint), -0.5f, 0.5f, 0, 1));
        for (int i = lassoFirst; i <= lassoLast && i < lineVertices.Length; ++i)
        {
            vertices.Add(GetPlanedPoint(lineVertices[i], meshTf));
            uvs.Add(Map(targetMeshFilter.transform.InverseTransformPoint(lineVertices[i]), -0.5f, 0.5f, 0, 1));
        }
        vertices.Add(GetPlanedPoint(lastPoint, meshTf));
        uvs.Add(Map(targetMeshFilter.transform.InverseTransformPoint(lastPoint), -0.5f, 0.5f, 0, 1));

        int index = 2;
        for (; index < vertices.Count; ++index)
        {
            triangles.Add(0);
            triangles.Add(index);
            triangles.Add(index - 1);
        }
        triangles.Add(0);
        triangles.Add(1);
        triangles.Add(vertices.Count - 1//index
            );


        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }


    //private Vector3 tempPos;

    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawWireSphere(tempPos, 0.1f);
    //}

    public Vector3 GetAveragePoint(Vector3[] posArr, int firstIndex, int lastIndex)
    {
        Vector3 result = Vector3.zero;
        for (int i = firstIndex; i <= lastIndex && i < posArr.Length; i++)
        {
            result += posArr[i];
        }
        return result / Mathf.Max(((lastIndex < posArr.Length ? lastIndex : posArr.Length - 1) - firstIndex), 1);
    }

    public static bool LineSegmentsIntersectionWithPrecisonControl(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, ref Vector2 intersection, float fSelfDefinedEpsilon = 1.0f)
    {
        //Debug.Log (string.Format("LineSegmentsIntersection2 p1 {0} p2 {1} p3 {2} p4{3}", p1, p2, p3, p4)); // the float value precision in the log is just 0.0f
        UnityEngine.Assertions.Assert.IsTrue(fSelfDefinedEpsilon > 0);

        float Ax, Bx, Cx, Ay, By, Cy, d, e, f, num/*,offset*/;
        float x1lo, x1hi, y1lo, y1hi;
        Ax = p2.x - p1.x;
        Bx = p3.x - p4.x;

        // X bound box test/
        if (Ax < 0)
        {
            x1lo = p2.x; x1hi = p1.x;
        }
        else
        {
            x1hi = p2.x; x1lo = p1.x;
        }

        if (Bx > 0)
        {
            if ((x1hi < p4.x && Mathf.Abs(x1hi - p4.x) > fSelfDefinedEpsilon)
                || (p3.x < x1lo && Mathf.Abs(p3.x - x1lo) > fSelfDefinedEpsilon))
                return false;
        }
        else
        {
            if ((x1hi < p3.x && Mathf.Abs(x1hi - p3.x) > fSelfDefinedEpsilon)
                || (p4.x < x1lo && Mathf.Abs(p4.x - x1lo) > fSelfDefinedEpsilon))
                return false;
        }

        Ay = p2.y - p1.y;
        By = p3.y - p4.y;

        // Y bound box test//
        if (Ay < 0)
        {
            y1lo = p2.y; y1hi = p1.y;
        }
        else
        {
            y1hi = p2.y; y1lo = p1.y;
        }

        if (By > 0)
        {
            if ((y1hi < p4.y && Mathf.Abs(y1hi - p4.y) > fSelfDefinedEpsilon)
                || (p3.y < y1lo && Mathf.Abs(p3.y - y1lo) > fSelfDefinedEpsilon))
                return false;
        }
        else
        {
            if ((y1hi < p3.y && Mathf.Abs(y1hi - p3.y) > fSelfDefinedEpsilon)
                || (p4.y < y1lo && Mathf.Abs(p4.y - y1lo) > fSelfDefinedEpsilon))
                return false;
        }

        Cx = p1.x - p3.x;
        Cy = p1.y - p3.y;
        d = By * Cx - Bx * Cy;  // alpha numerator//
        f = Ay * Bx - Ax * By;  // both denominator//

        // alpha tests//

        if (f > 0)
        {
            if ((d < 0 && Mathf.Abs(d) > fSelfDefinedEpsilon)
                || (d > f && Mathf.Abs(d - f) > fSelfDefinedEpsilon))
                return false;
        }
        else
        {
            if ((d > 0 && Mathf.Abs(d) > fSelfDefinedEpsilon)
                || (d < f && Mathf.Abs(d - f) > fSelfDefinedEpsilon))
                return false;
        }
        e = Ax * Cy - Ay * Cx;  // beta numerator//

        // beta tests //

        if (f > 0)
        {
            if ((e < 0 && Mathf.Abs(e) > fSelfDefinedEpsilon)
                || (e > f) && Mathf.Abs(e - f) > fSelfDefinedEpsilon)
                return false;
        }
        else
        {
            if ((e > 0 && Mathf.Abs(e) > fSelfDefinedEpsilon)
                || (e < f && Mathf.Abs(e - f) > fSelfDefinedEpsilon))
                return false;
        }

        // check if they are parallel
        if (f == 0 && Mathf.Abs(f) > fSelfDefinedEpsilon)
            return false;

        // compute intersection coordinates //
        num = d * Ax; // numerator //

        //    offset = same_sign(num,f) ? f*0.5f : -f*0.5f;   // round direction //

        //    intersection.x = p1.x + (num+offset) / f;
        intersection.x = p1.x + num / f;
        num = d * Ay;

        //    offset = same_sign(num,f) ? f*0.5f : -f*0.5f;

        //    intersection.y = p1.y + (num+offset) / f;
        intersection.y = p1.y + num / f;
        return true;
    }
}