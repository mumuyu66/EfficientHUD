using System;
using System.Collections.Generic;
using UnityEngine;

public class QuadMeshDetail
{
    public int buffIndex;
    public int indicesIndex;
}

public class ActorUIMeshDetail
{
    private UIMeshDataBuffer buffer;
    private List<QuadMeshDetail> quads = new List<QuadMeshDetail>();

    public ActorUIMeshDetail(int quadNum, UIMeshDataBuffer buffer)
    {
        for (int i = 0; i < quadNum; i++)
        {
            Vector3[] vertices = new Vector3[] {
                new Vector3(0,0,0),
                new Vector3(0,0,0),
                new Vector3(0,0,0),
                new Vector3(0,0,0)
            };
            Vector2[] uvs = new Vector2[] {
                new Vector2(0,0),
                new Vector2(0,0),
                new Vector2(0,0),
                new Vector2(0,0),
            };
            int bi, ii;
            bi = buffer.AddQuad(vertices, new Color(0, 0, 0, 0.5f), uvs,out ii, true);
            quads.Add(new QuadMeshDetail() { buffIndex = bi, indicesIndex = ii });
        }
    }

    public void CollapseQuadPostion(int index)
    {
        if (index >= quads.Count || index < 0) return;
        QuadMeshDetail quad = quads[index];
        buffer.CollapseQuad(quad.indicesIndex);
    }

    public void UpdataColor(int index, Color color)
    {
        if (index >= quads.Count || index < 0) return;
        QuadMeshDetail quad = quads[index];
        buffer.UpdataColor(quad.buffIndex,color);
    }

    public void UpdataVertices(int index, Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3)
    {
        if (index >= quads.Count || index < 0) return;
        QuadMeshDetail quad = quads[index];
        buffer.UpdataVertices(quad.buffIndex, v0,v1,v2,v3);
    }

    public void UpdataUV(int index, Vector2 v0, Vector2 v1, Vector2 v2, Vector2 v3)
    {
        if (index >= quads.Count || index < 0) return;
        QuadMeshDetail quad = quads[index];
        buffer.UpdataUV(quad.buffIndex, v0, v1, v2, v3);
    }
}

public class ActorUIMeshProvider
{
    private readonly Stack<ActorUIMeshDetail> m_Stack = new Stack<ActorUIMeshDetail>();
    

    public int countAll { get; private set; }
    public int countActive { get { return countAll - countInactive; } }
    public int countInactive { get { return m_Stack.Count; } }
    private UIMeshDataBuffer buffer;
    private int actorHUDQuadNum = 4;
    public ActorUIMeshProvider(int quadNum)
    {
        actorHUDQuadNum = quadNum;
        buffer = new UIMeshDataBuffer();
    }

    public void InitBuff(int size)
    {
        for (int i = 0; i < size; i++)
        {
            ActorUIMeshDetail element = new ActorUIMeshDetail(actorHUDQuadNum, buffer);
            m_Stack.Push(element);
            countAll++;
        }
    }

    public ActorUIMeshDetail Get()
    {
        ActorUIMeshDetail element;
        if (m_Stack.Count == 0)
        {
            element = new ActorUIMeshDetail(actorHUDQuadNum, buffer);
            countAll++;
        }
        else
        {
            element = m_Stack.Pop();
        }
       
        return element;
    }

    public void Release(ActorUIMeshDetail element)
    {
        if (m_Stack.Count > 0 && ReferenceEquals(m_Stack.Peek(), element))
            Debug.LogError("Internal error. Trying to destroy object that is already released to pool.");
        m_Stack.Push(element);
    }
}

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

    public int AddQuad(Vector3[] vertices, Color32 color, Vector2[] uvs, out int indicesIndex, bool collapse = false)
    {
        int startIndex = mVertices.Count;

        mVertices.AddRange(vertices);
        mUv0s.AddRange(uvs);

        mColors.Add(color);
        mColors.Add(color);
        mColors.Add(color);
        mColors.Add(color);

        indicesIndex = mIndices.Count;

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

    public void CollapseQuad(int indicesIndex)
    {
        for (int i = indicesIndex; i < indicesIndex + 6; i++)
        {
            mIndices[i] = indicesIndex;
        }
    }

    public void UpdataColor(int index,Color color)
    {
        for (int i = index; i < index + 4; i++)
        {
            mColors[i] = color;
        }
    }

    public void UpdataVertices(int index, Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3)
    {
        mVertices[index] = v0;
        mVertices[index + 1] = v1;
        mVertices[index + 2] = v2;
        mVertices[index + 3] = v3;
    }

    public void UpdataUV(int index, Vector2 v0, Vector2 v1, Vector2 v2, Vector2 v3)
    {
        mUv0s[index] = v0;
        mUv0s[index + 1] = v1;
        mUv0s[index + 2] = v2;
        mUv0s[index + 3] = v3;
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

