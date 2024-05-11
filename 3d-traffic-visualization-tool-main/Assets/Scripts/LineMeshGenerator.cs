using System.Collections.Generic;
using UnityEngine;

public static class LineMeshGenerator
{
    public static Mesh CreateLineMesh(List<Vector3> points, float width)
    {
        var mesh = new Mesh();

        var vertices = new List<Vector3>();
        var triangles = new List<int>();
        var uvs = new List<Vector2>();

        float totalDistance = 0f;
        for (int i = 0; i < points.Count - 1; i++)
        {
            Vector3 startPoint = points[i];
            Vector3 endPoint = points[i + 1];

            Vector3 direction = (endPoint - startPoint).normalized;
            Vector3 normal = new Vector3(-direction.z, 0, direction.x);

            Vector3 offsetStart1;
            Vector3 offsetStart2;
            Vector3 offsetEnd1;
            Vector3 offsetEnd2;

            float segmentDistance = Vector3.Distance(startPoint, endPoint);
            float uStart = totalDistance;
            float uEnd = totalDistance + segmentDistance;
            totalDistance = uEnd;

            if (i == 0) // First segment
            {
                offsetStart1 = startPoint + normal * (width / 2f);
                offsetStart2 = startPoint - normal * (width / 2f);
            }
            else // Calculate miter joint for others
            {
                Vector3 prevEndPoint = points[i - 1];
                Vector3 prevDirection = (startPoint - prevEndPoint).normalized;
                Vector3 prevNormal = new Vector3(-prevDirection.z, 0, prevDirection.x);

                Vector3 miter = (normal + prevNormal).normalized;
                float length = (width / 2f) / Vector3.Dot(miter, normal);

                offsetStart1 = startPoint + miter * length;
                offsetStart2 = startPoint - miter * length;
            }

            if (i < points.Count - 2) // If not the last segment, calculate miter joint for end point as well
            {
                Vector3 nextStartPoint = points[i + 2];
                Vector3 nextDirection = (nextStartPoint - endPoint).normalized;
                Vector3 nextNormal = new Vector3(-nextDirection.z, 0, nextDirection.x);

                Vector3 miter = (normal + nextNormal).normalized;
                float length = (width / 2f) / Vector3.Dot(miter, normal);

                offsetEnd1 = endPoint + miter * length;
                offsetEnd2 = endPoint - miter * length;
            }
            else // Last segment
            {
                offsetEnd1 = endPoint + normal * (width / 2f);
                offsetEnd2 = endPoint - normal * (width / 2f);
            }

            vertices.Add(offsetStart1);
            vertices.Add(offsetStart2);
            vertices.Add(offsetEnd1);
            vertices.Add(offsetEnd2);

            int vertexIndex = i * 4;
            triangles.Add(vertexIndex);
            triangles.Add(vertexIndex + 2);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 2);
            triangles.Add(vertexIndex + 3);

            uvs.Add(new Vector2(uStart, 0)); // Lower left
            uvs.Add(new Vector2(uStart, 1)); // Upper left
            uvs.Add(new Vector2(uEnd, 0)); // Lower right
            uvs.Add(new Vector2(uEnd, 1)); // Upper right
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        mesh.RecalculateBounds();

        return mesh;
    }
}
