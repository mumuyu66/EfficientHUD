using System;
using System.Collections.Generic;
using UnityEngine;


public class UIMeshDataBuffer
{
    private List<Vector3> mVertices = ListPool<Vector3>.Get();
    private List<Color32> mColors = ListPool<Color32>.Get();
    private List<Vector2> mUv0s = ListPool<Vector2>.Get();
    private List<Vector2> mUv1s = ListPool<Vector2>.Get();
    private List<Vector2> mUv2s = ListPool<Vector2>.Get();
    private List<Vector2> mUv3s = ListPool<Vector2>.Get();
    private List<Vector3> mNormals = ListPool<Vector3>.Get();
    private List<int> mIndices = ListPool<int>.Get();

    public int AddQuad(Vector3[] vertices, Color32 color, Vector2[] uvs, bool collapse = false)
    {
        int startIndex = mVertices.Count;

        mVertices.AddRange(vertices);
        mUv0s.AddRange(uvs);

        mColors.Add(color);
        mColors.Add(color);
        mColors.Add(color);
        mColors.Add(color);

        if (collapse)
        {
            mIndices.Add(startIndex);
            mIndices.Add(startIndex );
            mIndices.Add(startIndex );

            mIndices.Add(startIndex);
            mIndices.Add(startIndex);
            mIndices.Add(startIndex);
        }
        else
        {
            mIndices.Add(startIndex);
            mIndices.Add(startIndex + 1);
            mIndices.Add(startIndex + 2);

            mIndices.Add(startIndex + 1);
            mIndices.Add(startIndex + 3);
            mIndices.Add(startIndex + 2);
        }
        return startIndex;
    }

    public void FillMesh(Mesh mesh)
    {
        mesh.SetVertices(mVertices);
        mesh.SetColors(mColors);
        mesh.SetUVs(0, mUv0s);
       // mesh.SetUVs(1, mUv1s);
       // mesh.SetUVs(2, mUv2s);
       // mesh.SetUVs(3, mUv3s);
      //  mesh.SetNormals(mNormals);
        mesh.SetTriangles(mIndices, 0);
      //  mesh.RecalculateBounds();
    }
}

