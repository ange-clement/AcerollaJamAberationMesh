using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BigMaths
{
    private static List<int> findKNN(Vector3 pos, int knn, Vector3[] points)
    {
        List<int> knnIndices = new List<int>(knn);
        List<float> knnValues = new List<float>(knn);
        for (int i = 0; i < knn; i++)
        {
            knnIndices.Add(-1);
            knnValues.Add(99999999f);
        }

        for (int i = 0; i < points.Length; i++)
        {
            float value = (points[i] - pos).sqrMagnitude;
            int knnIndex = knn - 1;
            while(value < knnValues[knnIndex] && knnIndex > 0)
            { 
                int previousKnnIndex = knnIndex - 1;
                knnValues[knnIndex] = knnValues[previousKnnIndex];
                knnIndices[knnIndex] = knnIndices[previousKnnIndex];
                knnIndex = previousKnnIndex;
            }
            if (knnIndex != knn - 1)
            {
                if (knnIndex != 0 && value >= knnValues[knnIndex])
                    knnIndex = knnIndex + 1;
                knnValues[knnIndex] = value;
                knnIndices[knnIndex] = i;
            }

        }

        return knnIndices;
    }

    private static (Vector3, Vector3) projectOnSurface(Vector3 point, int knn, Vector3[] points, Vector3[] normals)
    {
        List<int> knnIndices = findKNN(point, knn, points);

        Vector3 centerPoint = Vector3.zero;
        Vector3 centerNormal = Vector3.zero;
        for (int i = 0; i < knnIndices.Count; i++)
        {
            //Debug.DrawLine(point, points[knnIndices[i]], Color.white);
            centerPoint += points[knnIndices[i]];
            centerNormal += normals[knnIndices[i]];
        }

        centerPoint /= knnIndices.Count;
        centerNormal /= knnIndices.Count;

        return (centerPoint + Vector3.ProjectOnPlane(point - centerPoint, centerNormal), centerNormal);
    }
    private static (bool, Vector3) doesIntersect(Vector3 startPos, Vector3 endPos, int knn, Vector3[] points, Vector3[] normals)
    {
        (Vector3 startProj, Vector3 centralNormalStart) = projectOnSurface(startPos, knn, points, normals);
        Vector3 startDiff = startPos - startProj;
        (Vector3 endProj, Vector3 centralNormalEnd) = projectOnSurface(endPos, knn, points, normals);
        Vector3 endDiff = endPos - endProj;

        return (Vector3.Dot(startDiff, endDiff) < 0, (startPos + endPos) / 2);
    }

    private static void addPlane(Vector3 pos, Vector3 v0, Vector3 v1, List<Vector3> vertices, List<int> triangles)
    {
        int startIndex = vertices.Count;

        vertices.Add(pos - v0 - v1);
        vertices.Add(pos + v0 - v1);
        vertices.Add(pos - v0 + v1);
        vertices.Add(pos + v0 + v1);

        triangles.Add(startIndex);
        triangles.Add(startIndex + 1);
        triangles.Add(startIndex + 3);
        triangles.Add(startIndex);
        triangles.Add(startIndex + 3);
        triangles.Add(startIndex + 1);

        triangles.Add(startIndex);
        triangles.Add(startIndex + 3);
        triangles.Add(startIndex + 2);
        triangles.Add(startIndex);
        triangles.Add(startIndex + 2);
        triangles.Add(startIndex + 3);
    }

    public static (List<Vector3>, List<int>) dualContour(Vector3 startPos, Vector3 right, Vector3 up, Vector3 forward, int subSample, int knn, Vector3[] points, Vector3[] normals)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        Vector3 r2 = right * 0.5f;
        Vector3 u2 = up * 0.5f;
        Vector3 f2 = forward * 0.5f;

        for (int x = 0; x < subSample; x++)
            for (int y = 0; y < subSample; y++)
                for (int z = 0; z < subSample; z++)
                {
                    Vector3 pos = startPos + (x+.5f) * right + (y+.5f) * up + (z+.5f) * forward;
                    Vector3 r = pos + right;
                    Vector3 u = pos + up;
                    Vector3 f = pos + forward;

                    bool intersect;
                    Vector3 intersectPos;
                    // X edge = YZ plane
                    (intersect, intersectPos) = doesIntersect(pos, r, knn, points, normals);
                    if (intersect)
                    {
                        addPlane(intersectPos, u2, f2, vertices, triangles);
                    }
                    // Y edge = XZ plane
                    (intersect, intersectPos) = doesIntersect(pos, u, knn, points, normals);
                    if (intersect)
                    {
                        addPlane(intersectPos, r2, f2, vertices, triangles);
                    }
                    // Z edge = XY plane
                    (intersect, intersectPos) = doesIntersect(pos, f, knn, points, normals);
                    if (intersect)
                    {
                        addPlane(intersectPos, r2, u2, vertices, triangles);
                    }
                }

        return (vertices, triangles);
    }

    public static (Vector3[], Vector3[]) makePointCloudFromMesh(Mesh mesh, int subSample, float doNotChooseProba)
    {
        List<Vector3> points = new List<Vector3>(mesh.triangles.Length * subSample);
        List<Vector3> normals = new List<Vector3>(mesh.triangles.Length * subSample);

        for (int i = 0; i < mesh.triangles.Length; i += 3)
        {
            if (doNotChooseProba > 0 && doNotChooseProba < 1 && Random.value < doNotChooseProba)
            {
                continue;
            }
            int t0 = mesh.triangles[i];
            int t1 = mesh.triangles[i + 1];
            int t2 = mesh.triangles[i + 2];

            Vector3 a = mesh.vertices[t0];
            Vector3 b = mesh.vertices[t1];
            Vector3 c = mesh.vertices[t2];

            Vector3 v0 = b - a;
            Vector3 v1 = c - a;

            Vector3 norm = Vector3.Cross(v0, v1).normalized;

            for (int s = 0; s < subSample; s++)
            {
                float r0 = Random.value;
                float r1 = Random.value;

                Vector3 point;

                if (r0 + r1 < 1)
                {
                    point = a + v0 * r0 + v1 * r1;
                }
                else
                {
                    point = a + v0 * (1 - r0) + v1 * (1 - r1);
                }

                points.Add(point);
                normals.Add(norm);
            }
        }

        return (points.ToArray(), normals.ToArray());
    }

    public static float easeInOutCubic(float x)
    {
        return x< 0.5f ? 4f * x* x* x : 1f - Mathf.Pow(-2f * x + 2f, 3f) * .5f;
    }

    public static float easeInOutSine(float x)
    {
        return -(Mathf.Cos(Mathf.PI* x) - 1f) * .5f;
    }
}
